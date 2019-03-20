using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
