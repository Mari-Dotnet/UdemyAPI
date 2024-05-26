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
using AutoMapper;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenservice _tokenservice;
        private readonly IMapper _mapper;


        public AccountController(DataContext context, ITokenservice tokenservice,
                                IMapper mapper)
        {
            _context = context;
            _tokenservice=tokenservice;
            _mapper = mapper;
        }

        [HttpPost("registercheck")] //Post: api/account/register
        public async Task<ActionResult<UserDto>> UserRegister(UserRegisterDTO model)
        {
            try
            {
                Random rnd = new Random();
                if (await IsUserExist(model.UserName)) return BadRequest("User already exist");
                AppUser RegisterModel = _mapper.Map<AppUser>(model);
                using var hmac = new HMACSHA512();
                RegisterModel.UserName = model.UserName;
                RegisterModel.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                RegisterModel.PasswordSalt = hmac.Key;
                RegisterModel.Photos = new List<Photo>()
            {
                new Photo(){Url=model.Url,IsMain=true,PublicId= Convert.ToString(rnd.Next(1,1000))},
            };
                _context.Users.Add(RegisterModel);
                await _context.SaveChangesAsync();
                return new UserDto()
                {
                    UserName = model.UserName,
                    Token = _tokenservice.CreateToken(RegisterModel)
                };
            }
            catch(Exception ex)
            {
                throw;
            }
            
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