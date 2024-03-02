using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SKLAD.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SKLAD.Models.ModelsTableDB;
using Microsoft.AspNetCore.Http;

namespace SKLAD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        ClientContext db;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public HomeController(ClientContext cc)
        {
            db = cc;
        }
        public IActionResult Index()
        {
            string type = HttpContext.Session.GetString("TYPE");

            switch (type)
            {
                case "client":
                    return RedirectToAction("PageClient", "Client");
                    break;
                case "employee":
                    return RedirectToAction("PageEmployee", "Employee");
                    break;
                case "admin":
                    return RedirectToAction("PageAdmin", "Admin");
                    break;
            }
           
            return View();
        }

        public IActionResult Login(string login, string password)
        {
            byte[] data = Encoding.Default.GetBytes(password);
            var result = new SHA256Managed().ComputeHash(data);
            string hashPassword = BitConverter.ToString(result).Replace("-", "").ToUpper();

            var findUser = db.Users.Where(p => p.Login == login && p.Password == hashPassword);

            if (!findUser.Any()) return View("ErrorLogin");

            var cur = findUser.First();
            HttpContext.Session.SetString("ID", cur.IdUser.ToString());
            HttpContext.Session.SetString("TYPE", cur.TypeOfAccount);


            if (cur.TypeOfAccount == "client")
            {
                return RedirectToAction("PageClient", "Client");
            }

            if (cur.TypeOfAccount == "employee")
            {
                return RedirectToAction("PageEmployee", "Employee");
            }

            if (cur.TypeOfAccount == "admin")
            {
                return RedirectToAction("PageAdmin", "Admin");
            }
            return View("NoLogin");
        }

        public IActionResult Exit()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
