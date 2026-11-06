using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using models;

namespace JwtService;

public class JwtServices
{
    string _key;
    public JwtServices(IConfiguration configuration)
    {
        _key = configuration.GetValue<string>("JwtSecretKey") ?? throw new InvalidOperationException("Не задано JwtSecretKey");
    }
    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("Id", Convert.ToString(user.Id)),
            new Claim("Username", user.Username!),
            new Claim("Email", user.Email!),
        };

        var token = new JwtSecurityToken(
        expires: DateTime.UtcNow.AddHours(1),
        claims: claims,
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}