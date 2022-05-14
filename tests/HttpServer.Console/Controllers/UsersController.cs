using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Console.Controllers
{
    public record User(string Name, string Login);
    public class UsersController : IController
    {
        public async Task<User[]> Index()
        {
            await Task.Delay(50);
            return new User[]
            {
                new User("Stepfan", "Gttb"),
                new User("Alex", "Fr-15"),
                new User("Eve", "Labean")
            };
        }
    }
}
