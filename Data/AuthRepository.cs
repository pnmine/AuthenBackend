namespace AuthenBackend.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username.ToLower().Equals(username.ToLower()));
            //user ที่มี username ตรงกับ database
            if (user == null) //ไม่พบ userใน database
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) //หาก password ไม่ตรงกับใน database
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else //if success
            {
                response.Success = true;
                response.Message = "Login successfully :)";
                // response.Data = user.Id.ToString();
                response.Data = CreateToken(user); //สร้าง token เก็บ user
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var response = new ServiceResponse<int>();
            if (await UserExists(user.Username)) // เช็คว่า user มีอยู่แล้วหรือยัง
            {
                response.Success = false;
                response.Message = "User already exists.!";
                return response;
            }
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Data = user.Id; //return userid
            response.Success = true;
            response.Message = "Create user successfully :)";
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(user => user.Username.ToLower() == username.ToLower())) //check if user exists username เดียวกัน
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var appSettingsToken = _configuration.GetSection("AppSettings:Token")?.Value; //get token from appsettings.json with null check
            if (appSettingsToken is null)
            {
                throw new Exception("Token not found in appsettings.json!");
            }
            if (appSettingsToken.Length < 32)
            {
                throw new Exception($"Token in appsettings.json is too short! It must be at least 32 characters long.now {appSettingsToken.Length} characters");
            }
            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken)); //create key
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //create credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), //expire 1 day ต้องlogin ใหม่ ทุกวันเพื่อให้ได้ tokenใหม่
                SigningCredentials = creds
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor); //create token
            return tokenHandler.WriteToken(token); //write token in to Jwt
        }
    }
}