using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace firstProject.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        [Required]
        public String RestaurantTitle { get; set; }
        [Required]
        public String Location { get; set; }
        [Required]
        public String Name { get; set; }
        public virtual ICollection<Image> images {get;set;}
    }
}