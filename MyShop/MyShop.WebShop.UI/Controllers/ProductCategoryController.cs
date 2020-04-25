using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebShop.UI.Controllers
{
    public class ProductCategoryController : Controller
    {
        ProductCategoryRepository dataContext;
        public ProductCategoryController()
        {
            this.dataContext = new ProductCategoryRepository();
        }
        // GET: ProductCategory
        public ActionResult Index()
        {
            List<ProductCategory> productCategories = dataContext.Collection().ToList();
            return View(productCategories);
        }

        public ActionResult Create()
        {
            ProductCategory productCategory = new ProductCategory();
            return View(productCategory);
        }

        [HttpPost]
        public ActionResult Create(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }
            else
            {
                dataContext.Insert(productCategory);
                dataContext.Commit();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string id)
        {
            ProductCategory productCategoryToEdit = dataContext.Find(id);
            if (productCategoryToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCategoryToEdit);
            }
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory productCategory, string id)
        {
            ProductCategory productCategoryToEdit = dataContext.Find(id);
            if (productCategoryToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                productCategoryToEdit.Category = productCategory.Category;

                dataContext.Update(productCategoryToEdit);
                dataContext.Commit();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(string id)
        {
            ProductCategory productCategory = dataContext.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCategory);
            }
        }

        public ActionResult Delete(string id)
        {
            ProductCategory productCategoryToDelete = dataContext.Find(id);
            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCategoryToDelete);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string id)
        {
            ProductCategory productCategoryToDelete = dataContext.Find(id);
            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                dataContext.Delete(id);
                dataContext.Commit();
                return RedirectToAction("Index");
            }
        }
    }
}