using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using API.Data;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extension
{
    public static class ApplicationService
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection Services,
                                                                     IConfiguration config)
        {
        
        Services.AddDbContext<DataContext>(options=>
        {
            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen();
            Services.AddCors();
            Services.AddScoped<ITokenservice,TokenService>();

            return Services;
        }

        
    }
}