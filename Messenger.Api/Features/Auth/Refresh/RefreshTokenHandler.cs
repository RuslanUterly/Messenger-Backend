using Messenger.Api.Context;
using Messenger.Api.Features.Auth.Tokens;
using Messenger.Domain.Aggregates.UserAggregate;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Api.Features.Auth.Refresh;

public static class RefreshTokenHandler
{
    public static async Task<IssuedTokens> Handle(
        MessengerContext context, 
        TokenProvider tokens,
        RotateTokensCommand command, 
        CancellationToken ct)
    {
        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(command.RefreshToken)));

        var stored = await context.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == hash, ct)
            ?? throw new UnauthorizedAccessException("Неизвестный refresh token");

        if (!stored.IsActive)
        {
            var compromised = await context.RefreshTokens
                .Where(t => t.UserId == stored.UserId && t.RevokedAt == null)
                .ToListAsync(ct);
            foreach (var t in compromised) t.Revoke();
            throw new UnauthorizedAccessException("Попытка подмены Refresh token");
        }

        var user = await context.Users.FindAsync([stored.UserId], ct)
            ?? throw new UnauthorizedAccessException("Пользователь не найден");

        if (user.Status == UserStatus.Banned)
            throw new UnauthorizedAccessException("Пользователь заблокирован");

        stored.Revoke();

        var (accessToken, rawRefreshToken) = tokens.GenerateTokens(user);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = Convert.ToHexString(          // ← в БД только хэш
                SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken))),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
        };

        context.RefreshTokens.Add(refreshToken);
        return new IssuedTokens(accessToken, rawRefreshToken);
    }

}

public record RotateTokensCommand(string RefreshToken);
public record IssuedTokens(string AccessToken, string RefreshToken);