using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Book.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;

namespace BookWeb.Areas.Admin.Controllers
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
            List<Product> objProducts = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objProducts);
        }
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(U => new SelectListItem
            {
                Text = U.Name,
                Value = U.Id.ToString()
            });

            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };


            if (id == null || id == 0)
            {
                //insert
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(U => U.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.Trim('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(U => new SelectListItem
                {
                    Text = U.Name,
                    Value = U.Id.ToString()
                });

                ProductVM productVM = new()
                {
                    CategoryList = CategoryList,
                    Product = new Product()
                };
                return View(productVM);
            }
        }

        //public IActionResult Create() 
        //{
        //    IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(U => new SelectListItem
        //    {
        //        Text = U.Name,
        //        Value = U.Id.ToString()
        //    });

        //    //ViewBag.CategoryList = CategoryList;

        //    ProductVM productVM = new()
        //    {
        //        CategoryList = CategoryList,
        //        Product = new Product()
        //    };
        //    return View(productVM);
        //}

        //[HttpPost]
        //public IActionResult Create(ProductVM obj) 
        //{
        //    if (ModelState.IsValid) 
        //    {
        //        _unitOfWork.Product.Add(obj.Product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product created successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        //public IActionResult Edit(int? id) 
        //{
        //    if(id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? ProductFromDB = _unitOfWork.Product.Get(U=>U.Id == id);
        //    if (ProductFromDB == null) 
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductFromDB);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product obj) 
        //{
        //    if (ModelState.IsValid) 
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}
        //public IActionResult Delete(int? id)
        //{
        //    if(id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product ProductFromDB = _unitOfWork.Product.Get(U=>U.Id==id);
        //    if(ProductFromDB == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductFromDB);
        //}
        //[HttpPost]
        //public IActionResult Delete(Product obj) 
        //{
        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProducts = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProducts });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id==id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.Trim('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product deleted successfully" });
        }

        #endregion
    }
}
