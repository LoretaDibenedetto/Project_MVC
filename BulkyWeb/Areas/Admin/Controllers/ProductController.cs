using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using BulkyWeb.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var objProductList = _unitOfWork.ProductRepository.GetAll().ToList();
           
            return View(objProductList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)

        { 
            ProductVM productVM = new()
            {
                CategoryList =_unitOfWork.CategoryRepository.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if(id== null|| id == 0)
            {
                //create
             return View(productVM);

            }
            else
            {   //update
                productVM.Product = _unitOfWork.ProductRepository.Get(u=>u.Id== id);
                return View(productVM);
            }




        }


        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRoutePath = _webHostEnvironment.WebRootPath;
                if(file!= null)
                {
                    string fileName = Guid.NewGuid().ToString() +Path.GetExtension(file.FileName);    
                    string productPath = Path.Combine(wwwRoutePath, @"images\product");

                    using(var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product" + fileName;
                }
                _unitOfWork.ProductRepository.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Category created successifully";
                return RedirectToAction("Index");
            }
            else
            {
     
                    productVM.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                return View(productVM);

            
            }
        }


        //[HttpGet]
        //public IActionResult Edit(int? id)
        //{

        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? ProductForDb = _unitOfWork.ProductRepository.Get(c => c.Id == id);
        //    if (ProductForDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductForDb);
        //}


        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.ProductRepository.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product edit successifully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        [HttpGet]
        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? ProductForDb = _unitOfWork.ProductRepository.Get(c => c.Id == id);
            if (ProductForDb == null)
            {
                return NotFound();
            }
            return View(ProductForDb);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unitOfWork.ProductRepository.Get(c => c.Id == id);
            if (obj == null)
            {
                return NotFound(id);
            }
            _unitOfWork.ProductRepository.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Category delete successifully";
            return RedirectToAction("index");

        }


    }





}
