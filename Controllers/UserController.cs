namespace AuthenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> GetUser()
        {
            var response = await _userService.GetAllUsers();
            if (response.Success == true)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}