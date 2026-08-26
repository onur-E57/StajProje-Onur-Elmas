namespace StajProje.WebUI.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
