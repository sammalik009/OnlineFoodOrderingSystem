using firstProject.DAL;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Text.RegularExpressions;
using firstProject.Models;

namespace firstProject.Controllers
{
    public class IndexController : Controller
    {
        private RestaurantContext _context;
        public IndexController()
        {
            _context = new RestaurantContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Index
        public ActionResult Index()
        {
            if (Request.Cookies["userName"] != null)
            {
                String str = (String)Request.Cookies["userName"].Value;
                if (str.Equals("")) return View();
                if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
                if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
                if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            }
            return View();
        }
        public ActionResult Signup(Users user) 
        {
            user.Key = "Customer";
            Response.Cookies["userName"].Value = user.Name;
            Response.Cookies["key"].Value = user.Key;
            Response.Cookies["credit"].Value = Convert.ToString(10000);
            user.CreditCardAmount = 10000;
            _context.users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("CustomerView", "Home");
        }
        public ActionResult Login()
        {
            if (Request.Cookies["userName"] != null)
            {
                String str = (String)Request.Cookies["userName"].Value;
                if (str.Equals("")) return View();
                if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
                if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
                if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            }
            return View();
        }
        public ActionResult LoginFunction(Users user)
        {
            var users = _context.users.ToList();
            var q = from x in users
                    where x.Name == user.Name &&
                    x.Password == user.Password
                    select x;
            if (q != null)
            {
                foreach (var u in q)
                {
                    String str1 = u.Key;
                    Response.Cookies["userName"].Value = user.Name;
                    Response.Cookies["key"].Value = str1;
                    Response.Cookies["credit"].Value = Convert.ToString(user.CreditCardAmount);
                    if (str1.Equals("Customer")) return RedirectToAction("CustomerView", "Home");
                    else if (str1.Equals("Admin")) return RedirectToAction("AdminView", "Home");
                    else
                    {
                        var v=_context.Restaurants.ToList();
                        var v1=from y in v
                               where y.Name==u.Name
                               select y;
                        foreach(var w in v1){
                            Response.Cookies["restaurantName"].Value=w.RestaurantTitle;
                            return RedirectToAction("RestaurantView", "Home");
                        }
                    }
                }
            }
            return View("Login");
        }
        public JsonResult CheckUserName(String userName)
        {
            System.Threading.Thread.Sleep(200);
            userName = userName.ToLower();
            var users = _context.users.ToList();
            foreach (var v in users)
            {
                if (v.Name.Equals(userName)) return Json(1);
            }
            return Json(0);
        }
        public JsonResult CheckEmail(String email)
        {
            System.Threading.Thread.Sleep(200);
            email=email.ToLower();
            String[] str = email.Split('@');
            if(!(str[1].Equals("gmail.com"))&&!(str[1].Equals("ymail.com"))&&!(str[1].Equals("hotmail.com"))&&!(str[1].Equals("outlook.com"))&&!(str[1].Equals("pucit.edu.pk"))&&!(str[1].Equals("yahoo.com")))
            {
                return Json(1);
            }
            else
            {
                var users = _context.users.ToList();
                foreach (var v in users)
                {
                    if (v.Email.Equals(email)) return Json(1);
                }
                return Json(0);
            }
        }
        public JsonResult CheckNumber(String number)
        {
            System.Threading.Thread.Sleep(200);
            int count = 0;
            for (int i = 0; i < number.Length; i++)
            {
                if (number[i] >= '0' && number[i] <= '9') count++;
            }
            if (count < 11) return Json(1);
            var users = _context.users.ToList();
            foreach (var v in users)
            {
                if (v.Number.Equals(number)) return Json(1);
            }
            return Json(0);
        }
        public JsonResult CheckCredit(String credit)
        {
            System.Threading.Thread.Sleep(200);
            int count = 0;
            for(int i=0;i<credit.Length;i++){
                if (credit[i] >= '0' && credit[i] <= '9') count++;
            }
            if (count < 10) return Json(1);
            var users = _context.users.ToList();
            foreach (var v in users)
            {
                if (v.CreditCardId.Equals(credit)) return Json(1);
            }
            return Json(0);
        }
    }
}