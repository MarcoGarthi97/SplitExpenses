using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SplitExpenses.Models
{
    public class User
    {
        [BsonIgnoreIfDefault]
        public ObjectId _Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string LevelUser { get; set; }

        public User(string username, string password, string email, string name, DateTime birthday, string gender, string phone, string country, string levelUser)
        {
            Username = username;
            Password = password;
            Email = email;
            Name = name;
            Birthday = birthday;
            Gender = gender;
            Phone = phone;
            Country = country;
            LevelUser = levelUser;
        }
    }

    public class Account
    {
        [BsonIgnoreIfDefault]
        public ObjectId _Id { get; set; }
        public int Id { get; set; }
        public List<string> Users { get; set; }
        public string Name { get; set; }
        public double TotalExpenses { get; set; }
        public double UserExpenses { get; set; }
        public double Balance { get; set; }
    }

    public class Expenses
    {
        [BsonIgnoreIfDefault]
        public ObjectId _Id { get; set; }
        public int FatherId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string PaidBy { get; set; }
        public List<string> PaidFor { get; set; }
        public double Cost { get; set; }

    }
}