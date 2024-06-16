using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [EnableCors("AllowAll")]
    public class UsersController : BaseApiController
    {
        private readonly IUserIRepository _userrepository;
        private readonly IMapper _mapper;
        public UsersController(IUserIRepository UserRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userrepository = UserRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() {
            var users = await _userrepository.GetAllUserAsync();
            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(await _userrepository.GetMembersAsync());
        }

        [HttpGet] //api/user/1
        [Route("getbyId/{Id}")]
        public async Task<ActionResult<MemberDto>> GetUser(int Id) {
            var user = await _userrepository.GetUserByIdAsync(Id);
            var userToReturn = _mapper.Map<MemberDto>(user);
            return userToReturn;
        }

        [HttpGet] //api/UserName/mari
        [Route("GetUser/{UserName}")]
        public async Task<ActionResult<MemberDto>> GetUserByName(string UserName) {
            var user = await _userrepository.GetUserByNameAsync(UserName);
            var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(await _userrepository.GetMemberAsync(UserName));
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userrepository.GetUserByNameAsync(userName);
            if(user==null){ return NotFound(); }
            _mapper.Map(memberUpdateDto,user);
            if (await _userrepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failled to update user");
        }

    }
}   