namespace Application.Interfaces;

public interface IPasswordResetService
{
    Task RequestPasswordResetAsync(string email);
    Task ResetPasswordAsync(string token, string newPassword);
    Task<bool> ValidateTokenAsync(string token);
}
