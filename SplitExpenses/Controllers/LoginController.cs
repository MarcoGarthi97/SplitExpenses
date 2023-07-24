using Newtonsoft.Json;
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
        
        public JsonResult Register(string json)
        {
            try
            {
                var user = JsonConvert.DeserializeObject<User>(json);

                Mongo mongo = new Mongo();
                var findUser = mongo.CheckUser(user.Username).Result;

                if (!findUser)
                {
                    var insert = mongo.InsertUser(user).Result;
                    if (insert)
                    {
                        var result = Logon(user.Username, user.Password);

                        return Json(result);
                    }

                }
            }
            catch (Exception ex)
            {

            }
            
            return Json(false);
        }

        public JsonResult CheckUsername(string username)
        {
            var mongo = new Mongo();
            var result = mongo.CheckUser(username);

            return Json(result);
        }
    }
}