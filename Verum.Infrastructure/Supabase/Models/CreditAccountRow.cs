using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("credit_accounts")]
public class CreditAccountRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("credit_limit")]
    public decimal CreditLimit { get; set; }

    [Column("used_amount")]
    public decimal UsedAmount { get; set; }

    [Column("cutoff_date")]
    public DateTime CutoffDate { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }
}
