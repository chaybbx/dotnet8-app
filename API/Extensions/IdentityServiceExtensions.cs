using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection service, IConfiguration config)
    {

        //Authorization: Bearer TOKEN
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var tokenKey = config["P"] ?? throw new Exception("TokenKey not found");

                // Define rules for validating incoming JWT tokens
                options.TokenValidationParameters = new TokenValidationParameters
                {
                     // Check that the token signature is valid (prevents fake or modified tokens)
                    ValidateIssuerSigningKey = true,

                    // The secret key used to verify the signature, Convert string → bytes because crypto works with bytes
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

            });

        return service;
    }
}
