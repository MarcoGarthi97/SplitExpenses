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
        static public string _username;
        public JsonResult Logon(string username, string password)
        {
            Mongo mongo = new Mongo();
            var findUser = mongo.GetUser(username, password).Result;

            if(findUser != null)
            {
                Session["InfoUser"] = findUser;
                Session["Logon"] = true;

                _username = findUser.Username;

                return Json(true);
            }
            else
                return Json(false);
        }        
    }
}