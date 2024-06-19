namespace AuthenBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository) //inject AuthRepository
        {
            _authRepository = authRepository;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
        {
            var response = await _authRepository.Register(
                new User { 
                    Username = request.Username , 
                    Firstname = request.Username,
                    Lastname = request.Lastname,
                    Email = request.Email,
                    Phone = request.Phone
                }
                , request.Password //password paintext ที่ส่งไปให้ Register service
            );
            if (response.Success == false) //if false
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request)
        {
            var response = await _authRepository.Login(request.Username, request.Password);
            if (response.Success == false) //if false
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}