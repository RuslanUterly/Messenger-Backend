using Messenger.Api.Context;
using Messenger.Domain.Aggregates.UserAggregate;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Api.Features.Auth.Tokens;

public class TokenProvider(MessengerContext context, IOptions<JwtOptions> options)
{
    public (string AccessToken, string RefreshToken) GenerateTokens(User user)
    {
        var accessToken = GenerateAccessToken(user.Id, user.Role.ToString());
        var rawRefreshToken = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(48));
        
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

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct)
    {
        var active = await context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var t in active) t.Revoke();
    }
}