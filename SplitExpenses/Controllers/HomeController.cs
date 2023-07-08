using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SplitExpenses.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["Logon"] = false;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Accounts()
        {
            return View();
        }

        public ActionResult Expenses()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            //return View(); //poi da togliere
            if (Authentication())
                return View();
            else
                return View("Login");
        }

        public bool Authentication()
        {
            try
            {
                if (Session["Logon"] != null && (bool)Session["Logon"])
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Session["Logon"] = false;
                return false;
            }
        }
    }
}