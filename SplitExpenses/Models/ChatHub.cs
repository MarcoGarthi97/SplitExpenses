using Microsoft.AspNet.SignalR;
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
        public async Task Notify()
        {
            var mongo = new Mongo();
            var invitations = await mongo.GetCountInvitations(LoginController._username);

            Clients.All.GetCountInvitations(invitations.Count);
        }
    }
}