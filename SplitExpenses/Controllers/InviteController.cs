using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SplitExpenses.Controllers
{
    public class InviteController : Controller
    {
        public async Task<JsonResult> GetInvites()
        {
            try
            {
                if (!Authentication())
                    return Json("");

                var mongo = new Mongo();
                var invitations = await mongo.GetCountInvitations(((User)Session["InfoUser"]).Username);

                return Json(invitations);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public async Task<JsonResult> UpdateInvites(int idIncremental, int val)
        {
            try
            {
                if (Authentication())
                {
                    var account = CheckAccount(idIncremental);
                    if (account != null && account.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Invitation == 0)
                    {
                        account.Users.Find(x => x.Name == ((User)Session["InfoUser"]).Username).Invitation = val;

                        var accountforUpdate = new Account();
                        accountforUpdate.Users = account.Users;

                        var mongo = new Mongo();
                        var update = await mongo.UpdateAccount(accountforUpdate, account.Id);

                        if (update)
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
    }
}