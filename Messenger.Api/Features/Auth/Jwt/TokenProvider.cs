using Messenger.Api.Context;
using Messenger.Domain.Aggregates.UserAggregate;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Api.Features.Auth.Jwt;

public class TokenProvider(MessengerContext context, IOptions<JwtOptions> options)
{
    public (string AccessToken, string RefreshToken) GenerateTokens(User user)
    {
        var accessToken = GenerateAccessToken(user.Id, user.Role.ToString());
        var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = Convert.ToHexString(          // ← в БД только хэш
                SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken))),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
        };

        context.RefreshTokens.Add(refreshToken);
        return (accessToken, rawRefreshToken);
    }

    private string GenerateAccessToken(Guid userId, string role)
    {
        var jwt = options.Value;

        var expiresAt = DateTime.UtcNow.AddMinutes(jwt.ExpiresMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new Claim(ClaimTypes.Role, role),
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return accessToken;
    }

    public async Task<(string AccessToken, string RefreshToken)> RotateAsync(string rawRefreshToken, CancellationToken ct)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken)));

        var stored = await context.RefreshTokens.SingleAsync(t => t.TokenHash == hash, ct)
            ?? throw new UnauthorizedAccessException("Неизвестный refresh token");

        if (!stored.IsActive)
        {
            // reuse detection: предъявили УЖЕ использованный токен = признак кражи
            await RevokeAllForUserAsync(stored.UserId, ct);
            throw new UnauthorizedAccessException("Refresh token reuse detected");
        }

        var user = await context.Users.FindAsync([stored.UserId], ct)
            ?? throw new UnauthorizedAccessException("Пользователь не найден");

        if (user.Status == UserStatus.Banned)              // бан применяется и к сессиям
            throw new UnauthorizedAccessException("Пользователь в бане");

        stored.Revoke();

        var (access, rawNew) = GenerateTokens(user);       // новый refresh со ReplacedById = stored.Id
        return (access, rawNew);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct)
    {
        var active = await context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var t in active) t.Revoke();
    }
}