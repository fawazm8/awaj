using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SellingProject.Models;
using SellingProject.Models.Dtos;
using SellingProject.Models.RepositoryInterface;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace SellingProject.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAuthRepository Repo { get; set; }
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            Repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            registerDto.UserName = registerDto.UserName.ToLower();
            if (await Repo.UserExist(registerDto.UserName)) return BadRequest("هذا المستخدم موجود من قبل ");
            var user = new User
            {
                UserName = registerDto.UserName
            };
            var createdUser = await Repo.Register(user, registerDto.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var userlogin = Repo.Login(loginDTO.UserName,loginDTO.Password);
            if(userlogin == null) return Unauthorized();
            var cliams = new[] {
                new Claim(ClaimTypes.NameIdentifier,userlogin.Id.ToString()),
                new Claim(ClaimTypes.Name,loginDTO.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512);
            var TokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(cliams),
                Expires= System.DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };
            var tokenHandeler = new JwtSecurityTokenHandler();
            var token = tokenHandeler.CreateToken(TokenDescriptor);
            return Ok(new{
                token = token
            });

        }
    }
}