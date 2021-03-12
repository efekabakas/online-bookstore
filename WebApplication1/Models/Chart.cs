using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class Chart
    {
        public Chart()
        {
            ChartLines = new HashSet<ChartLine>();
        }

        public int CustomerId { get; set; }
        public int? Cost { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<ChartLine> ChartLines { get; set; }
    }
}
