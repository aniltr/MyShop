using MyShop.Core.Contracts;
using MyShop.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebShop.UI.Controllers
{
    public class ShoppingCartController : Controller
    {
        ICartService cartService;
        public ShoppingCartController(ICartService cartService)
        {
            this.cartService = cartService;
        }
        // GET: ShoppingCart
        public ActionResult Index()
        {
            List<ShoppingCartViewModel> model = cartService.GetCartItems(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToCart(string productId)
        {
            cartService.AddToCart(this.HttpContext, productId);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(string cartId)
        {
            cartService.RemoveFromCart(this.HttpContext, cartId);
            return RedirectToAction("Index");
        }

        public PartialViewResult GetShoppingCartSummary()
        {
            CartSummaryViewModel viewModel = cartService.GetCartSummary(this.HttpContext);
            return PartialView(viewModel);
        }
    }
}