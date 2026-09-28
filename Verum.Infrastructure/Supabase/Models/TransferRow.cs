using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("transfers")]
public class TransferRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("business_id")]
    public Guid BusinessId { get; set; }

    [Column("personal_account_id")]
    public Guid PersonalAccountId { get; set; }

    [Column("business_account_id")]
    public Guid BusinessAccountId { get; set; }

    [Column("direction")]
    public string Direction { get; set; } = "to_business";

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("note")]
    public string Note { get; set; } = string.Empty;

    [Column("date")]
    public DateTime Date { get; set; }
}
