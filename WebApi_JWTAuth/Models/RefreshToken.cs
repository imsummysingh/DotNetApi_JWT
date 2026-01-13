namespace WebApi_JWTAuth.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }      //foreign key to user table
        public string TokenId { get; set; }
        public string RefreshUserToken { get; set; }
        public User User { get; set; }  //navigation properties to user //many to many
    }
}
