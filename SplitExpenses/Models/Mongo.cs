using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SplitExpenses.Models
{
    public class Mongo
    {
        private string connectionString = File.ReadAllText(Properties.Settings.Default.PathCredentials + @"\Mongo.txt");
        private IMongoDatabase GetDatabase()
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var mongoClient = new MongoClient(settings);

            return mongoClient.GetDatabase("SplitExpenses");
        }

        internal async Task<User> GetUser(string username, string password)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq(x => x.Username, username);
                filter &= Builders<User>.Filter.Eq(x => x.Password, password);

                var user = users.Find(filter).FirstOrDefault();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        internal async Task<List<User>> GetUsers(string filter)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var user = users.Aggregate().Match(x => x.Username.ToLower().Contains(filter.ToLower())).ToList();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        internal async Task<bool> CheckUser(string username)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq(x => x.Username, username);

                var user = users.Find(filter).FirstOrDefault();
                if(user != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> CheckUser(string username, string password)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq(x => x.Username, username);
                filter &= Builders<User>.Filter.Eq(x => x.Password, password);

                var user = users.Find(filter).FirstOrDefault();
                if (user != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> InsertUser(User user)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                users.InsertOne(user);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> UpdateUser(User user)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq(x => x.Username, user.Username);

                var update = Builders<User>.Update.Set(x => x, user);
                var updateResult = users.UpdateOne(filter, update);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> DeleteUser(User user)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq(x => x.Username, user.Username);
                filter &= Builders<User>.Filter.Eq(x => x.Password, user.Password);

                var delete = users.DeleteOne(filter);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<List<Account>> GetAccounts(string username)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Account> accounts = splitExpenses.GetCollection<Account>("Account");

                //var filter = Builders<Account>.Filter.Eq(x => x.Users.Contains(username), username);

                var filter = Builders<Account>.Filter.ElemMatch(x => x.Users, u => u.Name == username);

                var listAccount = accounts.Find(filter).ToList();

                return listAccount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        internal async Task<Account> GetAccount(ObjectId id)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Account> accounts = splitExpenses.GetCollection<Account>("Account");

                var filter = Builders<Account>.Filter.Eq(x => x.Id, id);

                var account = accounts.Find(filter).FirstOrDefault();

                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        internal async Task<bool> InsertAccount(Account account)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Account> accounts = splitExpenses.GetCollection<Account>("Account");

                accounts.InsertOne(account);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> UpdateAccount(Account account, Object id)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Account> accounts = splitExpenses.GetCollection<Account>("Account");

                var filter = Builders<Account>.Filter.Eq(x => x.Id, id);

                var updateDefinition = new List<UpdateDefinition<Account>>();

                if (account.Name != null)
                    updateDefinition.Add(Builders<Account>.Update.Set("Name", account.Name));

                if (account.UserExpenses != 0)
                    updateDefinition.Add(Builders<Account>.Update.Set("UserExpenses", account.UserExpenses));

                if (account.TotalExpenses != 0)
                    updateDefinition.Add(Builders<Account>.Update.Set("TotalExpenses", account.TotalExpenses));

                if (account.Users != null)
                    updateDefinition.Add(Builders<Account>.Update.Set("Users", account.Users));

                var update = Builders<Account>.Update.Combine(updateDefinition);

                var updateResult = accounts.UpdateOne(filter, update);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> DeleteAccount(ObjectId id)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();

                var expenses = GetExpenses(id).Result;
                foreach (var expense in expenses)
                    DeleteExpense(expense.Id);

                IMongoCollection<Account> accounts = splitExpenses.GetCollection<Account>("Account");

                var filter = Builders<Account>.Filter.Eq(x => x.Id, id);

                var delete = accounts.DeleteOne(filter);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<Expense> GetExpense(ObjectId id)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Expense> expenses = splitExpenses.GetCollection<Expense>("Expenses");

                var filter = Builders<Expense>.Filter.Eq(x => x.Id, id);

                var expense = expenses.Find(filter).FirstOrDefault();

                return expense;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        internal async Task<List<Expense>> GetExpenses(ObjectId fatherId)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Expense> expenses = splitExpenses.GetCollection<Expense>("Expenses");

                var filter = Builders<Expense>.Filter.Eq(x => x.FatherId, fatherId);

                var listExpenses = expenses.Aggregate().Match(filter).ToList();

                return listExpenses;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        internal async Task<bool> InsertExpense(Expense expense)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Expense> expenses = splitExpenses.GetCollection<Expense>("Expenses");

                expenses.InsertOne(expense);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> UpdateExpense(Expense expense, ObjectId id)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Expense> expenses = splitExpenses.GetCollection<Expense>("Expenses");

                var filter = Builders<Expense>.Filter.Eq(x => x.Id, id);

                var updateDefinition = new List<UpdateDefinition<Expense>>();

                if (expense.FatherId != null)
                    updateDefinition.Add(Builders<Expense>.Update.Set("FatherId", expense.FatherId));

                if (!string.IsNullOrEmpty(expense.Name))
                    updateDefinition.Add(Builders<Expense>.Update.Set("Name", expense.Name));

                if (!string.IsNullOrEmpty(expense.Category))
                    updateDefinition.Add(Builders<Expense>.Update.Set("Category", expense.Category));

                if (expense.Date != default(DateTime))
                    updateDefinition.Add(Builders<Expense>.Update.Set("Date", expense.Date));

                if (!string.IsNullOrEmpty(expense.PaidBy))
                    updateDefinition.Add(Builders<Expense>.Update.Set("PaidBy", expense.PaidBy));

                if (expense.PaidFor != null)
                    updateDefinition.Add(Builders<Expense>.Update.Set("PaidFor", expense.PaidFor));

                if (expense.Cost != 0)
                    updateDefinition.Add(Builders<Expense>.Update.Set("Cost", expense.Cost));

                var update = Builders<Expense>.Update.Combine(updateDefinition);

                var updateResult = expenses.UpdateOne(filter, update);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<bool> DeleteExpense(ObjectId id)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Expense> expenses = splitExpenses.GetCollection<Expense>("Expenses");

                var filter = Builders<Expense>.Filter.Eq(x => x.Id, id);

                var delete = expenses.DeleteOne(filter);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        internal async Task<List<Account>> GetCountInvitations(string username)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Account> accounts = splitExpenses.GetCollection<Account>("Account");

                var filter = Builders<Account>.Filter.ElemMatch(x => x.Users, u => u.Name == username && u.Invitation == 0);

                var invitation = accounts.Aggregate().Match(filter).ToList();

                return invitation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}