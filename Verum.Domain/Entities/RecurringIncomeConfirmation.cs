namespace Verum.Domain.Entities;

// Registra que un patron de ingreso recurrente ya fue resuelto para un periodo
// (mes) dado: cuanto llego realmente (puede ser 0 si no llego nada), y a que
// Income real quedo ligado (si llego dinero). Nunca se crea sola: siempre es
// resultado de una confirmacion explicita del usuario.
public class RecurringIncomeConfirmation
{
    public Guid Id { get; set; }
    public Guid RecurringIncomeId { get; set; }
    public DateTime Period { get; set; }
    public decimal ConfirmedAmount { get; set; }
    public Guid? IncomeId { get; set; }
    public DateTime ConfirmedAt { get; set; }
}
