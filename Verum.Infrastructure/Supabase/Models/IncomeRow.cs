using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("incomes")]
public class IncomeRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Column("source")]
    public string Source { get; set; } = string.Empty;

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("expected_date")]
    public DateTime ExpectedDate { get; set; }

    [Column("is_received")]
    public bool IsReceived { get; set; }
}
