using System.Security.Cryptography;
using System.Text;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services;

public class PasswordResetService : Interfaces.IPasswordResetService
{
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IUserRepository _userRepository;
    private readonly Interfaces.IEmailService _emailService;

    public PasswordResetService(
        IPasswordResetRepository passwordResetRepository,
        IUserRepository userRepository,
        Interfaces.IEmailService emailService)
    {
        _passwordResetRepository = passwordResetRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return; // Don't reveal whether email exists

        // Invalidate any existing tokens for this user
        await _passwordResetRepository.InvalidateAllUserTokensAsync(user.Id);

        // Generate a cryptographically secure token
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var plainToken = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        // Hash the token for storage
        var tokenHash = HashToken(plainToken);

        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _passwordResetRepository.CreateTokenAsync(resetToken);

        // Send email with the plain token
        await _emailService.SendPasswordResetEmailAsync(email, plainToken);
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var tokenHash = HashToken(token);
        var resetToken = await _passwordResetRepository.GetValidTokenAsync(tokenHash);

        if (resetToken == null)
            throw new InvalidOperationException("Invalid or expired reset token");

        var user = resetToken.User;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _passwordResetRepository.InvalidateTokenAsync(resetToken.Id);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var tokenHash = HashToken(token);
        var resetToken = await _passwordResetRepository.GetValidTokenAsync(tokenHash);
        return resetToken != null;
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
