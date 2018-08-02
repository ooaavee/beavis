using BeavisCli.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli.Demo.Services
{
    public class UserService
    {
        private static readonly List<UserModel> Repository = new List<UserModel>();

        static UserService()
        {
            Repository.Add(new UserModel { UserName = "Beavis", Password = "Cornholio" });
            Repository.Add(new UserModel { UserName = "Butt-Head", Password = "AC/DC" });
            Repository.Add(new UserModel { UserName = "Daria", Password = "fA-!ddaa-8_#33a_/aaaf(" });
        }

        public UserModel GetUser(string username)
        {
            return Repository.SingleOrDefault(user => user.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<UserModel> GetUsers()
        {
            return Repository;
        }
    }
}
