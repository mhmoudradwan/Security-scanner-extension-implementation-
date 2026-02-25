using Core.Entities;

namespace Core.Interfaces;

public interface IPasswordResetRepository
{
    Task CreateTokenAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetValidTokenAsync(string tokenHash);
    Task InvalidateTokenAsync(int tokenId);
    Task InvalidateAllUserTokensAsync(int userId);
}
