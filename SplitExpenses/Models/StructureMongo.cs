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
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string LevelUser { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

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
        public ObjectId Id { get; set; }
        public List<UsersAccount> Users { get; set; }
        public string Name { get; set; }
        [BsonIgnoreIfDefault]
        public double UserExpenses { get; set; }
        public double TotalExpenses { get; set; }

        public Account(List<UsersAccount> users, double totalExpenses)
        {
            Users = users;
            TotalExpenses = totalExpenses;
        }

        public Account(string name, List<UsersAccount> users)
        {
            Users = users;
            Name = name;
        }

        public Account(List<UsersAccount> users, string name, double totalExpenses, double balance)
        {
            Users = users;
            Name = name;
            TotalExpenses = totalExpenses;
        }
    }

    public class UsersAccount
    {
        public string Name { get; set; }
        public bool Owner { get; set; }
        public double Balance { get; set; }
        public int Invitation { get; set; }
    }

    public class Expense
    {
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public ObjectId FatherId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string PaidBy { get; set; }
        public List<string> PaidFor { get; set; }
        public double Cost { get; set; }

        public Expense(ObjectId fatherId, string name, DateTime date, string paidBy, List<string> paidFor, double cost)
        {
            FatherId = fatherId;
            Name = name;
            Date = date;
            PaidBy = paidBy;
            PaidFor = paidFor;
            Cost = cost;
        }

        public Expense(ObjectId fatherId, string name, string category, DateTime date, string paidBy, List<string> paidFor, double cost)
        {
            FatherId = fatherId;
            Name = name;
            Category = category;
            Date = date;
            PaidBy = paidBy;
            PaidFor = paidFor;
            Cost = cost;
        }
    }
}