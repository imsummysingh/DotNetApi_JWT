//we use this class object to send data from client to server, to get refresh token and then send response from generateRefreshToken()
namespace WebApi_JWTAuth.Models
{
    public class TokenModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }    
    }
}
