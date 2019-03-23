using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            StringBuilder builder2 = new StringBuilder();
            var r2 = new Random();
            //заполняем базу пользователей
            for (int i = 0; i < 20000; i++)
            {
                string email = null;
                var t =Task.Run(() =>
                {
                    for (int ii = 0; ii < 128; ii++)
                    {
                        builder2.Append(array[r2.Next(array.Length)]);
                    }
                    email = builder2.ToString();
                    builder2.Clear();
                });
                
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
                t.Wait();
                manager.AddUser(new User { Id = i, Email = email, Login = login, Phone = phone });
            }

            //var watch1 = Stopwatch.StartNew();
            //var users = manager.Find("aka", 100);
            //watch1.Stop();
            //Console.WriteLine("1 method elapsed " + watch1.ElapsedMilliseconds);

            var watch2 = Stopwatch.StartNew();
            var users2 = manager.Find2("aka", 10);
            watch2.Stop();
            Console.WriteLine("2 method elapsed " + watch2.ElapsedMilliseconds);

            var watch3 = Stopwatch.StartNew();
            var users3 = manager.Find3("aka", 10);
            watch3.Stop();
            Console.WriteLine("3 method elapsed " + watch3.ElapsedMilliseconds);

            var watch4 = Stopwatch.StartNew();
            var users4 = manager.Find4("aka", 10);
            watch4.Stop();
            Console.WriteLine("4 method elapsed " + watch4.ElapsedMilliseconds);

            var watch5 = Stopwatch.StartNew();
            var users5 = manager.Find5("aka", 10);
            watch5.Stop();
            Console.WriteLine("5 method elapsed " + watch5.ElapsedMilliseconds);           

            var watch6 = Stopwatch.StartNew();
            var users6 = manager.Find6("aka", 10);
            watch6.Stop();
            Console.WriteLine("6 method elapsed " + watch6.ElapsedMilliseconds);

            var watch7 = Stopwatch.StartNew();
            var users7 = manager.Find7("aka", 10);
            watch7.Stop();
            Console.WriteLine("7 method elapsed " + watch7.ElapsedMilliseconds);

            Console.ReadKey();
            //var newUser = new User { Id = users.ElementAt(0).Id, Email = "yandex.ru", Login = "Atrium", Phone = "88888888" };
            //manager.UpdateUser(newUser.Id, newUser);
            //foreach (var user in users)
            //{
            //    Console.WriteLine(user.Id + "\t" + user.Login + "\t" + user.Email + "\t" + user.Phone);
            //}
            //Console.ReadKey();
        }
    }
}
