using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("recurring_income_confirmations")]
public class RecurringIncomeConfirmationRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("recurring_income_id")]
    public Guid RecurringIncomeId { get; set; }

    [Column("period")]
    public DateTime Period { get; set; }

    [Column("confirmed_amount")]
    public decimal ConfirmedAmount { get; set; }

    [Column("income_id")]
    public Guid? IncomeId { get; set; }

    [Column("confirmed_at")]
    public DateTime ConfirmedAt { get; set; }
}
