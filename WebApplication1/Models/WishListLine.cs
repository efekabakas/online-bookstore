using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class WishListLine
    {
        public int WistListId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual WishList WistList { get; set; }
    }
}
