
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Costumer.Controllers
{
    [Area("Costumer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(inculdeProperties: "Category");
            return View(productList);
        }



        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.ProductRepository.Get(u => u.Id == id, inculdeProperties: "Category"),
                Count = 1,
                ProductId = id

           };

       
            return View(cart);
        }




        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)


        {
            shoppingCart.Id = 0;
          
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;    

            ShoppingCart cardFromDb = _unitOfWork.ShoppingCartRepository.Get
                (u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
              
            if (cardFromDb !=null) 
            {
                cardFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartRepository.Update(cardFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
            }

            TempData["success"] = "Cart updated successifully";
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
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