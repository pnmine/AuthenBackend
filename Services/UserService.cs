using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace AuthenBackend.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(DataContext dataContext,IMapper mapper,IHttpContextAccessor httpContextAccessor)
        {
            _context = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<ServiceResponse<GetUserDto>> GetAllUsers()
        {
            var response = new ServiceResponse<GetUserDto>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            if (user != null)
            {
                var userDto = new GetUserDto
                {
                    Username = user.Username,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    Phone = user.Phone
                };
                response.Data = userDto;
                response.Message = "Get User success;)"; 
                response.Success = true;
                return response;
            }
            response.Message = "Get User fail;)"; 
            response.Success = false;
            return response;

        }
    }
}