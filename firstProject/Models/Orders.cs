using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace firstProject.Models
{
    public class Orders
    {
        public int Id{get;set;}
        [Required]
        public DateTime TimeToPlaceOrder { set; get; }
        [Required]
        public DateTime TimeToFullfillOrder { set; get; }
        [Required]
        public String status { set; get; }
        [Required]
        public String userName { set; get; }
        [Required]
        public int payment { set; get; }
        [Required]
        public int balance { get; set; }
        [Required]
        public String itemName { get; set; }
        [Required]
        public int amount { get; set; }
        public virtual Users user { get; set; }
        [Required]
        public String customerAddress { get; set; }
        [Required]
        public String restaurantName { get; set; }
    }
}