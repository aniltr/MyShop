namespace MyShop.Core.Models
{
    public class CartItem : BaseEntity
    {
        public string CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}