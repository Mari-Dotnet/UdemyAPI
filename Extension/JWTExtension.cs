using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extension
{
    public static class JWTExtension
    {
        public static IServiceCollection AddJwtExtension(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                    AddJwtBearer(options=>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuerSigningKey=true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.
                                    UTF8.GetBytes(configuration["TokenKey"])),
                        ValidateIssuer=false,
                        ValidateAudience=false,
                        }; 
                    });
            return Services;
        }
        
    }
}