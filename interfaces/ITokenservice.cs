using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.interfaces
{
    public interface ITokenservice
    {
        string CreateToken(AppUser user);
    }
}