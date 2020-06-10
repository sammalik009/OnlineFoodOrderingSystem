using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace firstProject.DAL
{
    public class RestaurantInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<RestaurantContext>
    {
        protected override void Seed(RestaurantContext context)
        { 
        }
    }
}