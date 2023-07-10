﻿using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SplitExpenses.Controllers
{
    public class AccountController : Controller
    {
        public JsonResult GetExpenses()
        {
            if (!Authentication())
                return Json("");

            var mongo = new Mongo();
            var expenses = mongo.GetExpenses(((Account)Session["Account"]).Id).Result;

            return Json(expenses);
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