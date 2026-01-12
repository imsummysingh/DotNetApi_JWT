namespace WebApi_JWTAuth.Models
{

    //product which can be accessed only after login

    public class Product
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }

    }
}
