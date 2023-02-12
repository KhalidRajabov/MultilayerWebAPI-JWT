namespace AuthServer.Core.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Decimal Price { get; set; }
        public string? UserId { get; set; }
    }
}
