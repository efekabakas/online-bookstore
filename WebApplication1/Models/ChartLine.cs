using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class ChartLine
    {
        public int ChartId { get; set; }
        public short Quantity { get; set; }
        public int ProductId { get; set; }

        public virtual Chart Chart { get; set; }
        public virtual Product Product { get; set; }
    }
}
