namespace Verum.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
