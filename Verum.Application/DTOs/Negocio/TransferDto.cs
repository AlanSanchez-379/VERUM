namespace Verum.Application.DTOs.Negocio;

public record TransferDto(
    Guid Id,
    Guid BusinessId,
    Guid PersonalAccountId,
    string PersonalAccountName,
    Guid BusinessAccountId,
    string BusinessAccountName,
    string Direction,
    decimal Amount,
    string Note,
    DateTime Date);

public record TransferResult(bool Success, string? Error);
