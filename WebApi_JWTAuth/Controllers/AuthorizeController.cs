using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_JWTAuth.Data;
using WebApi_JWTAuth.Models;
using WebApi_JWTAuth.Service;

namespace WebApi_JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public AuthorizeController(ApplicationDbContext context, IConfiguration configuration, TokenService tokenService)
        {
            _configuration = configuration;
            _context = context;
            _tokenService = tokenService;
        }

        //method for registering user
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("user registerd");
        }

        //for login the user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == login.Username && user.Password == login.Password);
            if(user == null) return Unauthorized();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            return Ok(new { accessToken, refreshToken });
        }

        //let user refresh their jwt access token, if the original one is expired
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenModel token)
        {
            var refreshToken = _tokenService.GetRefreshToken(token.RefreshToken);
            if(refreshToken == null) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);
            if(user == null) return Unauthorized();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

            //revoke the old refresh token
            _tokenService.RevokeRefreshToken(refreshToken.RefreshUserToken);

            return Ok(new {accessToken = newAccessToken, refreshToken = newRefreshToken});

        }

    }
}
