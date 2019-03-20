using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextIndexConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //массив допустимых символов
            char[] array = (new List<int[]> { new[] { 32, 32 }, new[] { 48, 57 }, new[] { 97, 122 } })
                   .SelectMany<int[], char>((ar) =>
                   {
                       List<char> dd = new List<char>();
                       for (int ii = ar[0]; ii <= ar[1]; ii++)
                       {
                           dd.Add((char)ii);
                       };
                       return dd;
                   }).ToArray();
            var manager = new UserManager();
            StringBuilder builder = new StringBuilder();
            var r = new Random();
            //заполняем базу пользователей
            for (int i = 0; i < 10000; i++)
            {
                for (int ii = 0; ii < 128; ii++)
                {
                    builder.Append(array[r.Next(array.Length)]);
                }
                var email = builder.ToString();
                builder.Clear();
                for (int ii = 0; ii < 64; ii++)
                {
                    builder.Append(array[r.Next(array.Length)]);
                }
                var login = builder.ToString();
                builder.Clear();
                for (int ii = 0; ii < 16; ii++)
                {
                    builder.Append(array[r.Next(array.Length)]);
                }
                var phone = builder.ToString();
                builder.Clear();
                manager.AddUser(new User { Id = i, Email = email, Login = login, Phone = phone });
            }

            var users = manager.Find("aka", 100);
            var newUser = new User { Id = users.ElementAt(0).Id, Email = "yandex.ru", Login = "Atrium", Phone = "88888888" };
            manager.UpdateUser(newUser.Id, newUser);
            foreach (var user in users)
            {
                Console.WriteLine(user.Id + "\t" + user.Login + "\t" + user.Email + "\t" + user.Phone);
            }
            Console.ReadKey();
        }
    }
}
