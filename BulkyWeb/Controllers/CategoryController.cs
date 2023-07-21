using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository db)
        {
            _categoryRepo = db;
        }
        public IActionResult Index()
        {
            var objCategoryList = _categoryRepo.GetAll().ToList();
     
            return View(objCategoryList);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }


        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("", "the DisplayOrder cannot exactly match the Name.");
            }
            if (obj.Name!= null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "test is an invalid value");
            }
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(obj);
                _categoryRepo.Save();
                TempData["success"] = "Category created successifully";
                return RedirectToAction("Index");   
            }
            return View();
        }


        [HttpGet]
        public IActionResult Edit( int? id)
        {

            if(id == null || id == 0 ) 
            {
                return NotFound();
            }
            Category? categoryForDb = _categoryRepo.Get(c => c.Id == id);
            if (categoryForDb == null)
            {
                return NotFound();
            }
            return View(categoryForDb);
        }


        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(obj);
                _categoryRepo.Save();
                TempData["success"] = "Category edit successifully";
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
            Category? categoryForDb = _categoryRepo.Get(c => c.Id == id);
            if (categoryForDb == null)
            {
                return NotFound();
            }
            return View(categoryForDb);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _categoryRepo.Get(c => c.Id == id);
            if(obj == null)
            {
                return NotFound(id);
            }
           _categoryRepo.Remove(obj);
            _categoryRepo.Save();
           
            TempData["success"] = "Category delete successifully";
            return RedirectToAction("index");
          
        }


    }





}
