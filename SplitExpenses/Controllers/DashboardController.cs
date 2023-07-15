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
                var accounts = mongo.GetAccounts(((User)Session["InfoUser"]).Username).Result;

                foreach (var account in accounts)
                {
                    accounts.Find(x => x.Id == account.Id).UserExpenses = account.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Balance;
                }

                return Json(accounts);
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
                    if(((User)Session["InfoUser"]).Username != user.Username)
                        usernames.Add(user.Username);

                return Json(usernames);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public JsonResult InsertAccount(string name, List<string> users)
        {
            try
            {
                if (!Authentication())
                    return Json("");

                List<UsersAccount> usersAccount = new List<UsersAccount>();
                users.Add(((User)Session["InfoUser"]).Username);
                foreach(var user in users)
                {
                    UsersAccount userAccount = new UsersAccount();
                    userAccount.Name = user;
                    userAccount.Balance = 0;

                    if(user == ((User)Session["InfoUser"]).Username)
                    {
                        userAccount.Owner = true;
                        userAccount.Invitation = 1;
                    }
                    else
                    {
                        userAccount.Owner = true;
                        userAccount.Invitation = 0;
                    }

                    usersAccount.Add(userAccount);
                }

                var account = new Account(name, usersAccount);

                var mongo = new Mongo();
                var insert = mongo.InsertAccount(account).Result;

                if (!insert)
                    return Json("");

                var accounts = mongo.GetAccounts(((User)Session["InfoUser"]).Username).Result;
                return Json(accounts);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public JsonResult DeleteAccount(int idIncremental)
        {
            try
            {
                if (Authentication())
                {
                    var account = CheckAccount(idIncremental);
                    if (account != null)
                    {
                        var mongo = new Mongo();
                        var delete = mongo.DeleteAccount(account.Id).Result;

                        if (delete)
                            return Json(true);
                    }
                }

                return Json(false);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public Account CheckAccount(int idIncremental)
        {
            var mongo = new Mongo();
            var accounts = mongo.GetAccounts(((User)Session["InfoUser"]).Username).Result;

            foreach (var account in accounts)
            {
                if (account.Id.Increment == idIncremental)
                    return account;
            }

            return null;
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