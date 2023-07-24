using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using SplitExpenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public JsonResult GetExpense(int idIncremental)
        {
            if (Authentication())
            {
                var expense = CheckEspense(idIncremental);
                if (expense != null)
                    return Json(expense);
            }

            return Json("");
        }

        public async Task<JsonResult> InsertExpense(string name, string cost, DateTime date, string owner, List<string> usersFor)
        {
            try
            {
                if (!Authentication())
                    return Json("");

                double value = Convert.ToDouble(cost);
                value = Math.Round(value, 2);

                var expense = new Expense(
                    ((Account)Session["Account"]).Id,
                    name,
                    date,
                    owner,
                    usersFor,
                    value
                    );

                var mongo = new Mongo();
                var insert = mongo.InsertExpense(expense).Result;

                RecalculateAll();

                await ReloadExpenses(expense);

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public async Task<JsonResult> UpdateExpense(int idIncremental, string name, string cost, DateTime date, string owner, List<string> usersFor)
        {
            try
            {
                if (Authentication())
                {
                    var expenseCheck = CheckEspense(idIncremental);
                    if (expenseCheck != null)
                    {
                        var expense = new Expense(expenseCheck.FatherId, name, date, owner, usersFor, Convert.ToDouble(cost));

                        var mongo = new Mongo();
                        var update = mongo.UpdateExpense(expense, expenseCheck.Id);

                        RecalculateAll();

                        await ReloadExpenses(expense);

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

        public async Task<JsonResult> DeleteExpense(int idIncremental)
        {
            try
            {
                if (Authentication())
                {
                    var expense = CheckEspense(idIncremental);
                    if (expense != null)
                    {
                        var mongo = new Mongo();
                        var delete = mongo.DeleteExpense(expense.Id);

                        RecalculateAll();

                        await ReloadExpenses(expense);

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

        public Expense CheckEspense(int idIncremental)
        {
            var mongo = new Mongo();
            var expenses = mongo.GetExpenses(((Account)Session["Account"]).Id).Result;

            foreach (var expense in expenses)
            {
                if (expense.Id.Increment == idIncremental)
                    return expense;
            }

            return null;
        }

        public async Task<JsonResult> GetBalanceUsers()
        {
            if (!Authentication())
                return Json("");

            var mongo = new Mongo();
            var account = await mongo.GetAccount(((Account)Session["Account"]).Id);

            return Json(account.Users);
        }

        private async Task ReloadExpenses(Expense expense)
        {
            var mongo = new Mongo();

            var users = expense.PaidFor;
            users.Add(expense.PaidBy);

            foreach (var user in users)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                context.Clients.User(user).GetExpenses();
                context.Clients.User(user).GetBalance();
            }
        }

        private void RecalculateAll()
        {
            var recalculatedAmounts = RecalculateAmounts();

            double total = GetTotalAccount();
            var account = new Account(recalculatedAmounts, total);

            var mongo = new Mongo();
            var update = mongo.UpdateAccount(account, ((Account)Session["Account"]).Id);
        }

        private double GetTotalAccount()
        {
            var mongo = new Mongo();
            var expenses = mongo.GetExpenses(((Account)Session["Account"]).Id).Result;

            double total = 0;
            foreach (var expense in expenses)
            {
                total += expense.Cost;
            }

            return total;
        }

        private List<UsersAccount> RecalculateAmounts()
        {
            var mongo = new Mongo();

            var account = mongo.GetAccount(((Account)Session["Account"]).Id).Result;
            var result = account.Users;

            foreach (var user in result)
                result[result.IndexOf(user)].Balance = 0;

            var expenses = mongo.GetExpenses(((Account)Session["Account"]).Id).Result;

            try
            {
                foreach (var expense in expenses)
                {
                    double amount = expense.Cost - (expense.Cost / Convert.ToDouble(expense.PaidFor.Count + 1));
                    amount = Math.Round(amount, 2);

                    var user = result.Find(x => x.Name == expense.PaidBy);
                    result[result.IndexOf(user)].Balance += amount;

                    foreach (var userPaidFor in expense.PaidFor)
                    {
                        amount = expense.Cost / Convert.ToDouble(expense.PaidFor.Count + 1);
                        amount = Math.Round(amount, 2);

                        user = result.Find(x => x.Name == userPaidFor);
                        result[result.IndexOf(user)].Balance -= amount;
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return result;
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