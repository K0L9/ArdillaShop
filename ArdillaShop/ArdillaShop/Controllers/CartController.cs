using ArdillaShop.Data;
using ArdillaShop.Models;
using ArdillaShop.Models.ViewModels;
using ArdillaShop.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArdillaShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id)).Include(x => x.Category);

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Order));
        }
        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCart);
            }

            var objToRemove = shoppingCartList.ElementAt(id);
            shoppingCartList.Remove(objToRemove);
            HttpContext.Session.Set(ENV.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Order()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCart);
            }

            List<int> prductinCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(u => prductinCart.Contains(u.Id));

            ProductUserVM productUserVM = new ProductUserVM()
            {
                AppUser = _db.AppUser.FirstOrDefault(u => u.Id == claims.Value),
                ProductList = productList.ToList()
            };

            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Order")]
        public async Task<IActionResult> OrderPost(ProductUserVM productUserVM)
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString()
                + "OrderConfirmation.html";

            var subject = "Thank you for buy";
            string HtmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            string messageBody = string.Format(
                HtmlBody,
                productUserVM.AppUser.FirstName,
                productUserVM.AppUser.Email,
                productUserVM.ProductList.Select(x => x.Price).Sum().ToString()
                );

            messageBody = IncludeProductsToTable(messageBody, productUserVM.ProductList);

            await _emailSender.SendEmailAsync(ENV.AdminEmail, subject, messageBody);
            await _emailSender.SendEmailAsync(productUserVM.AppUser.Email, subject, messageBody);

            return RedirectToAction(nameof(OrderConfirmation));
        }

        //HACK : Remove this method (implement to prev method)
        private string IncludeProductsToTable(string to, IEnumerable<Product> what)
        {
            StringBuilder builder = new StringBuilder();
            int counter = 1;
            string style = "style=\"border: 1px solid rgba(195, 195, 195, .7); border - collapse: collapse;\"";
            string styleBold = "style=\"border: 1px solid rgba(195, 195, 195, .7); border - collapse: collapse; font-weight: 600;\"";
            foreach (var item in what)
            {
                builder.Append($"<tr><td {style}>{counter}</td><td {style}></td><td>{item.Name}</td><td {style}>Category name</td><td {styleBold}>{item.Price}$</td></tr>");
                counter++;
            }

            int indexToPaste = to.IndexOf("</tr>");
            indexToPaste += "</tr>".Count();
            return to.Insert(indexToPaste, builder.ToString());
        }

        public IActionResult OrderConfirmation()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCart);
            }

            List<int> prductinCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(u => prductinCart.Contains(u.Id));

            ProductUserVM productUserVM = new ProductUserVM()
            {
                AppUser = _db.AppUser.FirstOrDefault(u => u.Id == claims.Value),
                ProductList = productList.ToList()
            };

            HttpContext.Session.Clear();
            return View(productUserVM);
        }
    }
}
