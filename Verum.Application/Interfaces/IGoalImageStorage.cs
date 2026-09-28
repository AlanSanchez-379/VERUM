namespace Verum.Application.Interfaces;

public interface IGoalImageStorage
{
    Task<string> UploadAsync(Guid goalId, byte[] bytes, string contentType);
    Task<string?> GetSignedUrlAsync(string? imagePath);
    Task DeleteAsync(string imagePath);
}
