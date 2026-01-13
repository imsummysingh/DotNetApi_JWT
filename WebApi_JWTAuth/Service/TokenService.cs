//to generate jwt token and refresh token inside this class
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi_JWTAuth.Data;
using WebApi_JWTAuth.Models;

namespace WebApi_JWTAuth.Service
{
    public class TokenService
    {
        //Instialized the DI containers instances

        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public TokenService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        //generating access token method-> will generate jwt access token for our authenticated user
        public string GenerateAccessToken(User user)
        {
            //1. we define the claims : these values will be encoded into the token

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            //2. we get the secret key from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]));

            //3. creating signing credentials using HmacSha256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //4. we define when the token will expire
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings: AccessTokenExpiryMinutes"]));

            //5. we then build the jwt token using issuer, audience, claim, expires & signing creds
            var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings: Issuer"],
                    audience: _configuration["JwtSettings: Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

            //6. finally, we return the encoded token string
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        //Method for refresh token
        public string GenerateRefreshToken(int userId)
        {
            //1. we create a secure random string to use as a refresh token, and assign it a unique token id
            var tokenId = Guid.NewGuid().ToString();
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


            //2. setrting the expiry for the refresh token
            var expiryDays = Convert.ToInt32(_configuration["JwtSettings: RefreshTokenExpiryDays"]);
            var expiryDate = DateTime.UtcNow.AddDays(expiryDays);

            //3. we then store the refresh token with userId & tokenId
            var token = new RefreshToken
            {
                UserId = userId,
                TokenId = tokenId,
                RefreshUserToken = refreshToken,
            };

            //4. finally we save it in the database and return the refresh token string
            _context.RefreshTokens.Add(token);
            _context.SaveChanges();

            return refreshToken;
        }

        //get refresh token method
        public RefreshToken GetRefreshToken(string refreshToken)
        {
            return _context.RefreshTokens.FirstOrDefault(RefreshToken => RefreshToken.RefreshUserToken == refreshToken);
        }

        //method to revoke the refresh token - invalidate the refresh token
        public void RevokeRefreshToken(string refreshToken)
        {
            var token = _context.RefreshTokens.FirstOrDefault(RefreshToken => RefreshToken.RefreshUserToken == refreshToken);
            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
                _context.SaveChanges();
            }
        }

    }
}
