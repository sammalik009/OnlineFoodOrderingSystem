using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace firstProject.Models
{
    public class Image
    {
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public String RestaurantName{get;set;}
        public String Type { get; set; }
        [DisplayName("Upload File")]
        public String ImagePath { get; set; }
        public int price { get; set; }
        public String Description { get; set; }
        public virtual Restaurant restaurant { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}