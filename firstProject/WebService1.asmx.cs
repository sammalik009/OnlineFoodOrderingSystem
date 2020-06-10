using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace firstProject
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public void checkUserName(String userName)
        {
            //Console.WriteLine(name);
            String message = "SUCCESS";
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(message));
            //  return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }  
    }
}
