using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("accounts")]
public class AccountRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("type")]
    public string Type { get; set; } = "Banco";

    [Column("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [Column("balance")]
    public decimal Balance { get; set; }
}
