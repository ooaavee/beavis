using BeavisCli.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli.Demo.Services
{
    public class DemoUserRepository
    {
        private readonly List<UserModel> _db = new List<UserModel>();

        public DemoUserRepository()
        {
            _db.Add(new UserModel { UserName = "Beavis", Password = "Cornholio" });
            _db.Add(new UserModel { UserName = "Butt-Head", Password = "AC/DC" });
            _db.Add(new UserModel { UserName = "Daria", Password = "fA-!ddaa-8_#33a_/aaaf(" });
        }

        public UserModel GetUser(string username)
        {
            return _db.SingleOrDefault(user => user.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<UserModel> GetUsers()
        {
            return _db;
        }
    }
}
