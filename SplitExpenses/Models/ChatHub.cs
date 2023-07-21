using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using SplitExpenses.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitExpenses.Models
{
    public class ChatHub : Hub
    {
        public async Task Notify(string username)
        {
            var mongo = new Mongo();
            var invitations = await mongo.GetCountInvitations(LoginController._username);

            Clients.User(username).GetCountInvitations(invitations.Count);
        }

        public async Task ReloadExpenses(string username)
        {
            Clients.User(username).GetExpenses();
        }

        public async Task ReloadBalance(string username)
        {
            Clients.User(username).GetBalance();
        }
    }
}