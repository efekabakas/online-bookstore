using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication1.Models
{
    public partial class Customer
    {
        public Customer()
        {
            OrderTables = new HashSet<OrderTable>();
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CreditCard { get; set; }

        public virtual Chart Chart { get; set; }
        public virtual WishList WishList { get; set; }
        public virtual ICollection<OrderTable> OrderTables { get; set; }
    }
}
