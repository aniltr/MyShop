﻿using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebShop.UI.Controllers
{
    public class HomeController : Controller
    {
        IRepository<Product> dataContext;
        IRepository<ProductCategory> categoryContext;
        public HomeController(IRepository<Product> dataContext, IRepository<ProductCategory> categoryContext)
        {
            this.dataContext = dataContext;
            this.categoryContext = categoryContext;
        }

        public ActionResult Index(string category = null)
        {
            List<Product> products;
            List<ProductCategory> productCategories;
            ProductListViewModel viewModel = new ProductListViewModel();

            if (category == null)
            {
                products = this.dataContext.Collection().ToList();
            }
            else
            {
                products = this.dataContext.Collection().Where(p => p.Category == category).ToList();
            }
            productCategories = categoryContext.Collection().ToList();
            viewModel.Products = products;
            viewModel.ProudctCategories = productCategories;

            return View(viewModel);
        }

        public ActionResult Details(string id)
        {
            Product product = dataContext.Find(id);
            if(product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}