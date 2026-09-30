using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("receivables")]
public class ReceivableRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("business_id")]
    public Guid BusinessId { get; set; }

    [Column("client_name")]
    public string ClientName { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("is_collected")]
    public bool IsCollected { get; set; }
}
