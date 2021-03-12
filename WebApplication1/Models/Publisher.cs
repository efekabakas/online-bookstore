using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class Publisher
    {
        public Publisher()
        {
            Products = new HashSet<Product>();
        }

        public string PublisherName { get; set; }
        public int PublisherId { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
