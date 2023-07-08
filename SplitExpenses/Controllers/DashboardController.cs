using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SplitExpenses.Controllers
{
    public class DashboardController : Controller
    {
        public JsonResult GetAccounts()
        {
            try
            {
                if (!Authentication())
                    return Json("");

                var mongo = new Mongo();
                var accounts = mongo.GetAccounts(((User)Session["InfoUser"]).Username);

                return Json(accounts.Result);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public JsonResult GetUsers(string filter)
        {
            try
            {
                if (!Authentication())
                    return Json("");

                var mongo = new Mongo();
                var users = mongo.GetUsers(filter).Result;

                var usernames = new List<string>();
                foreach (var user in users)
                    usernames.Add(user.Username);

                return Json(usernames);
            }
            catch (Exception ex)
            {
                return Json("");
            }
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