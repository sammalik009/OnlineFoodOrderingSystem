using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using firstProject.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace firstProject.DAL
{
    public class RestaurantContext:DbContext
    {
        public RestaurantContext() : base("RestaurantContext")
        {
        }
        public DbSet<Orders> orders { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<CartItem> cart { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
 
    }
}