using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyWeb.DataAccess.Data;
using Humanizer.Localisation.DateToOrdinalWords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
      
        public CompanyController(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
           
        }
        public IActionResult Index()
        {
            var objCompanyList = _unitOfWork.CompanyRepository.GetAll().ToList();
           
            return View(objCompanyList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)

        { 
           
            
            if(id== null|| id == 0)
            {
                //create
             return View(new Company());

            }
            else
            {   //update
                Company companyObj= _unitOfWork.CompanyRepository.Get(u=>u.Id== id);
                return View(companyObj);
            }




        }


        [HttpPost]
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                

                if (companyObj.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(companyObj);
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(companyObj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
               
                return View(companyObj);
            }
        }



        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{

        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? CompanyForDb = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
        //    if (CompanyForDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(CompanyForDb);
        //}


        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Company? obj = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound(id);
        //    }
        //    _unitOfWork.CompanyRepository.Remove(obj);
        //    _unitOfWork.Save();

        //    TempData["success"] = "Category delete successifully";
        //    return RedirectToAction("index");

        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var objCompanyList = _unitOfWork.CompanyRepository.GetAll().ToList();
            return Json(new {data = objCompanyList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objCompanyListDelete = _unitOfWork.CompanyRepository.Get(u  => u.Id == id);
            if (objCompanyListDelete == null)
            {
                return Json(new {success = false,message = "Error while deleting"} );
            }

            
            _unitOfWork.CompanyRepository.Remove(objCompanyListDelete);
            _unitOfWork.Save();
         
            return Json(new { success=true, message= "Delete Successful"});

        }
        #endregion
    }





}
