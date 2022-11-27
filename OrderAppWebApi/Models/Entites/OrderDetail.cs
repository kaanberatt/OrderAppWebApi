namespace OrderAppWebApi.Models.Entites
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
