using MyShop.Core.Models;
using MyShop.Core.ViewModel;
using MyShop.DataAccess.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebShop.UI.Controllers
{
    public class ProductController : Controller
    {
        ProductRepository dataContext;
        ProductCategoryRepository categoryContext;
        public ProductController()
        {
            this.dataContext = new ProductRepository();
            this.categoryContext = new ProductCategoryRepository();
        }
        // GET: Product
        public ActionResult Index()
        {
            List<Product> products = dataContext.Collection().ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Product = new Product();
            viewModel.Categories = categoryContext.Collection();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Product product)
        {
            if(!ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                dataContext.Insert(product);
                dataContext.Commit();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string id)
        {
            Product productToEdit = dataContext.Find(id);
            if(productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                ProductViewModel viewModel = new ProductViewModel();
                viewModel.Product = productToEdit;
                viewModel.Categories = categoryContext.Collection();
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Edit(Product product, string id)
        {
            Product productToEdit = dataContext.Find(id);
            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                productToEdit.Name = product.Name;
                productToEdit.Description = product.Description;
                productToEdit.Category = product.Category;
                productToEdit.Price = product.Price;
                productToEdit.Image = product.Image;

                dataContext.Update(productToEdit);
                dataContext.Commit();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(string id)
        {
            Product product = dataContext.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }

        public ActionResult Delete(string id)
        {
            Product productToDelete = dataContext.Find(id);
            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productToDelete);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string id)
        {
            Product productToDelete = dataContext.Find(id);
            if (productToDelete == null)
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