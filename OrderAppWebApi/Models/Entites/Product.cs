﻿namespace OrderAppWebApi.Models.Entites
{
    public class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Unit{ get; set; }
        public decimal UnitPrice{ get; set; }
        public bool Status{ get; set; }
        public DateTime CreatedDate{ get; set; }
        public DateTime? UpdatedDate{ get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
