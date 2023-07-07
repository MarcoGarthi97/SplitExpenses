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
                    return Json("franco");

                var mongo = new Mongo();
                var accounts = mongo.GetAccounts(((User)Session["InfoUser"]).Username);

                return Json(accounts.Result);
            }
            catch (Exception ex)
            {
                return Json(null);
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