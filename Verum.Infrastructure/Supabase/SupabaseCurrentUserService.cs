using Verum.Application.Interfaces;

namespace Verum.Infrastructure.Supabase;

public class SupabaseCurrentUserService : ICurrentUserService
{
    private readonly global::Supabase.Client _client;

    public SupabaseCurrentUserService(global::Supabase.Client client)
    {
        _client = client;
    }

    public Guid UserId
    {
        get
        {
            var id = _client.Auth.CurrentUser?.Id;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var userId))
            {
                throw new InvalidOperationException("No hay una sesión activa.");
            }

            return userId;
        }
    }
}
