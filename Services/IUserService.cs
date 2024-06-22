using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenBackend.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<GetUserDto>> GetAllUsers();
    }
}