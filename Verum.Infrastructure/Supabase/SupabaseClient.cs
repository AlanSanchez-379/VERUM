namespace Verum.Infrastructure.Supabase;

// Wrapper de configuracion para el cliente oficial de Supabase (paquete supabase-csharp).
public class SupabaseClientOptions
{
    public string Url { get; set; } = string.Empty;
    public string AnonKey { get; set; } = string.Empty;
    public bool UseDummyData { get; set; } = true;
}

public static class SupabaseClientFactory
{
    public static global::Supabase.Client Create(SupabaseClientOptions options)
    {
        return new global::Supabase.Client(
            options.Url,
            options.AnonKey,
            new global::Supabase.SupabaseOptions { AutoConnectRealtime = false });
    }
}
