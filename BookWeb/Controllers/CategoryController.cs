using Book.Models;
using Book.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Book.DataAccess.Repository.IRepository;

namespace BookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _CategoryRepo;
        public CategoryController(ICategoryRepository db)
        {
            _CategoryRepo = db;
        }

        public IActionResult Index()
        {
            List<Category> objCategories = _CategoryRepo.GetAll().ToList();
            return View(objCategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == "Test" || obj.Name == "test")
            {
                ModelState.AddModelError("name", "Test is an invalid value");
            }
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Category Name and Display order cannot be same");
            }
            if (ModelState.IsValid)
            {
                _CategoryRepo.Add(obj);
                _CategoryRepo.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
           return View();  
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _CategoryRepo.Get(u=>u.Id == id);
            //Category? categoryFromDB = _db.Categories.Find(id);
            //Category? categoryFromDB1 = _db.Categories.FirstOrDefault(U => U.Id == id);
            //Category? categoryFromDB2 = _db.Categories.Where(U=>U.Id == id).FirstOrDefault();
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        [HttpPost]
        public IActionResult Edit(Category obj) 
        {
            if(ModelState.IsValid)
            {
                _CategoryRepo.Update(obj);
                _CategoryRepo.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0)
            { 
                return NotFound(); 
            }

            Category? categoryFromDB = _CategoryRepo.Get(u=> u.Id == id); 
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        [HttpPost]
        public IActionResult Delete(Category obj)
        {
            _CategoryRepo.Remove(obj);
            _CategoryRepo.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
