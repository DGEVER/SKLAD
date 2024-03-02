using SKLAD.Models.ModelsTableDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SKLAD.Controllers
{
    public class ClientController : Controller
    {
        ClientContext db;
        private readonly ILogger<ClientController> _logger;
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

        public class VProdNewOrder
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SizeDec { get; set; }
            public string ColorDec { get; set; }
            public string CategoryDec { get; set; }
            public int Count { get; set; }
            public VProdNewOrder(int id, string name, string sizeDec, string colorDec, string categoryDec, int count)
            {
                Id = id;
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
        public ClientController(ClientContext cc, ILogger<ClientController> logger)
        {
            db = cc;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PageClient()
        {
            string type = HttpContext.Session.GetString("TYPE");
            if (type != "client") RedirectToAction("Index", "Home");
            _logger.LogInformation("Открытие страницы клиента");
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
            List<VProd> products = new List<VProd>();
            foreach(var p in prod)
            {
                products.Add(new VProd(p.Name, p.SizeDec, p.ColorDec, p.CategoryDec, p.Count));
            }
            ViewBag.Products = products;
            _logger.LogInformation("Открытие страницы просмотра каталога");
            return View();
        }

        public IActionResult ShowOrders()
        {
            var client = from Client in db.Clients where Client.IdUser == Convert.ToInt32(HttpContext.Session.GetString("ID")) select Client.IdClient;
            int id_client = client.First();
            var orders = from Order in db.Orders
                         join Client in db.Clients on Order.IdClientNavigation.IdClient equals Client.IdClient
                         join Status in db.Statuses on Order.IdStatusNavigation.IdStatus equals Status.IdStatus
                         where Client.IdClient == id_client
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
            List<VChangeLog> vcl = new List<VChangeLog>();
            List < VSostav > vs = new List<VSostav>();
            foreach (var v in sostavOrder)
            {
                vs.Add(new VSostav(v.NameP, v.NameSize, v.NameColor, v.NameCaregory, v.Count));
            }

            foreach (var v in changeLog)
            {
                vcl.Add(new VChangeLog(v.NameStatus, v.Date));
            }

            ViewBag.ChangeLog = vcl;
            ViewBag.VS = vs;
            ViewBag.id_order = id_order;
            _logger.LogInformation("Открытие страницы просмотра данных о заказе");
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

        public IActionResult AddOrder1()
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
            List<VProdNewOrder> products = new List<VProdNewOrder>();

            foreach(var p in prod)
            {
                products.Add(new VProdNewOrder(p.ID, p.Name, p.SizeDec, p.ColorDec, p.CategoryDec, p.Count));
            }

            ViewBag.Product = products;
            _logger.LogInformation("Открытие страницы добавления заказа");
            return View();
        }
        
        public IActionResult AddOrder2(int id_product, int count)
        {
            _logger.LogInformation("Открытие страницы добавления заказа");
            Order order = new Order();
            order.DateTime = DateTime.Now;
            order.IdStatus = 1;
            order.IdClient = Convert.ToInt32(HttpContext.Session.GetString("ID"));
            var t = db.Database.BeginTransaction();
            int id_order = -1;
            try
            {
                db.Orders.Add(order);

                db.SaveChanges();
                //int id_order = db.Orders.Last().OrderBy().IdOrder;
                id_order = db.Orders.OrderByDescending(p => p.IdOrder).First().IdOrder;

                OrderDetail detail = new OrderDetail();
                detail.IdOrder = id_order;
                detail.IdProduct = id_product;
                detail.Amount = count;

                db.OrderDetails.Add(detail);

                db.SaveChanges();
                t.Commit();
                _logger.LogInformation("Успех!");
            }
            catch (Exception ex)
            {
                t.Rollback();
                _logger.LogError("Ошибка!" + ex.Message);
            }
            ViewBag.id_order = id_order;
            return View();
        }

        public IActionResult AddOrder3(int id_order)
        {
            _logger.LogInformation("Открытие страницы добавления заказа");
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
            List<VProdNewOrder> products = new List<VProdNewOrder>();

            foreach (var p in prod)
            {
                products.Add(new VProdNewOrder(p.ID, p.Name, p.SizeDec, p.ColorDec, p.CategoryDec, p.Count));
            }

            ViewBag.Product = products;
            ViewBag.id_order = id_order;
            return View();
        }

        public IActionResult AddOrder4(int id_product, int id_order, int count)
        {
            _logger.LogInformation("Открытие страницы добавления заказа");
            OrderDetail detail = new OrderDetail();
            detail.IdOrder = id_order;
            detail.IdProduct = id_product;
            detail.Amount = count;
            var t = db.Database.BeginTransaction();
            try
            {
                db.OrderDetails.Add(detail);

                db.SaveChanges();
                t.Commit();
                _logger.LogInformation("Успех!");
            }
            catch (Exception ex)
            {
                t.Rollback();
                _logger.LogError("Ошибка!"+ ex.Message);
            }
            ViewBag.id_order = id_order;
            return View("~/Views/Client/AddOrder2.cshtml");
        }

        public IActionResult Otmena(int id_order)
        {
            _logger.LogInformation("Открытие страницы отмены заказа");
            var curOrder = from Order in db.Orders where Order.IdOrder == id_order select Order;

            Order or = new Order();

            var order = curOrder.FirstOrDefault();
            order.IdStatus = 7;
            or.IdOrder = order.IdOrder;
            or.DateTime = order.DateTime;
            or.IdClient = order.IdClient;
            or.IdStatus = 7;
            var t = db.Database.BeginTransaction();
            try
            {
                db.Orders.Update(order);
                db.SaveChanges();
                t.Commit();
                _logger.LogInformation("Успех!");
            }
            catch(Exception ex)
            {
                t.Rollback();
                _logger.LogError("Ошибка!" + ex.Message);
            }
            return RedirectToAction("PageClient", "Client");
        }
    }
}
