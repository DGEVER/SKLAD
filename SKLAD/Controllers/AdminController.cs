using SKLAD.Models.ModelsTableDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using TemplateEngine.Docx;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace SKLAD.Controllers
{
    public class AdminController : Controller
    {
        SKLADContext db;

        private readonly ILogger<AdminController> _logger;

        private readonly IWebHostEnvironment _appEnvironment;

        public AdminController(SKLADContext sc, ILogger<AdminController> logger, IWebHostEnvironment environment)
        {
            db = sc;
            _logger = logger;
            _appEnvironment = environment;
        }

        public class VProd
        {
            public string Name { get; set; }
            public string SizeDec { get; set; }
            public string ColorDec { get; set; }
            public string CategoryDec { get; set; }
            public int Count { get; set; }
            public VProd(string name, string sizeDec, string colorDec, string categoryDec, int count)
            {
                Name = name;
                SizeDec = sizeDec;
                ColorDec = colorDec;
                CategoryDec = categoryDec;
                Count = count;
            }
        }

        public class VOrder
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public VOrder(int id, string name, string status)
            {
                Id = id;
                Name = name;
                Status = status;
            }
        }
        public class VSostav
        {
            public string NameP { get; set; }
            public string NameSize { get; set; }
            public string NameColor { get; set; }
            public string NameCategory { get; set; }
            public int Count { get; set; }
            public VSostav(string nameP, string nameSize, string nameColor, string nameCategory, int count)
            {
                NameP = nameP;
                NameSize = nameSize;
                NameColor = nameColor;
                NameCategory = nameCategory;
                Count = count;
            }
        }

        public class VAttr
        {
            public int ID { get; set; }
            public string Descr { get; set; }
            public VAttr(int id, string descr)
            {
                ID = id;
                Descr = descr;
            }
        }

        public class VChangeLog
        {
            public string NameStatus { get; set; }
            public DateTime Date { get; set; }

            public VChangeLog(string name, DateTime date)
            {
                Date = date;
                NameStatus = name;
            }
        }

        public class VProd2
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string SizeDec { get; set; }
            public string ColorDec { get; set; }
            public string CategoryDec { get; set; }
            public int Count { get; set; }
            public VProd2(int id, string name, string sizeDec, string colorDec, string categoryDec, int count)
            {
                ID = id;
                Name = name;
                SizeDec = sizeDec;
                ColorDec = colorDec;
                CategoryDec = categoryDec;
                Count = count;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PageAdmin()
        {
            string type = HttpContext.Session.GetString("TYPE");
            _logger.LogInformation("Открытие страницы админстратора");
            if (type != "admin") RedirectToAction("Index", "Home");
            return View();
        }

        public IActionResult ShowCatalog()
        {
            var prod = from Product in db.Products
                       join Size in db.Sizes on Product.IdSizeNavigation.IdSize equals Size.IdSize
                       join Color in db.Colors on Product.IdColorNavigation.IdColor equals Color.IdColor
                       join Category in db.Categories on Product.IdCategoriesNavigation.IdCategories equals Category.IdCategories
                       select new
                       {
                           Name = Product.ProductName,
                           SizeDec = Size.SizeDescription,
                           ColorDec = Color.ColorDescription,
                           CategoryDec = Category.CategoriesDescription,
                           Count = Product.Amount
                       };

            System.IO.File.Delete("OutputFile.docx");
            System.IO.File.Copy("ShowCatalog.docx", "OutputFile.docx");
            TableContent table = new TableContent();
            table.Name = "table";
            int n = 0;

            List<VProd> products = new List<VProd>();
            foreach (var p in prod)
            {
                products.Add(new VProd(p.Name, p.SizeDec, p.ColorDec, p.CategoryDec, p.Count));
                n++;
                table.AddRow(new FieldContent("number", n.ToString()),
                    new FieldContent("nameProduct", p.Name),
                    new FieldContent("nameCategory", p.CategoryDec),
                    new FieldContent("nameSize", p.SizeDec),
                    new FieldContent("nameColor", p.ColorDec),
                    new FieldContent("count", p.Count.ToString())
                    );
            }

            //Подготовка файла

            var valuesToFile = new Content(table);

            using (var ouputDocumet = new TemplateProcessor("OutputFile.docx").SetRemoveContentControls(true))
            {
                ouputDocumet.FillContent(valuesToFile);
                ouputDocumet.SaveChanges();
            }

            //Конец подкотовки файла


            ViewBag.Products = products;
            _logger.LogInformation("Открытие страницы просмотра каталога");
            return View();
        }
        public IActionResult Search(string nameProduct)
        {
            var prod = from Product in db.Products
                       join Size in db.Sizes on Product.IdSizeNavigation.IdSize equals Size.IdSize
                       join Color in db.Colors on Product.IdColorNavigation.IdColor equals Color.IdColor
                       join Category in db.Categories on Product.IdCategoriesNavigation.IdCategories equals Category.IdCategories
                       select new
                       {
                           Name = Product.ProductName,
                           SizeDec = Size.SizeDescription,
                           ColorDec = Color.ColorDescription,
                           CategoryDec = Category.CategoriesDescription,
                           Count = Product.Amount
                       };
            prod = prod.Where(x => EF.Functions.Like(x.Name, nameProduct + '%'));
            List<VProd> products = new List<VProd>();
            foreach (var p in prod)
            {
                products.Add(new VProd(p.Name, p.SizeDec, p.ColorDec, p.CategoryDec, p.Count));
            }
            ViewBag.Products = products;
            return View("ShowCatalog");
        }

        public IActionResult ShowOrders()
        {

            var orders = from Order in db.Orders
                         join Client in db.Clients on Order.IdClientNavigation.IdClient equals Client.IdClient
                         join Status in db.Statuses on Order.IdStatusNavigation.IdStatus equals Status.IdStatus
                         orderby Order.DateTime descending
                         select new
                         {
                             ID = Order.IdOrder,
                             NameClient = Client.FirstName + ' ' + Client.LastName,
                             Status = Status.StatusDescription
                         };
            List<VOrder> or = new List<VOrder>();
            foreach (var order in orders)
            {
                or.Add(new VOrder(order.ID, order.NameClient, order.Status));
            }

            ViewBag.Orders = or;
            _logger.LogInformation("Открытие страницы просмотра заказов");
            return View();
        }

        public IActionResult ShowAboutOrder([FromQuery(Name = "id_order")] int id_order)
        {
            var sostavOrder = from OrderDetail in db.OrderDetails
                              join Product in db.Products on OrderDetail.IdProductNavigation.IdProduct equals Product.IdProduct
                              join Size in db.Sizes on Product.IdSizeNavigation.IdSize equals Size.IdSize
                              join Color in db.Colors on Product.IdColorNavigation.IdColor equals Color.IdColor
                              join Category in db.Categories on Product.IdCategoriesNavigation.IdCategories equals Category.IdCategories
                              where OrderDetail.IdOrder == id_order
                              select new
                              {
                                  NameP = Product.ProductName,
                                  NameSize = Size.SizeDescription,
                                  NameColor = Color.ColorDescription,
                                  NameCaregory = Category.CategoriesDescription,
                                  Count = OrderDetail.Amount
                              };

            var changeLog = from StatusChangeLog in db.StatusChangeLogs
                            join Status in db.Statuses on StatusChangeLog.IdStatusNavigation.IdStatus equals Status.IdStatus
                            where StatusChangeLog.IdOrder == id_order
                            select new
                            {
                                Date = StatusChangeLog.DateTime,
                                NameStatus = Status.StatusDescription
                            };

            string nameStatus = (from Order in db.Orders
                                 join Status in db.Statuses on Order.IdStatusNavigation.IdStatus equals Status.IdStatus
                                 where Order.IdOrder == id_order
                                 select Status.StatusDescription).First();
            System.IO.File.Delete("OutputFile.docx");
            System.IO.File.Copy("ShowAboutOrder.docx", "OutputFile.docx");
            TableContent table = new TableContent();
            table.Name = "table";
            int n = 0;

            List<VChangeLog> vcl = new List<VChangeLog>();

            List<VSostav> vs = new List<VSostav>();
            foreach (var v in sostavOrder)
            {
                vs.Add(new VSostav(v.NameP, v.NameSize, v.NameColor, v.NameCaregory, v.Count));
                n++;
                table.AddRow(new FieldContent("number", n.ToString()),
                    new FieldContent("nameProduct", v.NameP),
                    new FieldContent("nameCategory", v.NameCaregory),
                    new FieldContent("nameSize", v.NameSize),
                    new FieldContent("nameColor", v.NameColor),
                    new FieldContent("count", v.Count.ToString())
                    );
            }


            foreach (var v in changeLog)
            {
                vcl.Add(new VChangeLog(v.NameStatus, v.Date));
            }
            string date = DateTime.Now.ToShortDateString();
            var valuesToFile = new Content(new FieldContent("status", nameStatus),
                table,
                new FieldContent("date", date));

            using (var ouputDocumet = new TemplateProcessor("OutputFile.docx").SetRemoveContentControls(true))
            {
                ouputDocumet.FillContent(valuesToFile);
                ouputDocumet.SaveChanges();
            }

            ViewBag.ChangeLog = vcl;
            ViewBag.VS = vs;
            ViewBag.id_order = id_order;
            _logger.LogInformation("Открытие страницы просмотра данных о заказе");
            return View();
        }

        public IActionResult AddNewEmployee1()
        {
            _logger.LogInformation("Открытие страницы для добавления нового работника");
            return View();
        }

        public IActionResult AddNewEmployee2(string login, string password)
        {
            _logger.LogInformation("Открытие страницы для добавления нового работника");
            byte[] data = Encoding.Default.GetBytes(password);
            var result = new SHA256Managed().ComputeHash(data);
            string hashPassword = BitConverter.ToString(result).Replace("-", "").ToUpper();

            User user = new User();
            user.Login = login;
            user.Password = hashPassword;
            user.TypeOfAccount = "employee";

            var t = db.Database.BeginTransaction();
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
                t.Commit();
                _logger.LogInformation("Успех!");
            }
            catch (Exception ex)
            {
                t.Rollback();
                _logger.LogError("Ошибка!" + ex.Message);
            }
            return RedirectToAction("PageAdmin", "Admin");
        }

        public IActionResult AddProduct1()
        {
            var colors = from Color in db.Colors select Color;
            var size = from Size in db.Sizes select Size;
            var categoryes = from Category in db.Categories select Category;

            List<VAttr> color = new List<VAttr>();
            List<VAttr> s = new List<VAttr>();
            List<VAttr> cat = new List<VAttr>();

            foreach(var co in colors)
            {
                color.Add(new VAttr(co.IdColor, co.ColorDescription));
            }

            foreach(var si in size)
            {
                s.Add(new VAttr(si.IdSize, si.SizeDescription));
            }

            foreach(var c in categoryes)
            {
                cat.Add(new VAttr(c.IdCategories, c.CategoriesDescription));
            }

            ViewBag.Color = color;
            ViewBag.Size = s;
            ViewBag.Category = cat;
            _logger.LogInformation("Открытие страницы для добавления нового товара");
            return View();
        }

        public IActionResult AddProduct2(int id_category, int id_color, int id_size, string name, int count)
        {
            _logger.LogInformation("Открытие страницы для добавления нового товара");
            Product p = new Product();
            p.IdCategories = id_category;
            p.ProductName = name;
            p.Amount = count;
            p.IdColor = id_color;
            p.IdSize = id_size;
            var t = db.Database.BeginTransaction();
            try
            {
                db.Products.Add(p);
                db.SaveChanges();
                t.Commit();
                _logger.LogInformation("Успех!");
            }
            catch (Exception ex)
            {
                t.Rollback();
                _logger.LogError("Ошибка!" + ex.Message);
            }
            return RedirectToAction("PageAdmin", "Admin");
        }

        public IActionResult ChangeStatus1(int id_order)
        {
            var status = from Status in db.Statuses select Status;
            List<Status> statusList = status.ToList();
            ViewBag.Status = statusList;
            ViewBag.id_order = id_order;
            _logger.LogInformation("Открытие страницы для изменения статуса заказа");
            return View();
        }

        public IActionResult ChangeStatus2(int id_status, int id_order)
        {
            _logger.LogInformation("Открытие страницы для изменения статуса заказа");
            var curOrders = from Order in db.Orders where Order.IdOrder == id_order select Order;
            var curOrder = curOrders.First();
            curOrder.IdStatus = id_status;
            var t = db.Database.BeginTransaction();
            try
            {
                db.Orders.Update(curOrder);
                db.SaveChanges();
                t.Commit();
                _logger.LogInformation("Успех!");
            }
            catch(Exception ex)
            {
                t.Rollback();
                _logger.LogError("Ошибка!" + ex.Message);
            }
            return RedirectToAction("PageAdmin", "Admin");
        }

        public IActionResult ChangeCountProduct1()
        {
            var prod = from Product in db.Products
                       join Size in db.Sizes on Product.IdSizeNavigation.IdSize equals Size.IdSize
                       join Color in db.Colors on Product.IdColorNavigation.IdColor equals Color.IdColor
                       join Category in db.Categories on Product.IdCategoriesNavigation.IdCategories equals Category.IdCategories
                       select new
                       {
                           ID = Product.IdProduct,
                           Name = Product.ProductName,
                           SizeDec = Size.SizeDescription,
                           ColorDec = Color.ColorDescription,
                           CategoryDec = Category.CategoriesDescription,
                           Count = Product.Amount
                       };
            List<VProd2> products = new List<VProd2>();
            foreach (var p in prod)
            {
                products.Add(new VProd2(p.ID, p.Name, p.SizeDec, p.ColorDec, p.CategoryDec, p.Count));
            }
            ViewBag.Products = products;
            _logger.LogInformation("Открытие страницы для изменения количества товара");
            return View();
        }

        public IActionResult ChangeCountProduct2([FromQuery(Name = "id_product")] int id_product)
        {
            _logger.LogInformation("Открытие страницы для изменения количества товара");
            ViewBag.ID = id_product;
            return View();
        }

        public IActionResult ChangeCountProduct3(int id_product, int count)
        {
            _logger.LogInformation("Открытие страницы для изменения количества товара");
            var product = db.Products.Where(p => p.IdProduct == id_product).FirstOrDefault();
            product.Amount = count;

            var t = db.Database.BeginTransaction();
            try
            {
                db.Products.Update(product);
                db.SaveChanges();
                t.Commit();
                _logger.LogInformation("Успех!");
            }
            catch (Exception ex)
            {
                t.Rollback();
                _logger.LogError("Ошибка!" + ex.Message);
            }
            ViewBag.ID = id_product;
            return RedirectToAction("ShowCatalog", "Admin");
        }

        public IActionResult GetFileClient()
        {
            string file_path = Path.Combine(_appEnvironment.ContentRootPath, "OutputFile.docx");
            string file_type = "application/docx";
            string file_name = "OutputFile.docx";

            _logger.LogInformation("Скачивание файла");
            return PhysicalFile(file_path, file_type, file_name);
        }
    }
}
