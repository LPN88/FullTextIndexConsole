using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace FullTextIndexConsole
{
    public class UserManager
    {
        private readonly DataBaseContext _db;

        public UserManager()
        {
            _db = new DataBaseContext();
        }

        public void AddUser(User user)
        {
            _db.AddUser(user);
        }

        public void UpdateUser(int userId, User User)
        {
            _db.UpdateUser(userId,User);
        }

        public List<User> Find(string filter, int limit)
        {           
            return _db.Find(filter, limit);
        }

        public List<User> Find2(string filter, int limit)
        {
            return _db.Find2(filter, limit);
        }

        public List<User> Find3(string filter, int limit)
        {
            return _db.Find3(filter, limit);
        }

        public List<User> Find4(string filter, int limit)
        {
            return _db.Find4(filter, limit);
        }

        public List<User> Find5(string filter, int limit)
        {
            return _db.Find5(filter, limit);
        }

        public List<User> Find6(string filter, int limit)
        {
            return _db.Find6(filter, limit);
        }

        public List<User> Find7(string filter, int limit)
        {
            return _db.Find7(filter, limit);
        }
    }
}
