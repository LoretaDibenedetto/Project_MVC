﻿using Bulky.DataAccess.Repository.IRepository;
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
                OrderHeader = new()
            };

          

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
              
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
                inculdeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddess;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;



            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        //[HttpPost]
        //[ActionName("Summary")]
        //public IActionResult SummaryPOST()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
        //        inculdeProperties: "Product");

        //    ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        //    ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

        //    ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);


        //    foreach (var cart in ShoppingCartVM.ShoppingCartList)
        //    {
        //        cart.Price = GetPriceBasedOnQuantity(cart);
        //        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        //    }

        //    if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        //    {
        //        //it is a regular customer 
        //        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //        ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
        //    }
        //    else
        //    {
        //        //it is a company user
        //        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
        //        ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
        //    }
        //    _unitOfWork.OrderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
        //    _unitOfWork.Save();
        //    foreach (var cart in ShoppingCartVM.ShoppingCartList)
        //    {
        //        OrderDetail orderDetail = new()
        //        {
        //            ProductId = cart.ProductId,
        //            OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
        //            Price = cart.Price,
        //            Count = cart.Count
        //        };
        //        _unitOfWork.OrderDetailRepository.Add(orderDetail);
        //        _unitOfWork.Save();
        //    }

        //    if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        //    {
        //        //it is a regular customer account and we need to capture payment
        //        //stripe logic
        //        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        //        var options = new SessionCreateOptions
        //        {
        //            SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
        //            CancelUrl = domain + "customer/cart/index",
        //            LineItems = new List<SessionLineItemOptions>(),
        //            Mode = "payment",
        //        };

        //        foreach (var item in ShoppingCartVM.ShoppingCartList)
        //        {
        //            var sessionLineItem = new SessionLineItemOptions
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = item.Product.Title
        //                    }
        //                },
        //                Quantity = item.Count
        //            };
        //            options.LineItems.Add(sessionLineItem);
        //        }


        //        var service = new SessionService();
        //        Session session = service.Create(options);
        //        _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        //        _unitOfWork.Save();
        //        Response.Headers.Add("Location", session.Url);
        //        return new StatusCodeResult(303);

        //    }

        //    return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        //}



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