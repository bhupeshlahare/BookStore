﻿using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookWeb.Areas.Admin.Controllers
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
            List<Product> objProducts = _unitOfWork.Product.GetAll().ToList();
            return View(objProducts);
        }
        public IActionResult Create() 
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(U => new SelectListItem
            {
                Text = U.Name,
                Value = U.Id.ToString()
            });

            //ViewBag.CategoryList = CategoryList;
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product obj) 
        {
            if (ModelState.IsValid) 
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
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
            Product? ProductFromDB = _unitOfWork.Product.Get(U=>U.Id == id);
            if (ProductFromDB == null) 
            {
                return NotFound();
            }
            return View(ProductFromDB);
        }
        [HttpPost]
        public IActionResult Edit(Product obj) 
        {
            if (ModelState.IsValid) 
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
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
            Product ProductFromDB = _unitOfWork.Product.Get(U=>U.Id==id);
            if(ProductFromDB == null)
            {
                return NotFound();
            }
            return View(ProductFromDB);
        }
        [HttpPost]
        public IActionResult Delete(Product obj) 
        {
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
