using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class PasswordResetRepository : IPasswordResetRepository
{
    private readonly SecurityScannerDbContext _context;

    public PasswordResetRepository(SecurityScannerDbContext context)
    {
        _context = context;
    }

    public async Task CreateTokenAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetValidTokenAsync(string tokenHash)
    {
        return await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.TokenHash == tokenHash &&
                !t.IsUsed &&
                t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task InvalidateTokenAsync(int tokenId)
    {
        var token = await _context.PasswordResetTokens.FindAsync(tokenId);
        if (token != null)
        {
            token.IsUsed = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task InvalidateAllUserTokensAsync(int userId)
    {
        var tokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ToListAsync();

        foreach (var token in tokens)
            token.IsUsed = true;

        await _context.SaveChangesAsync();
    }
}
