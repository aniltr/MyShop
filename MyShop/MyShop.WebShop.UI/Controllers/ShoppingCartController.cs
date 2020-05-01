using MyShop.Core.Contracts;
using MyShop.Core.Models;
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
        IOrderService orderService;
        public ShoppingCartController(ICartService cartService, IOrderService orderService)
        {
            this.cartService = cartService;
            this.orderService = orderService;
        }
        // GET: ShoppingCart
        public ActionResult Index()
        {
            List<ShoppingCartItemViewModel> model = cartService.GetCartItems(this.HttpContext);
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

        public ActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckOut(Order order)
        {
            var cartItems = cartService.GetCartItems(this.HttpContext);
            Order baseOrder = new Order();
            baseOrder.OrderStatus = "Order Created";
            //payment processing
            baseOrder.OrderStatus = "Payment Processed";
            orderService.CreateOrder(baseOrder, cartItems);
            cartService.ClearCart(this.HttpContext);
            return RedirectToAction("Thankyou", new { OrderId = order.Id });
        }

        public ActionResult Thankyou(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}