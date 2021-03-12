using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class WishList
    {
        public WishList()
        {
            WishListLines = new HashSet<WishListLine>();
        }

        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<WishListLine> WishListLines { get; set; }
    }
}
