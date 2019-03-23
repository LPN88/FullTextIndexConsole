using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace FullTextIndexConsole
{
    //БД использующая полнотекстовый индекс для поиска
    public class DataBaseContext
    {
        //список пользователей
        private static readonly SortedSet<User> _users = new SortedSet<User>(new UserComparer());
        //словарь с полнотекстовым индексом в качестве ключа
        private static readonly Dictionary<string,List<int>> _ftStorage = new Dictionary<string, List<int>>();

        static object locker = new object();
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

        public Dictionary<string, List<int>> FtStorage { get { return _ftStorage; } }

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
            List<Task<List<int>>> tasks = new List<Task<List<int>>>(); 
           
            int chunkSize = 500;
            int chunks = _ftStorage.Count() / chunkSize;
            for (int i = 0; i < chunks; i++)
            {
                int chunkStart = i * chunkSize;
                int chunkEnd = chunkStart + chunkSize;
                tasks.Add(Task.Run<List<int>>(() =>
                {
                    List<int> ar = new List<int>();
                    for (int j = chunkStart; j < chunkEnd; j++)
                    {
                        if (_ftStorage.ElementAt(j).Key.Contains(filter))
                        {
                            ar.AddRange(_ftStorage.ElementAt(j).Value);
                        }
                        //foreach (var pair in _ftStorage.Where(k => k.Key.Contains(filter)))
                        //{
                        //    ar.AddRange(pair.Value.Select(x => x));
                        //}
                    }
                    return ar;
                }));                
            }
            Task.WhenAll(tasks);            
            return _users.Join(tasks.SelectMany(t => t.Result), u => u.Id, ft => ft, (u, ft) => u).Take(limit).ToList();
                               
        }

        public List<User> Find2(string filter, int limit)
        {
            return _users.Join(_ftStorage.Where(k => k.Key.Contains(filter)).SelectMany(k => k.Value), u => u.Id, ft => ft, (u, ft) => u).Take(limit).ToList();
        }

        public List<User> Find3(string filter, int limit)
        {
            ConcurrentQueue<int> idUser = new ConcurrentQueue<int>();
            //int counter = 0;
            Parallel.ForEach(_ftStorage, (pair, loopState) =>
            {
                if (pair.Key.Contains(filter))
                {
                    //lock (locker)
                    //{
                    foreach (var id in pair.Value)
                    {
                        idUser.Enqueue(id);
                    }
                    //}
                }
            }
            );
            return _users.Join(idUser, u => u.Id, ft => ft, (u, ft) => u).Take(limit).ToList();
        }

        public List<User> Find4(string filter, int limit)
        {
            ConcurrentQueue<int> idUser = new ConcurrentQueue<int>();
            foreach (var pair in _ftStorage.AsParallel().Where(k => k.Key.Contains(filter)))
            {
                foreach (var id in pair.Value)
                {
                    idUser.Enqueue(id);
                }
            }
            return _users.Join(idUser, u => u.Id, ft => ft, (u, ft) => u).Take(limit).ToList();
        }

        public List<User> Find5(string filter, int limit)
        {
            
            return _users.Where(u=>u.Email.Contains(filter) || u.Login.Contains(filter) || u.Phone.Contains(filter)).Take(limit).ToList();
        }

        public List<User> Find6(string filter, int limit)
        {
            ConcurrentQueue<User> idUser = new ConcurrentQueue<User>();           
            Parallel.ForEach(_users, (u, loopState) =>
            {
                if (u.Email.Contains(filter) || u.Login.Contains(filter) || u.Phone.Contains(filter))
                {
                    idUser.Enqueue(u);                 
                }
            }
            );
            return idUser.OrderBy(u=>u.Id).Take(limit).ToList();
        }

        public List<User> Find7(string filter, int limit)
        {
            ConcurrentQueue<User> idUser = new ConcurrentQueue<User>();            
            int c = 0;
            Parallel.ForEach(_users, (u, loopState) =>
            {
                if (u.Email.Contains(filter) || u.Login.Contains(filter) || u.Phone.Contains(filter))
                {                   
                    idUser.Enqueue(u);
                    ++c;
                    if (c >= limit)
                    {
                        loopState.Break();
                    }
                }
            }
            );
            return idUser.OrderBy(u => u.Id).Take(limit).ToList();
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
