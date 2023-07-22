using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
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
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var objProductList = _unitOfWork.ProductRepository.GetAll().ToList();
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll().ToList().Select(u => new SelectListItem
            {
                Text = u.Name,  
                Value =u.Id.ToString()
            });
            return View(objProductList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll().ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ViewBag.CategoryList = CategoryList;    
            return View();
        }


        [HttpPost]
        public IActionResult Create(Product obj)
        {
            
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successifully";
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet]
        public IActionResult Edit(int? id)
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


        [HttpPost]
        public IActionResult Edit(Product obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product edit successifully";
                return RedirectToAction("Index");
            }
            return View();
        }

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
