using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("recurring_incomes")]
public class RecurringIncomeRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("source")]
    public string Source { get; set; } = string.Empty;

    [Column("expected_amount")]
    public decimal ExpectedAmount { get; set; }

    [Column("day_of_month")]
    public int DayOfMonth { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}
