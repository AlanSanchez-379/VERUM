using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("business_accounts")]
public class BusinessAccountRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("business_id")]
    public Guid BusinessId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [Column("balance")]
    public decimal Balance { get; set; }
}
