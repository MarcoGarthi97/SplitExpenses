using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SplitExpenses.Controllers
{
    public class AuthController : Controller
    {
        public JsonResult Logon(string username, string password)
        {
            Mongo mongo = new Mongo();
            var findUser = mongo.CheckUser(username, password);

            return Json(findUser);
        }

        public ActionResult Dashboard()
        {
            if(Authentication())
                return View();
            else
                return View("Login");
        }

        public bool Authentication()
        {
            try
            {
                if ((bool)Session["Logon"])
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}