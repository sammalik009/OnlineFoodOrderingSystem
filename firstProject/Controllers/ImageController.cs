using firstProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using firstProject.DAL;

namespace firstProject.Controllers
{
    public class ImageController : Controller
    {
        private RestaurantContext _context;
        public ImageController()
        {
            _context = new RestaurantContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpGet]
        public ActionResult Add()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            return View("Add");
        }
        [HttpPost]
        public ActionResult Add(Image imageModel)
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            String fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
            String extension = Path.GetExtension(imageModel.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            imageModel.ImagePath = "~/Image/" + fileName;
            imageModel.RestaurantName = (String)Request.Cookies["restaurantName"].Value;
            fileName=Path.Combine(Server.MapPath("~/Image/"),fileName);
            imageModel.ImageFile.SaveAs(fileName);
            _context.Images.Add(imageModel);
            _context.SaveChanges();
            return RedirectToAction("Add","Image");
        }
        public JsonResult CheckTitle(String title)
        {
            System.Threading.Thread.Sleep(200);
            title = title.ToUpper();
            var data = _context.Images.Select(x => x.Title == title).FirstOrDefault();
            if (data == true) return Json(1);
            return Json(0);
        }
        [HttpGet]
        public ActionResult View()
        {
            if (Request.Cookies["userName"] == null) return RedirectToAction("Login", "Index");
            String str = (String)Request.Cookies["userName"].Value;
            if (str.Equals("")) return RedirectToAction("Login", "Index");
            if ("Customer".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("CustomerView", "Home");
            if ("Admin".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("AdminView", "Home");
            if ("Restaurant".Equals((String)Request.Cookies["key"].Value)) return RedirectToAction("RestaurantView", "Home");
            return View();
         }
    }
}