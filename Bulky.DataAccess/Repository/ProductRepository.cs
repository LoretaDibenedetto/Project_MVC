using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository:Repository<Product>, IProductRepository 
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        

        public void Update(Product objp)
        {
          var objFromDb = _db.Products.FirstOrDefault(u => u.Id == objp.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = objp.Title;
                objFromDb.ISBN = objp.ISBN;
                objFromDb.Price = objp.Price;
                objFromDb.Price50 = objp.Price50;
                objFromDb.Price100 = objp.Price100;
                objFromDb.ListPrice = objp.ListPrice;
                objFromDb.Description = objp.Description;
                objFromDb.CategoryId= objp.CategoryId;
                objFromDb.Author = objp.Author;
                if(objFromDb != null)
                {
                    objFromDb.ImageUrl = objp.ImageUrl; 
                }
            }
        }
    }
}
