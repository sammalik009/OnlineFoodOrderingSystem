using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace firstProject.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        [Required]
        public String userName { set; get; }
        [Required]
        public int payment { set; get; }
        [Required]
        public String itemName { get; set; }
        [Required]
        public int amount { get; set; }
        [Required]
        public String customerAddress { get; set; }
        [Required]
        public String restaurantName { get; set; }
    }
}