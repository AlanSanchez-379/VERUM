using Verum.Domain.Enums;

namespace Verum.Domain.Entities;

public class Goal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public GoalPriority Priority { get; set; }
    public string? ImagePath { get; set; }
}
