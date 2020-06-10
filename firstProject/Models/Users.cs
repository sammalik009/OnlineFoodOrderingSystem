using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace firstProject.Models
{
    public class Users
    {
        public int Id { get; set; }
        [DisplayName("Please Enter Your Name")]
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        public String Name { get; set; }
        [DisplayName("Please Enter Your Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public String Password { get; set; }
        [DisplayName("Please Enter Your E-mail")]
        [Required]
        [MinLength(10)]
        [MaxLength(30)]
        public String Email { get; set; }
        [DisplayName("Please Enter Your Address")]
        [Required]
        [MinLength(20)]
        [MaxLength(50)]
        public String Address { get; set; }
        [DisplayName("Please Enter Your Credit Card Number")]
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        public String CreditCardId { get; set; }
        public long CreditCardAmount{get;set;}
        [Required]
        [MinLength(11)]
        [MaxLength(11)]
        public String Number { get; set; }
        [Required]
        public String Key { get; set; }
        public virtual ICollection<Orders> orders { set; get; }
    }
}