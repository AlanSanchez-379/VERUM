using Verum.Application.Interfaces;

namespace Verum.Infrastructure.Repositories.Dummy;

// Guarda las imagenes en memoria y las expone como data URI. Solo para el modo sin Supabase.
public class DummyGoalImageStorage : IGoalImageStorage
{
    private static readonly Dictionary<string, (byte[] Bytes, string ContentType)> Store = new();
    private static readonly object Lock = new();

    public Task<string> UploadAsync(Guid goalId, byte[] bytes, string contentType)
    {
        var path = $"dummy/{goalId}";
        lock (Lock)
        {
            Store[path] = (bytes, contentType);
        }
        return Task.FromResult(path);
    }

    public Task<string?> GetSignedUrlAsync(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return Task.FromResult<string?>(null);
        }

        lock (Lock)
        {
            if (!Store.TryGetValue(imagePath, out var entry))
            {
                return Task.FromResult<string?>(null);
            }

            var dataUri = $"data:{entry.ContentType};base64,{Convert.ToBase64String(entry.Bytes)}";
            return Task.FromResult<string?>(dataUri);
        }
    }

    public Task DeleteAsync(string imagePath)
    {
        lock (Lock)
        {
            Store.Remove(imagePath);
        }
        return Task.CompletedTask;
    }
}
