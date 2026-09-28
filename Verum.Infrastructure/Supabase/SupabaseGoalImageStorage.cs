using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Verum.Application.Interfaces;

namespace Verum.Infrastructure.Supabase;

// Llama directo a la API REST de Storage (en vez del cliente Storage del
// paquete supabase-csharp) porque ese cliente no reenvía el token de sesión
// seteado via Auth.SetSession: el objeto se subía pero quedaba huérfano y
// desaparecía. Con el token a mano acá, el request queda igual de autenticado
// que el resto de las llamadas a la base (mismas políticas RLS por carpeta).
public class SupabaseGoalImageStorage : IGoalImageStorage
{
    private const string Bucket = "goal-images";

    private static readonly HttpClient Http = new();

    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;
    private readonly SupabaseClientOptions _options;

    public SupabaseGoalImageStorage(global::Supabase.Client client, ICurrentUserService currentUser, SupabaseClientOptions options)
    {
        _client = client;
        _currentUser = currentUser;
        _options = options;
    }

    public async Task<string> UploadAsync(Guid goalId, byte[] bytes, string contentType)
    {
        var extension = contentType switch
        {
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "jpg"
        };
        var path = $"{_currentUser.UserId}/{goalId}.{extension}";

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.Url}/storage/v1/object/{Bucket}/{path}");
        ApplyAuthHeaders(request);
        request.Headers.Add("x-upsert", "true");
        request.Content = new ByteArrayContent(bytes);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        using var response = await Http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return path;
    }

    public async Task<string?> GetSignedUrlAsync(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.Url}/storage/v1/object/sign/{Bucket}/{imagePath}");
        ApplyAuthHeaders(request);
        request.Content = new StringContent("{\"expiresIn\":3600}", Encoding.UTF8, "application/json");

        using var response = await Http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var signedUrl = doc.RootElement.GetProperty("signedURL").GetString();
        return string.IsNullOrEmpty(signedUrl) ? null : $"{_options.Url}/storage/v1{signedUrl}";
    }

    public async Task DeleteAsync(string imagePath)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_options.Url}/storage/v1/object/{Bucket}");
        ApplyAuthHeaders(request);
        var body = JsonSerializer.Serialize(new { prefixes = new[] { imagePath } });
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        using var response = await Http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private void ApplyAuthHeaders(HttpRequestMessage request)
    {
        var accessToken = _client.Auth.CurrentSession?.AccessToken ?? _options.AnonKey;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("apikey", _options.AnonKey);
    }
}
