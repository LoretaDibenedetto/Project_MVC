using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Costumer.Controllers{  
    [Area("costumer")]
       [Authorize]
    public class CardController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }  
        public CardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
         
        }


        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
                inculdeProperties: "Product"),
            
            };

          

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
              
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }


        public IActionResult Summary()
        {
            return View();
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;

            }else
            {
                if(shoppingCart.Count <= 1000)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart

                _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
              
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);

            _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);

           
              
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
