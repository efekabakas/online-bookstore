using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class OrderTable
    {
        public OrderTable()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int TotalPrice { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
