using System.ComponentModel.DataAnnotations;

namespace Messenger.Api.Features.Auth.Jwt;

public class JwtOptions
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }

    [Range(5, 60)]
    public int ExpiresHours { get; set; }
}
