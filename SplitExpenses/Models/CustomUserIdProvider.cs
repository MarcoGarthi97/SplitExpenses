using Microsoft.AspNet.SignalR;
using SplitExpenses.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitExpenses.Models
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            // your logic to fetch a user identifier goes here.

            // for example:

            var userId = request.User.Identity.Name;
            return LoginController._username;
        }
    }
}
