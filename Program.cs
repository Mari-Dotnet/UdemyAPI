using API.Data;
using API.Extension;
using API.Helpers;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddJwtExtension(builder.Configuration);
var app = builder.Build();

//configure http request pipeline 
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors(builder=>builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));
app.UseCors(builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// using var scope =app.Services.CreateScope();
// var services=scope.ServiceProvider;
// try{
// var context=services.GetRequiredService<DataContext>();
// await context.Database.MigrateAsync();
// await Seed.SeedUsers(context);
// }catch(Exception ex){
//     var logger=services.GetService<ILogger<Program>>();
//     logger.LogError(ex, "An error occured on during  migrations");

// }   

app.Run();

