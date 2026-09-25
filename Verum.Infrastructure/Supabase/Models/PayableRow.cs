using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("payables")]
public class PayableRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("business_id")]
    public Guid BusinessId { get; set; }

    [Column("supplier_name")]
    public string SupplierName { get; set; } = string.Empty;

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("is_paid")]
    public bool IsPaid { get; set; }
}
