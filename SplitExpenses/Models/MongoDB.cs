using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SplitExpenses.Models
{
    public class MongoDB
    {
        private string connectionString = File.ReadAllText(Properties.Settings.Default.PathCredentials + @"\Mongo.txt");
        private IMongoDatabase GetDatabase()
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var mongoClient = new MongoClient(settings);

            return mongoClient.GetDatabase("SplitExpenses");
        }

        internal User GetUser(string username, string password)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq("Username", username);
                filter &= Builders<User>.Filter.Eq("Password", password);

                var user = users.Find(filter).FirstOrDefault();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        internal bool InsertUser(User user)
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

        internal bool UpdateUser(User user)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq("Username", user.Username);

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

        internal bool DeleteUser(User user)
        {
            try
            {
                IMongoDatabase splitExpenses = GetDatabase();
                IMongoCollection<User> users = splitExpenses.GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq("Username", user.Username);
                filter &= Builders<User>.Filter.Eq("Password", user.Password);

                var delete = users.DeleteOne(filter);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }
    }
}