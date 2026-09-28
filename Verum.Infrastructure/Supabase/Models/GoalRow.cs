using Postgrest.Attributes;
using Postgrest.Models;

namespace Verum.Infrastructure.Supabase.Models;

[Table("goals")]
public class GoalRow : BaseModel
{
    [PrimaryKey("id", shouldInsert: false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("target_amount")]
    public decimal TargetAmount { get; set; }

    [Column("current_amount")]
    public decimal CurrentAmount { get; set; }

    [Column("target_date")]
    public DateTime TargetDate { get; set; }

    [Column("priority")]
    public string Priority { get; set; } = "Media";

    [Column("image_path")]
    public string? ImagePath { get; set; }
}
