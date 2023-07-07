using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SplitExpenses.Controllers
{
    public class LoginController : Controller
    {
        public JsonResult Logon(string username, string password)
        {
            Mongo mongo = new Mongo();
            var findUser = mongo.GetUser(username, password);

            if(findUser != null)
            {
                Session["InfoUser"] = findUser;
                return Json(true);
            }
            else
                return Json(false);
        }        
    }
}