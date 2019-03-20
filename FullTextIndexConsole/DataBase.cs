using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextIndexConsole
{
    //БД использующая полнотекстовый индекс для поиска
    public class DataBaseContext
    {
        //список пользователей
        private static readonly SortedSet<User> _users = new SortedSet<User>(new UserComparer());
        //словарь с полнотекстовым индексом в качестве ключа
        private static readonly SortedDictionary<string,List<int>> _ftStorage = new SortedDictionary<string, List<int>>();
        //добавляем ключевое слово в словарь
        private void AddOrUpdateIndex(string key, int value)
        {
            if (_ftStorage.ContainsKey(key))
            {
                _ftStorage[key].Add(value);
            }
            else
            {
                _ftStorage.Add(key, new List<int> { value });
            }
        }
        //удаляем ключевое слово или связанного пользователя
        private void RemoveIndex(string key, int value)
        {
            if (_ftStorage.ContainsKey(key))
            {
                _ftStorage[key].Remove(value);
                if (_ftStorage[key].Count() == 0)
                {
                    _ftStorage.Remove(key);
                }
            }
        }

        public SortedSet<User> Users { get { return _users; }  }
                
        public bool AddUser(User user)
        {
            if (_users.FirstOrDefault(x=> x.Id == user.Id)!=null)
            {
                Console.WriteLine("Пользователь с таким ИД уже существует в бд!");
                return false;
            }
            else
            {
                _users.Add(user);
                AddOrUpdateIndex(user.Email, user.Id);
                AddOrUpdateIndex(user.Login, user.Id);
                AddOrUpdateIndex(user.Phone, user.Id);
                return true;
            }
           
        }

        public void UpdateUser(int userId, User user)
        {
            var dbUser = _users.FirstOrDefault(u => u.Id == userId);
            if (dbUser!=null)
            {
                RemoveIndex(dbUser.Email, userId);
                RemoveIndex(dbUser.Login, userId);
                RemoveIndex(dbUser.Phone, userId);
                dbUser.Email = user.Email;
                dbUser.Login = user.Login;
                dbUser.Phone = user.Phone;
            }
            else
            {
                Console.WriteLine("Пользователя с таким ИД не существует в бд!");
            }
        }

        public List<User> Find(string filter, int limit)
        {
            //return _ftStorage.Where(k => k.Key.Contains(filter)).SelectMany(k => k.Value).Join(_users, ft => ft, u => u.Id, (ft, u) => u).Take(limit).ToList();
            return _users.Join(_ftStorage.Where(k => k.Key.Contains(filter)).SelectMany(k => k.Value), u => u.Id, ft => ft, (u,ft) => u).Take(limit).ToList();
        }
    }

    //Компаратор для правильного сохранения в упорядоченной коллекции
    public class UserComparer : IComparer<User>
    {
        public int Compare(User a, User b)
        {
            //var one = (User)a;
            //var two = (User)b;
            if (a.Id > b.Id)
                return 1;
            if (a.Id < b.Id)
                return -1;
            else
                return 0;
        }
    }
}
