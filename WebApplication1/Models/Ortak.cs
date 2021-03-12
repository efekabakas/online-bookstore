using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Ortak
    {

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action:"IsEmailInUse", controller:"Login")]
        public string Email { get; set; }
        public string CreditCard { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool? Type { get; set; }

    }
}
