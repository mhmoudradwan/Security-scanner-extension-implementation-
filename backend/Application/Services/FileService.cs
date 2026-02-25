using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Core.Interfaces;

namespace Application.Services;

public class FileService : Interfaces.IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly IUserRepository _userRepository;

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/gif" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public FileService(IWebHostEnvironment env, IUserRepository userRepository)
    {
        _env = env;
        _userRepository = userRepository;
    }

    public async Task<string> UploadProfilePictureAsync(int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided");

        if (file.Length > MaxFileSize)
            throw new ArgumentException("File size exceeds the 5MB limit");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type. Allowed types: jpg, jpeg, png, gif");

        if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new ArgumentException("Invalid MIME type. Allowed types: image/jpeg, image/png, image/gif");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Delete existing profile picture if it exists
        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            DeleteFileFromDisk(user.ProfileImageUrl);
        }

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "profile-pictures");
        Directory.CreateDirectory(uploadsDir);

        var filePath = Path.Combine(uploadsDir, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativeUrl = $"/uploads/profile-pictures/{fileName}";

        user.ProfileImageUrl = relativeUrl;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return relativeUrl;
    }

    public async Task DeleteProfilePictureAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            DeleteFileFromDisk(user.ProfileImageUrl);
            user.ProfileImageUrl = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
    }

    public async Task<string?> GetProfilePictureAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.ProfileImageUrl;
    }

    private void DeleteFileFromDisk(string relativeUrl)
    {
        try
        {
            var webRoot = _env.WebRootPath ?? "wwwroot";
            var fullPath = Path.Combine(webRoot, relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch
        {
            // Ignore file deletion errors
        }
    }
}
