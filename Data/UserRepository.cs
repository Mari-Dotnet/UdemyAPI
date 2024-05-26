using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserIRepository
    {

        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public UserRepository(DataContext dbcontext, IMapper mapper)
        {
            _dataContext=dbcontext;
            _mapper=mapper;
        }
        public async Task<IEnumerable<AppUser>> GetAllUserAsync()
        {
            // in below GetMembersAsync method also retrun same result without using include 
            // it will taken care by mapper
            return await _dataContext.Users
                    .Include(x => x.Photos)
                    .ToListAsync();
        }

        public  async Task<MemberDto> GetMemberAsync(string UserName)
        {
            return await _dataContext.Users.
                            Where(x => x.UserName.Equals(UserName)).
                            ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
                
                

        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _dataContext.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int Id)
        {
            return await _dataContext.Users.FindAsync(Id);
        }

        public async Task<AppUser> GetUserByNameAsync(string UserName)
        {
            return await _dataContext.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x=>x.UserName.Equals(UserName,StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync()>0;
        }

        public async void Update(AppUser user)
        {
            _dataContext.Entry(user).State = EntityState.Modified;
        }
    }
}