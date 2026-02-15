using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot find TokenKey in the Configuration");
        if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");

        // -----------------------------
        // 1️⃣ Create claims (payload data)
        // These values will go inside the JWT payload
        // -----------------------------
        var claims = new List<Claim>
        {
          // Store username as identity claim
          new (ClaimTypes.NameIdentifier,user.UserName)
        };

        // -----------------------------
        // 2️⃣ Create security key
        // Convert secret string → bytes (crypto uses bytes)
        // -----------------------------
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        
        // Define signing algorithm + key used to sign the token
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);


        // -----------------------------
        // 3️⃣ Describe how the token should look
        // -----------------------------
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // Identity data (payload)
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            // Signing information (creates header + signature)
            SigningCredentials = creds
        };

        // -----------------------------
        // 4️⃣ Create and serialize the JWT
        // -----------------------------
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor); // Build token object from descriptor

        // Convert token object → JWT string (HEADER.PAYLOAD.SIGNATURE)
        return tokenHandler.WriteToken(token);
    }
}
