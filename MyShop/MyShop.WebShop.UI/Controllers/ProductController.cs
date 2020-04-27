using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModel;
using MyShop.DataAccess.InMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebShop.UI.Controllers
{
    public class ProductController : Controller
    {
        string _imgUncPath = "//Content//ProductImages//";
        IRepository<Product> dataContext;
        IRepository<ProductCategory> categoryContext;
        public ProductController(IRepository<Product> dataContext, IRepository<ProductCategory> categoryContext)
        {
            this.dataContext = dataContext;
            this.categoryContext = categoryContext;
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
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if(!ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                if(file != null)
                {
                    product.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath(_imgUncPath) + product.Image);
                }

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
        public ActionResult Edit(Product product, string id, HttpPostedFileBase file)
        {
            Product productToEdit = dataContext.Find(id);
            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if(file!= null)
                {
                    productToEdit.Image = productToEdit.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath(_imgUncPath) + productToEdit.Image);
                }

                productToEdit.Name = product.Name;
                productToEdit.Description = product.Description;
                productToEdit.Category = product.Category;
                productToEdit.Price = product.Price;
                

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