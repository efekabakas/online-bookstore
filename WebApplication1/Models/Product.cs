using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class Product
    {
        public Product()
        {
            ChartLines = new HashSet<ChartLine>();
            OrderLines = new HashSet<OrderLine>();
            WishListLines = new HashSet<WishListLine>();
        }

        public string Title { get; set; }
        public int? Pages { get; set; }
        public int ProductId { get; set; }
        public string Isbn { get; set; }
        public DateTime? Date { get; set; }
        public string ProductType { get; set; }
        public short Stock { get; set; }
        public decimal Price { get; set; }
        public string Language { get; set; }
        public string Genre { get; set; }
        public byte Rating { get; set; }
        public int PublisherId { get; set; }
        public int AuthorId { get; set; }
        public int SupplierId { get; set; }

        public int Count { get; set; }
        public virtual Author Author { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ChartLine> ChartLines { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<WishListLine> WishListLines { get; set; }
    }
}
