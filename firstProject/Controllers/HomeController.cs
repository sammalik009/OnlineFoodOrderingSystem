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
    public class HomeController : Controller
    {
        private RestaurantContext _context;
        public HomeController()
        {
            _context = new RestaurantContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public JsonResult PlaceOrder(int id, int amount,int key)
        {
            var i = _context.Images.ToList();
            Orders order=new Orders();
            order.userName=(String)Request.Cookies["userName"].Value;
            var l = _context.users.ToList();
            order.amount = amount;
            order.status = "Pending";
            order.TimeToPlaceOrder = DateTime.Now;
            order.TimeToFullfillOrder = DateTime.Now.AddDays(1);
            int price = 0;
            long credit = 0;
            var u = _context.users.ToList();
            foreach (var m in i)
            {
                if (m.Id == id)
                {
                    order.itemName = m.Title;
                    price = m.price;
                    order.payment = price * amount;
                    order.restaurantName=m.RestaurantName;
                    if (key == 0)
                    {
                        order.balance = 0;
                        foreach (var k in l)
                        {
                            if (order.userName.Equals(k.Name))
                            {
                                credit = k.CreditCardAmount;
                                if (order.payment <= credit)
                                {
                                    k.CreditCardAmount -= order.payment;
                                    order.balance = 0;
                                    break;
                                }
                                else
                                {
                                    return Json(1);
                                }
                            }
                        } 
                    }
                    else if (key == 1)
                    {
                        order.balance = order.payment;
                    }
                    foreach (var n in u)
                    {
                        if (n.Name == order.userName)
                        {
                            order.customerAddress = n.Address;
                        }
                    }
                    _context.orders.Add(order);
                    _context.SaveChanges();
                    return Json(0);
                }
            }
            return Json(1);
        }
        public JsonResult RemoveFromCart(int id)
        {
            var ci = _context.cart.ToList();
            foreach (var c in ci)
            {
                if (c.Id == id)
                {
                    _context.cart.Remove(c);
                    _context.SaveChanges();
                    return Json(0);
                }
            }
            return Json(1);
        }
        public JsonResult AddToCart(int id, int amount)
        {
            var i = _context.Images.ToList();
            CartItem c = new CartItem();
            c.userName = (String)Request.Cookies["userName"].Value;
            var l = _context.users.ToList();
            c.amount = amount;
            int price = 0;
            var u = _context.users.ToList();
            foreach (var m in i)
            {
                if (m.Id == id)
                {
                    c.itemName = m.Title;
                    price = m.price;
                    c.restaurantName = m.RestaurantName;
                    var ci = _context.cart.ToList();
                    foreach (var a in ci)
                    {
                        if (a.restaurantName.Equals(c.restaurantName) && a.userName.Equals(c.userName) && a.itemName.Equals(c.itemName))
                        {
                            a.amount = a.amount + amount;
                            a.payment = a.amount * price;
                            _context.SaveChanges();
                            return Json(0);
                        }
                    }
                    c.payment = price * amount;
                    foreach (var n in u)
                    {
                        if (n.Name == c.userName)
                        {
                            c.customerAddress = n.Address;
                        }
                    }
                    _context.cart.Add(c);
                    _context.SaveChanges();
                    return Json(0);
                }
            }
            return Json(1);
        }
        public ActionResult AdminInformation()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            var r = _context.users.ToList();
            foreach (var i in r)
            {
                if (i.Name.Equals(str))
                {
                    ViewBag.u = i;
                    break;
                }
            }
            return View();
        }
        public ActionResult AboutCustomer()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            var r = _context.users.ToList();
            foreach (var i in r)
            {
                if (i.Name.Equals(str))
                {
                    ViewBag.u = i;
                    break;
                }
            }
            return View();
        }
        public JsonResult PlaceCartOrder(int key)
        {
            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Now.AddDays(1);
            String name = (String)Request.Cookies["userName"].Value;
            var ci = _context.cart.ToList();
            String method="Credit";
            if(key==0)method="Cash";
            int balance = 0;
            long payment=0;
            foreach (var c in ci)
            {
                if (c.userName.Equals(name))
                {
                    if(method.Equals("Cash"))balance=c.payment;
                    else
                    {
                        payment+=c.payment;
                        var u = _context.users.ToList();
                        foreach (var us in u)
                        {
                            if (us.Name.Equals(name))
                            {
                                if (us.CreditCardAmount < payment)
                                {
                                    return Json(1);
                                }
                            }
                        }
                        balance = 0;
                    }
                    _context.orders.Add(new Orders
                    {
                        userName=c.userName,
                        TimeToFullfillOrder=t2,
                        TimeToPlaceOrder=t1,
                        payment=c.payment,
                        customerAddress=c.customerAddress,
                        restaurantName=c.restaurantName,
                        balance=balance,
                        itemName=c.itemName,
                        amount=c.amount,
                        status="Pending"
                    });
                    _context.cart.Remove(c);
                }
            }
            _context.SaveChanges();
            return Json(0);
        }
        public ActionResult ShowCart()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            List<CartItem> cart = new List<CartItem>();
            var ci = _context.cart.ToList();
            String name = (String)Request.Cookies["userName"].Value;
            long bill = 0;
            int items = 0;
            foreach (var c in ci)
            {
                if (c.userName.Equals(name))
                {
                    items++;
                    bill += c.payment;
                    cart.Add(c);
                }
            }
            long credit = 0;
            var u = _context.users.ToList();
            foreach (var us in u)
            {
                if (us.Name.Equals(str)) credit = us.CreditCardAmount;
            }
            ViewBag.credit = credit;
            ViewBag.items = items;
            ViewBag.bill = bill;
            ViewBag.list = cart;
            return View();
        }
        public ActionResult ShowCart2()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            List<CartItem> cart = new List<CartItem>();
            var ci = _context.cart.ToList();
            String name = (String)Request.Cookies["userName"].Value;
            long bill = 0;
            int items = 0;
            foreach (var c in ci)
            {
                if (c.userName.Equals(name))
                {
                    items++;
                    bill += c.payment;
                    cart.Add(c);
                }
            }
            long credit = 0;
            var u = _context.users.ToList();
            foreach (var us in u)
            {
                if (us.Name.Equals(str)) credit = us.CreditCardAmount;
            }
            ViewBag.credit = credit;
            ViewBag.items = items;
            ViewBag.bill = bill;
            ViewBag.list = cart;
            return View();
        }
        // GET: Home
        public ActionResult ShowCart3()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            List<CartItem> cart = new List<CartItem>();
            var ci = _context.cart.ToList();
            String name = (String)Request.Cookies["userName"].Value;
            long bill = 0;
            int items = 0;
            foreach (var c in ci)
            {
                if (c.userName.Equals(name))
                {
                    items++;
                    bill += c.payment;
                    cart.Add(c);
                }
            }
            long credit = 0;
            var u = _context.users.ToList();
            foreach (var us in u)
            {
                if (us.Name.Equals(str)) credit = us.CreditCardAmount;
            }
            ViewBag.credit = credit;
            ViewBag.items = items;
            ViewBag.bill = bill;
            ViewBag.list = cart;
            return View();
        }
        public ActionResult AboutRestaurant()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            var r = _context.users.ToList();
            foreach (var i in r)
            {
                if (i.Name.Equals(str))
                {
                    ViewBag.u = i;
                    break;
                }
            }
            var q = _context.Restaurants.ToList();
            foreach (var j in q)
            {
                if (j.Name.Equals(str))
                {
                    ViewBag.r = j;
                    break;
                }
            }
            return View();
        }
        public ActionResult AddRestaurant()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals(""))    return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value))    return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value))   return RedirectToAction("RestaurantView", "Home");
            return View();
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
            var data = _context.users.ToList();
            foreach (var r in data)
            {
                if (r.Number.Equals(number))
                {
                    return Json(1);
                }
            }
            return Json(0);
        }
        public JsonResult RemoveMenu(int id)
        {
            var q = _context.Images.ToList();
            foreach (var i in q)
            {
                if (i.Id == id)
                {
                    _context.Images.Remove(i);
                    _context.SaveChanges();
                    return Json(0);
                }
            }
            return Json(1);
        }
        public ActionResult CreateRestaurant(Users user,Restaurant restaurant)
        {
            user.Key = "Restaurant";
            user.CreditCardAmount = 10000;
            _context.users.Add(user);
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            return RedirectToAction("AddRestaurant","Home");
        }
        public JsonResult RemoveRestaurant(int id)
        {
            var q = _context.Restaurants.Where(x => x.Id == id).SingleOrDefault();
            String name = q.Name;
            _context.Restaurants.Remove(q);
            _context.SaveChanges();
            var q1=_context.users.ToList();
            var q2 = _context.orders.ToList();
            var q3 = _context.Images.ToList();
            foreach (var r in q1)
            {
                if (r.Name.Equals(name))
                {
                    _context.users.Remove(r);
                }
            }
            foreach (var r1 in q2)
            {
                if (r1.restaurantName.Equals(name))
                {
                    _context.orders.Remove(r1);
                }
            }
            foreach (var r2 in q3)
            {
                if (r2.RestaurantName.Equals(name))
                {
                    _context.Images.Remove(r2);
                }
            }
            _context.SaveChanges();
            return Json(0);
        }
        public ActionResult ViewRestaurants()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Index", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            List<Restaurant> q = _context.Restaurants.ToList();
            ViewBag.restaurants = q;
            return View();
        }
        public ActionResult ViewAllOrders()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Index", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            List<Orders> list1 = _context.orders.ToList();
            List<Orders> list2 = new List<Orders>(), list3 = new List<Orders>(), list4 = new List<Orders>();
            foreach (var q in list1)
            {
                if (q.status.Equals("Pending"))
                {
                    list2.Add(q);
                }
                else if (q.status.Equals("Fullfilled"))
                {
                    list3.Add(q);
                }
                else if (q.status.Equals("Expired"))
                {
                    list4.Add(q);
                }
            }
            ViewBag.pendings = list2;
            ViewBag.fullfilled = list3;
            ViewBag.expired = list4;
            return View();
        }
        public JsonResult CheckCredit(String credit)
        {
            System.Threading.Thread.Sleep(200);
            int count = 0;
            for(int i=0;i<credit.Length;i++){
                if (credit[i] >= '0' && credit[i] <= '9') count++;
            }
            if (count < 10) return Json(1);
            var data = _context.users.ToList();
            foreach (var r in data)
            {
                if (r.CreditCardId.Equals(credit))
                {
                    return Json(1);
                }
            }
            return Json(0);
        }
        public JsonResult CheckOwnerName(String OwnerName)
        {
            var q = _context.users.ToList();
            foreach (var r in q)
            {
                if (r.Name.Equals(OwnerName))
                {
                    return Json(1);
                }
            }
            return Json(0);
        }
        public JsonResult CheckEmail(String email)
        {
            System.Threading.Thread.Sleep(200);
            String str1 = email.ToLower();
            String[] str = str1.Split('@');
            if (!(str[1].Equals("gmail.com")) && !(str[1].Equals("ymail.com")) && !(str[1].Equals("hotmail.com")) && !(str[1].Equals("outlook.com")) && !(str[1].Equals("pucit.edu.pk")))
            {
                return Json(1);
            }
            else
            {
                var data = _context.users.ToList();
                foreach (var r in data)
                {
                    if (r.Email.Equals(str1))
                    {
                        return Json(1);
                    }
                }
                return Json(0);
            }
        }
        public JsonResult CheckRestaurantName(String name)
        {
            var q = _context.Restaurants.ToList();
            foreach(var r in q){
                if (r.RestaurantTitle.Equals(name)) return Json(1);
            }
            return Json(0);
        }
        public ActionResult Logout()
        {
            Response.Cookies["userName"].Value = " ";
            Response.Cookies["key"].Value = " ";
            return RedirectToAction("Login", "Index");
        }
        public ActionResult Index()
        {
            if (Request.Cookies["userName"] != null)
            {
                String str = (String)Request.Cookies["userName"].Value;
                if (str.Equals("")) return RedirectToAction("Login", "Index");
                if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
                if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
                if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            }
            return RedirectToAction("Login","Index");
        }
        public ActionResult ViewOrders()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            var o = _context.orders.ToList();
            String name = (String)Request.Cookies["userName"].Value;
            List<Orders> list1 = new List<Orders>();
            List<Orders> list2 = new List<Orders>();
            List<Orders> list3 = new List<Orders>();
            foreach (var i in o)
            {
                if (i.userName.Equals(name))
                {
                    if (i.status.Equals("Expired"))
                    {
                        list3.Add(i);
                    }
                    else if (i.status.Equals("Pending"))
                    {
                        list1.Add(i);
                    }
                    else
                    {
                        list2.Add(i);
                    }
                }
            }
            ViewBag.list1 = list1;
            ViewBag.list2 = list2;
            ViewBag.list3 = list3;
            return View();
        }
        public ActionResult ViewOrders2()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            String restaurantName = (String)Request.Cookies["restaurantName"].Value;
            var o = _context.orders.ToList();
            String name = (String)Request.Cookies["userName"].Value;
            List<Orders> list1 = new List<Orders>();
            List<Orders> list2 = new List<Orders>();
            List<Orders> list3 = new List<Orders>();
            foreach (var i in o)
            {
                if (i.restaurantName.Equals(restaurantName))
                {
                    if (i.status.Equals("Expired"))
                    {
                        list3.Add(i);
                    }
                    else if (i.status.Equals("Pending"))
                    {
                        list1.Add(i);
                    }
                    else
                    {
                        list2.Add(i);
                    }
                }
            }
            ViewBag.list1 = list1;
            ViewBag.list2 = list2;
            ViewBag.list3 = list3;
            return View();
        }
        public ActionResult ViewYourOrders()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            var o = _context.orders.ToList();
            String name=(String)Request.Cookies["userName"].Value;
            List<Orders> list1 = new List<Orders>();
            List<Orders> list2 = new List<Orders>();
            List<Orders> list3 = new List<Orders>();
            foreach (var i in o)
            {
                if (i.userName.Equals(name))
                {
                    if (i.status.Equals("Expired"))
                    {
                        list3.Add(i);
                    }
                    else if (i.status.Equals("Pending"))
                    {
                        list1.Add(i);
                    }
                    else
                    {
                        list2.Add(i);
                    }
                }
            }
            ViewBag.list1 = list1;
            ViewBag.list2 = list2;
            ViewBag.list3 = list3;
            return View();
        }
        public ActionResult ViewMenuItems()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            String name = (String)Request.Cookies["restaurantName"].Value;
            var images = (from x in _context.Images
                         where x.RestaurantName.Equals(name)
                         orderby x.Id descending
                         select x).ToList();
            ViewBag.images = images;
            return View();
        }
        public ActionResult ReceiveOrder(int id)
        {
            var o = _context.orders.ToList();
            foreach (var i in o)
            {
                if (i.Id == id)
                {
                    long p = i.payment;
                    i.status = "Fullfilled";
                    var r = _context.Restaurants.ToList();
                    foreach (var r1 in r)
                    {
                        if (r1.RestaurantTitle.Equals(i.restaurantName))
                        {
                            var u = _context.users.ToList();
                            foreach (var l in u)
                            {
                                if (l.Key.Equals("Admin"))
                                {
                                    l.CreditCardAmount += p / 50;
                                }
                                else if (r1.Name.Equals(l.Name))
                                {
                                    l.CreditCardAmount += (p / 50)*49;
                                }
                            }
                        }
                    }
                    _context.SaveChanges();
                    return Json(0);
                }
            }
            return Json(1);
        }
        public JsonResult cancelOrder(int id)
        {
            bool flag=false;
            var o = _context.orders.ToList();
            var user = _context.users.ToList();
            String name = (String)Request.Cookies["userName"].Value;
            long credit = 0;
            foreach (var i in o)
            {
                if (i.Id == id)
                {
                    if (i.balance == 0) credit = i.payment;
                    _context.orders.Remove(i);
                    _context.SaveChanges();
                    flag=true;
                    break;
                }
            }
            if (credit > 0)
            {
                foreach (var l in user)
                {
                    if (l.Name.Equals(name))
                    {
                        l.CreditCardAmount += credit;
                        _context.SaveChanges();
                    }
                }
            }
            if(flag==true)return Json(0);
            return Json(1);
        }
        public ActionResult AdminView()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            var restaurants = _context.Restaurants.ToList();
            var users = _context.users.ToList();
            var menuItems = _context.Images.ToList();
            var oP = _context.orders.ToList();
            long credit = 0;
            int totalRestaurants=0,totalUsers=0,totalPendingOrders=0,totalRemainingBalance=0,
                totalMenuItems=0,totalOrderPlaced=0,totalOrderFullfilled=0,totalOrderExpired=0,Profit=0,Loss=0,OverAllIncome=0;
            var o=_context.orders.Select(x=>x.Id>=1);
            foreach (var i in oP)
            {
                if (i.status.Equals("Pending"))
                {
                    if (i.TimeToFullfillOrder <= DateTime.Now)
                    {
                        i.status = "Expired";
                    }
                }
            }
            _context.SaveChanges();
            oP = _context.orders.ToList();
            foreach (var r in restaurants)
            {
                totalRestaurants++;
            }
            foreach (var u in users)
            {
                if (u.Key.Equals("Customer")) totalUsers++;
                else if (u.Key.Equals("Admin")) credit = u.CreditCardAmount;
            }
            foreach (var m in menuItems)
            {
                totalMenuItems++;
            }
            foreach (var p in oP)
            {
                totalOrderPlaced++;
                if (p.status == "Expired")
                {
                    Loss += p.payment;
                    totalOrderExpired++;
                }
                else if (p.status == "Fullfilled")
                {
                    Profit += p.payment;
                    totalOrderFullfilled++;
                }
                else
                {
                    totalPendingOrders++;
                    totalRemainingBalance += p.balance;
                }
            }
            OverAllIncome = Profit - Loss;
            ViewBag.credit = credit;
            ViewBag.Profit = Profit;
            ViewBag.Loss = Loss;
            ViewBag.totalMenuItems = totalMenuItems;
            ViewBag.totalRestaurants = totalRestaurants;
            ViewBag.totalUsers = totalUsers;
            ViewBag.OverAllIncome = OverAllIncome;
            ViewBag.totalRemainingBalance = totalRemainingBalance;
            ViewBag.totalPendingOrders = totalPendingOrders;
            ViewBag.totalOrdersExpired = totalOrderExpired;
            ViewBag.totalOrdersFullfilled = totalOrderFullfilled;
            ViewBag.totalOrdersPlaced = totalOrderPlaced;
            var images = from x in _context.Images
                         orderby x.Id descending
                         select x;
            ViewBag.images = images;
            return View();
        }
        public ActionResult RestaurantView()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            var menuItems = _context.Images.ToList();
            var oP = _context.orders.ToList();
            int totalPendingOrders = 0, totalRemainingBalance = 0, totalMenuItems = 0, totalOrderPlaced = 0,
                totalOrderFullfilled = 0, totalOrderExpired = 0, Profit = 0, Loss = 0, OverAllIncome = 0;
            var o = _context.orders.Select(x => x.Id >= 1);
            String restaurantName = (String)Request.Cookies["restaurantName"].Value;
            foreach (var i in oP)
            {
                if (i.status.Equals("Pending"))
                {
                    if (i.TimeToFullfillOrder <= DateTime.Now)
                    {
                        i.status = "Expired";
                    }
                }
            }
            _context.SaveChanges();
            oP = _context.orders.ToList();
            foreach (var m in menuItems)
            {
                if (m.RestaurantName == restaurantName)
                {
                    totalMenuItems++;
                }
            }
            foreach (var p in oP)
            {
                if (p.restaurantName == restaurantName)
                {
                    totalOrderPlaced++;
                    if (p.status == "Expired")
                    {
                        Loss += p.payment;
                        totalOrderExpired++;
                    }
                    else if (p.status == "Fullfilled")
                    {
                        Profit += p.payment;
                        totalOrderFullfilled++;
                    }
                    else
                    {
                        totalPendingOrders++;
                        totalRemainingBalance += p.balance;
                    }
                }
            }
            long credit = 0;
            var u = _context.users.ToList();
            foreach (var us in u)
            {
                if (us.Name.Equals(str)) credit = us.CreditCardAmount;
            }
            ViewBag.credit = credit;
            OverAllIncome = Profit - Loss;
            ViewBag.Profit = Profit;
            ViewBag.Loss = Loss;
            ViewBag.totalMenuItems = totalMenuItems;
            ViewBag.OverAllIncome = OverAllIncome;
            ViewBag.totalRemainingBalance = totalRemainingBalance;
            ViewBag.totalPendingOrders = totalPendingOrders;
            ViewBag.totalOrdersExpired = totalOrderExpired;
            ViewBag.totalOrdersFullfilled = totalOrderFullfilled;
            ViewBag.totalOrdersPlaced = totalOrderPlaced;
            var images = from x in _context.Images
                         orderby x.Id descending
                         select x;
            ViewBag.images = images;
            return View();
        }
        public ActionResult CustomerView()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            var oP = _context.orders.ToList();
            foreach (var i in oP)
            {
                if (i.status.Equals("Pending"))
                {
                    if (i.TimeToFullfillOrder <= DateTime.Now)
                    {
                        i.status = "Expired";
                    }
                }
            }
            _context.SaveChanges();
            oP = _context.orders.ToList();
            var images = from x in _context.Images
                         orderby x.Id descending
                         select x;
            var u = _context.users.ToList();
            long credit = 0;
            foreach (var us in u)
            {
                if (us.Name.Equals(str)) credit = us.CreditCardAmount;
            }
            ViewBag.credit = credit;
            ViewBag.images = images;
            return View();
        }
        public ActionResult ViewCustomerOrders()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            var o = _context.orders.ToList();
            String name = (String)Request.Cookies["userName"].Value;
            List<Orders> list1 = new List<Orders>();
            List<Orders> list2 = new List<Orders>();
            List<Orders> list3 = new List<Orders>();
            foreach (var i in o)
            {
                if (i.userName.Equals(name))
                {
                    if (i.status.Equals("Expired"))
                    {
                        list3.Add(i);
                    }
                    else if (i.status.Equals("Pending"))
                    {
                        list1.Add(i);
                    }
                    else
                    {
                        list2.Add(i);
                    }
                }
            }
            ViewBag.list1 = list1;
            ViewBag.list2 = list2;
            ViewBag.list3 = list3;
            return View();
        }
        public ActionResult ViewRestaurant(int id)
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            var q = _context.Restaurants.ToList();
            String name=" ";
            foreach(var s in q){
                if(s.Id==id){
                    ViewBag.restaurant = s;
                    name=s.RestaurantTitle;
                }
            }
            var r=_context.Images.ToList();
            List<Image> list1 = new List<Image>();
            List<Image> list2 = new List<Image>();
            List<Image> list3 = new List<Image>();
            List<Image> list4 = new List<Image>();
            List<Image> list5 = new List<Image>();
            List<Image> list6 = new List<Image>();
            List<Image> list7 = new List<Image>();
            List<Image> list8 = new List<Image>();
            List<Image> list9 = new List<Image>();
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0, count5 = 0, count6 = 0, count7 = 0, count8 = 0, count9 = 0;
            foreach (var s in r)
            {
                if (s.RestaurantName.Equals(name))
                {
                    if (s.Type.Equals("Fast Food")) { list1.Add(s); count1++; }
                    else if (s.Type.Equals("Desi")) { list2.Add(s); count2++; }
                    else if (s.Type.Equals("Desert")) { list3.Add(s); count3++; }
                    else if (s.Type.Equals("Drinks")) { list4.Add(s); count4++; }
                    else if (s.Type.Equals("Bar_B_Q")) { list5.Add(s); count5++; }
                    else if (s.Type.Equals("Starters")) { list6.Add(s); count6++; }
                    else if (s.Type.Equals("Pizza")) { list7.Add(s); count7++; }
                    else if (s.Type.Equals("Burger")) { list8.Add(s); count8++; }
                    else if (s.Type.Equals("Sandwitches")) { list9.Add(s); count9++; }
                }
            }
            ViewBag.list1 = list1;
            ViewBag.count1 = count1;
            ViewBag.list2 = list2;
            ViewBag.count2 = count2;
            ViewBag.list3 = list3;
            ViewBag.count3 = count3;
            ViewBag.list4 = list4;
            ViewBag.count4 = count4;
            ViewBag.list5 = list5;
            ViewBag.count5 = count5;
            ViewBag.list6 = list6;
            ViewBag.count6 = count6;
            ViewBag.list7 = list7;
            ViewBag.count7 = count7;
            ViewBag.list8 = list8;
            ViewBag.count8 = count8;
            ViewBag.list9 = list9;
            ViewBag.count9 = count9;
            return View();
        }
        public ActionResult ViewRestaurant3(int id)
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            var q = _context.Restaurants.ToList();
            String name = " ";
            foreach (var s in q)
            {
                if (s.Id == id)
                {
                    ViewBag.restaurant = s;
                    name = s.RestaurantTitle;
                }
            }
            var r = _context.Images.ToList();
            List<Image> list1 = new List<Image>();
            List<Image> list2 = new List<Image>();
            List<Image> list3 = new List<Image>();
            List<Image> list4 = new List<Image>();
            List<Image> list5 = new List<Image>();
            List<Image> list6 = new List<Image>();
            List<Image> list7 = new List<Image>();
            List<Image> list8 = new List<Image>();
            List<Image> list9 = new List<Image>();
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0, count5 = 0, count6 = 0, count7 = 0, count8 = 0, count9 = 0;
            foreach (var s in r)
            {
                if (s.RestaurantName.Equals(name))
                {
                    if (s.Type.Equals("Fast Food")) { list1.Add(s); count1++; }
                    else if (s.Type.Equals("Desi")) { list2.Add(s); count2++; }
                    else if (s.Type.Equals("Desert")) { list3.Add(s); count3++; }
                    else if (s.Type.Equals("Drinks")) { list4.Add(s); count4++; }
                    else if (s.Type.Equals("Bar_B_Q")) { list5.Add(s); count5++; }
                    else if (s.Type.Equals("Starters")) { list6.Add(s); count6++; }
                    else if (s.Type.Equals("Pizza")) { list7.Add(s); count7++; }
                    else if (s.Type.Equals("Burger")) { list8.Add(s); count8++; }
                    else if (s.Type.Equals("Sandwitches")) { list9.Add(s); count9++; }
                }
            }
            ViewBag.list1 = list1;
            ViewBag.count1 = count1;
            ViewBag.list2 = list2;
            ViewBag.count2 = count2;
            ViewBag.list3 = list3;
            ViewBag.count3 = count3;
            ViewBag.list4 = list4;
            ViewBag.count4 = count4;
            ViewBag.list5 = list5;
            ViewBag.count5 = count5;
            ViewBag.list6 = list6;
            ViewBag.count6 = count6;
            ViewBag.list7 = list7;
            ViewBag.count7 = count7;
            ViewBag.list8 = list8;
            ViewBag.count8 = count8;
            ViewBag.list9 = list9;
            ViewBag.count9 = count9;
            return View();
        }
        public ActionResult ViewRestaurant2(int id)
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            var q = _context.Restaurants.ToList();
            String name = " ";
            foreach (var s in q)
            {
                if (s.Id == id)
                {
                    ViewBag.restaurant = s;
                    name = s.RestaurantTitle;
                }
            }
            var r = _context.Images.ToList();
            List<Image> list1 = new List<Image>();
            List<Image> list2 = new List<Image>();
            List<Image> list3 = new List<Image>();
            List<Image> list4 = new List<Image>();
            List<Image> list5 = new List<Image>();
            List<Image> list6 = new List<Image>();
            List<Image> list7 = new List<Image>();
            List<Image> list8 = new List<Image>();
            List<Image> list9 = new List<Image>();
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0, count5 = 0, count6 = 0, count7 = 0, count8 = 0, count9 = 0;
            foreach (var s in r)
            {
                if (s.RestaurantName.Equals(name))
                {
                    if (s.Type.Equals("Fast Food")) { list1.Add(s); count1++; }
                    else if (s.Type.Equals("Desi")) { list2.Add(s); count2++; }
                    else if (s.Type.Equals("Desert")) { list3.Add(s); count3++; }
                    else if (s.Type.Equals("Drinks")) { list4.Add(s); count4++; }
                    else if (s.Type.Equals("Bar_B_Q")) { list5.Add(s); count5++; }
                    else if (s.Type.Equals("Starters")) { list6.Add(s); count6++; }
                    else if (s.Type.Equals("Pizza")) { list7.Add(s); count7++; }
                    else if (s.Type.Equals("Burger")) { list8.Add(s); count8++; }
                    else if (s.Type.Equals("Sandwitches")) { list9.Add(s); count9++; }
                }
            }
            ViewBag.list1 = list1;
            ViewBag.count1 = count1;
            ViewBag.list2 = list2;
            ViewBag.count2 = count2;
            ViewBag.list3 = list3;
            ViewBag.count3 = count3;
            ViewBag.list4 = list4;
            ViewBag.count4 = count4;
            ViewBag.list5 = list5;
            ViewBag.count5 = count5;
            ViewBag.list6 = list6;
            ViewBag.count6 = count6;
            ViewBag.list7 = list7;
            ViewBag.count7 = count7;
            ViewBag.list8 = list8;
            ViewBag.count8 = count8;
            ViewBag.list9 = list9;
            ViewBag.count9 = count9;
            return View();
        }
        public ActionResult ViewRestaurants2()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            List<Restaurant> q = _context.Restaurants.ToList();
            ViewBag.restaurants = q;
            return View();
        }
        public ActionResult SearchFunction3(String search)
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if (search == null) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            List<RestaurantSearch> list1 = new List<RestaurantSearch>();
            List<ImageSearch> list2 = new List<ImageSearch>();
            var r = _context.Restaurants.ToList();
            var i = _context.Images.ToList();
            foreach (var re in r)
            {
                RestaurantSearch rs = new RestaurantSearch();
                rs.restaurant = re;
                rs.count = searchCount(search.ToLower(), re.RestaurantTitle.ToLower());
                list1.Add(rs);
            }
            foreach (var im in i)
            {
                ImageSearch ism = new ImageSearch();
                ism.image = im;
                ism.count = searchCount(search.ToLower(), im.Title.ToLower());
                list2.Add(ism);
            }
            var a = from x in list1
                    orderby x.count descending
                    select x;
            var b = from x in list2
                    orderby x.count descending
                    select x;
            ViewBag.list1 = a;
            ViewBag.list2 = b;
            return View();
        }
        public ActionResult SearchFunction(String search)
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if (search == null) return RedirectToAction("AdminView", "Home");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            List<RestaurantSearch> list1 = new List<RestaurantSearch>();
            List<ImageSearch> list2 = new List<ImageSearch>();
            var r = _context.Restaurants.ToList();
            var i = _context.Images.ToList();
            foreach (var re in r)
            {
                RestaurantSearch rs = new RestaurantSearch();
                rs.restaurant = re;
                rs.count = searchCount(search.ToLower(), re.RestaurantTitle.ToLower());
                list1.Add(rs);
            }
            foreach (var im in i)
            {
                ImageSearch ism = new ImageSearch();
                ism.image = im;
                ism.count = searchCount(search.ToLower(), im.Title.ToLower());
                list2.Add(ism);
            }
            var a = from x in list1
                    orderby x.count descending
                    select x;
            var b = from x in list2
                    orderby x.count descending
                    select x;
            ViewBag.list1 = a;
            ViewBag.list2 = b;
            return View();
        }
        public ActionResult SearchFunction2(String search)
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if (search == null) return RedirectToAction("RestaurantView", "Home");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            List<RestaurantSearch> list1 = new List<RestaurantSearch>();
            List<ImageSearch> list2 = new List<ImageSearch>();
            var r = _context.Restaurants.ToList();
            var i = _context.Images.ToList();
            foreach (var re in r)
            {
                RestaurantSearch rs=new RestaurantSearch();
                rs.restaurant=re;
                rs.count=searchCount(search.ToLower(),re.RestaurantTitle.ToLower());
                list1.Add(rs);
            }
            foreach (var im in i)
            {
                ImageSearch ism = new ImageSearch();
                ism.image = im;
                ism.count = searchCount(search.ToLower(), im.Title.ToLower());
                list2.Add(ism);
            }
            var a = from x in list1
                    orderby x.count descending
                    select x;
            var b = from x in list2
                    orderby x.count descending
                    select x;
            ViewBag.list1 = a;
            ViewBag.list2 = b;
            return View();
        }
        private int max(int a, int b)
        {
            if (a > b) return a;
            return b;
        }
        private int searchCount(String str1, String str2)
        {
            int [,] arr1=new int[str1.Length+1,str2.Length+1];
            for (int i = 0; i <= str1.Length; i++) {
                for (int j = 0; j <= str2.Length; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        arr1[i,j] = 0;
                    }
                    else
                    {
                        if (str1[i-1].Equals(str2[j-1]))
                        {
                            arr1[i, j] = arr1[i - 1, j - 1] + 1;
                        }
                        else
                        {
                            arr1[i, j] = max(arr1[i - 1, j], arr1[i, j - 1]);
                        }
                    }
                }
            }
            return arr1[str1.Length,str2.Length];
        }
    }
}