using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using models;

namespace JwtService;

public class JwtServices
{
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
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SECRET_KEY_TESTSECRET_KEY_TESTSECRET_KEY_TESTSECRET_KEY_TESTSECRET_KEY_TEST")),
        SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}