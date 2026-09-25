namespace Verum.Domain.Rules;

public enum ReachStatus
{
    DentroDeAlcance,
    CercaDelLimite,
    FueraDeAlcance
}

// Clasifica si una meta es alcanzable segun el margen actual y el tiempo restante.
public static class ProjectionRule
{
    public static decimal RequiredPerPeriod(decimal remaining, int periodsRemaining)
        => periodsRemaining <= 0 ? remaining : remaining / periodsRemaining;

    public static ReachStatus ClassifyReach(decimal remaining, int periodsRemaining, decimal currentMargin)
    {
        if (remaining <= 0)
        {
            return ReachStatus.DentroDeAlcance;
        }

        if (periodsRemaining <= 0)
        {
            return ReachStatus.FueraDeAlcance;
        }

        var requiredPerPeriod = RequiredPerPeriod(remaining, periodsRemaining);

        if (requiredPerPeriod <= currentMargin)
        {
            return ReachStatus.DentroDeAlcance;
        }

        if (requiredPerPeriod <= currentMargin * 1.3m)
        {
            return ReachStatus.CercaDelLimite;
        }

        return ReachStatus.FueraDeAlcance;
    }
}
