using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

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

                var listAccounts = new List<Account>();
                foreach (var account in accounts)
                {
                    if(account.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Invitation == 1)
                    {
                        accounts.Find(x => x.Id == account.Id).UserExpenses = account.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Balance;
                        listAccounts.Add(account);
                    }
                }

                return Json(listAccounts);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public JsonResult GetAccount()
        {
            if (!Authentication())
                return Json("");

            return Json(((Account)Session["Account"]));
        }

        public JsonResult GetAccountInfo(int idIncremental)
        {
            try
            {
                if (Authentication())
                {
                    var accountCheck = CheckAccount(idIncremental);
                    if (accountCheck != null && accountCheck.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Invitation == 1)
                    {
                        var mongo = new Mongo();
                        Session["Account"] = mongo.GetAccount(accountCheck.Id).Result;

                        var account = ((Account)Session["Account"]).Clone();
                        account.Users.RemoveAll(x => x.Invitation != 1 || x.Name == ((User)Session["InfoUser"]).Username);

                        return Json(account);
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return Json("");
        }

        public JsonResult GetUser()
        {
            if (!Authentication())
                return Json("");

            return Json(((User)Session["InfoUser"]).Username);
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

        public async Task<JsonResult> InsertAccount(string name, List<string> users)
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
                        userAccount.Owner = false;
                        userAccount.Invitation = 0;
                    }

                    usersAccount.Add(userAccount);
                }

                var account = new Account(name, usersAccount);

                var mongo = new Mongo();
                var insert = mongo.InsertAccount(account).Result;

                if (!insert)
                    return Json("");

                await Notification(account.Users);

                var accounts = mongo.GetAccounts(((User)Session["InfoUser"]).Username).Result;
                return Json(accounts);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public async Task<JsonResult> UpdateAccount(string name, int idIncremental)
        {
            try
            {
                if (Authentication())
                {
                    var account = CheckAccount(idIncremental);
                    if (account != null && account.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Invitation == 1)
                    {
                        var updateAccount = new Account(name);

                        var mongo = new Mongo();
                        var delete = mongo.UpdateAccount(updateAccount, account.Id).Result;

                        await Notification(account.Users);

                        if (delete)
                            return Json(true);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(false);
        }

        public async Task<JsonResult> DeleteAccount(int idIncremental)
        {
            try
            {
                if (Authentication())
                {
                    var account = CheckAccount(idIncremental);
                    if (account != null && account.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Invitation == 1)
                    {
                        var mongo = new Mongo();
                        var delete = mongo.DeleteAccount(account.Id).Result;

                        await Notification(account.Users);

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

        public async Task<bool> Notification(List<UsersAccount> users)
        {
            var mongo = new Mongo();

            foreach(var user in users)
            {
                if(user.Invitation == 0)
                {
                    var invitations = await mongo.GetCountInvitations(user.Name);

                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.User(user.Name).GetCountInvitations(invitations.Count);
                }
            }

            return true;
        }

        public Account CheckAccount(int? idIncremental)
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