﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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

                var filter = Builders<Account>.Filter.ElemMatch(x => x.Users, u => u.Name == username && u.Invitation == 1);

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

                var update = Builders<Account>.Update.Set(x => x.Users, account.Users).Set(x => x.TotalExpenses, account.TotalExpenses);

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

        internal async Task<bool> UpdateExpense(Expense expense)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<Expense> expenses = splitExpenses.GetCollection<Expense>("Expenses");

                var filter = Builders<Expense>.Filter.Eq(x => x.Id, expense.Id);

                var update = Builders<Expense>.Update.Set(x => x, expense);
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