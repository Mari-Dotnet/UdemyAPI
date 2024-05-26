using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [AllowAnonymous]
    public class UsersController : BaseApiController
    {
        private readonly IUserIRepository _userrepository;
        private readonly IMapper _mapper;
        public UsersController(IUserIRepository UserRepository,IMapper mapper)
        {
            _mapper = mapper;
            _userrepository = UserRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            var users= await _userrepository.GetAllUserAsync();
            var usersToReturn=_mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(await _userrepository.GetMembersAsync());
        }

        [HttpGet("{Id}")] //api/user/1
        public async Task<ActionResult<MemberDto>> GetUser(int Id){
            var user =await _userrepository.GetUserByIdAsync(Id);
            var userToReturn=_mapper.Map<MemberDto>(user);
            return userToReturn;
        }

        [HttpGet("{UserName}")] //api/UserName/mari
        public async Task<ActionResult<MemberDto>> GetUserByName(string Name){
            var user =await _userrepository.GetUserByNameAsync(Name);
            var userToReturn=_mapper.Map<MemberDto>(user);
            return  Ok(await _userrepository.GetMemberAsync(Name)) ;
        }
    }
}   