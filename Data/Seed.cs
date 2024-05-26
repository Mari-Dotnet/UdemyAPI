using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext dataContext)
        {
            if(await dataContext.Users.AnyAsync()){
                return;
            }
            else{
                var userdata=await File.ReadAllTextAsync("Data/UserSeedData.json");
                var options=new JsonSerializerOptions();
                var userlist= JsonSerializer.Deserialize<List<AppUser>>(userdata, options);
                foreach(var user in userlist)
                {
                    using var hmac=new HMACSHA512();
                    user.UserName=user.UserName.ToLower();
                    user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$word"));
                    user.PasswordSalt=hmac.Key;
                    dataContext.Users.Add(user);
                }

                await dataContext.SaveChangesAsync();
            }

        }
    }
}