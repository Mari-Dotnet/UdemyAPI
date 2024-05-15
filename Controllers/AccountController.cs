using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.interfaces;
using SQLitePCL;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
         private readonly ITokenservice _tokenservice;

         public AccountController(DataContext context, ITokenservice tokenservice)
        {
            _context = context;
            _tokenservice=tokenservice;
        }

        [HttpPost("register")] //Post: api/account/register
        public async Task<ActionResult<UserDto>> Register([FromBody]RegisterDto modeldto)
        {
           if(await IsUserExist(modeldto.UserName)) return BadRequest("User already exist");
            AppUser model=new AppUser();
            using var hmac=new HMACSHA512();  
            model.UserName=modeldto.UserName;
            model.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(modeldto.Password));
            model.PasswordSalt=hmac.Key;
            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            return new UserDto(){
                UserName=modeldto.UserName,
                Token=_tokenservice.CreateToken(model)
            };       
            
        } 

        private async Task<bool> IsUserExist(string Username)
        {
            return  await _context.Users.AnyAsync(x=>x.UserName.ToLower().Equals(Username.ToLower()));
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto){
            var userdetail= await _context.Users.FirstOrDefaultAsync(x=>x.UserName.ToLower().Equals(logindto.UserName.ToLower()));
            if(userdetail==null){
                return Unauthorized("Invalid username");
            }
            
            using var hmac =new HMACSHA512(userdetail.PasswordSalt);
            var computerhas=hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password));
            for (int i=0;i<computerhas.Length;i++)
            {
                if(computerhas[i] !=userdetail.PasswordHash[i]){
                    return Unauthorized("invalid pasword");
                }
            
            }
            string token= _tokenservice.CreateToken(new AppUser(){UserName=userdetail.UserName});
            return new UserDto(){
                UserName=userdetail.UserName,
                Token=token,
            };
        }
    }
}