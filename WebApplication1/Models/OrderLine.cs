using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class OrderLine
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual OrderTable Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
