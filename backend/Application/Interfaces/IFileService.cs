using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IFileService
{
    Task<string> UploadProfilePictureAsync(int userId, IFormFile file);
    Task DeleteProfilePictureAsync(int userId);
    Task<string?> GetProfilePictureAsync(int userId);
}
