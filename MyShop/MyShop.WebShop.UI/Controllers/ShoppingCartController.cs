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
        IRepository<Customer> customerContext;
        public ShoppingCartController(ICartService cartService, IOrderService orderService, IRepository<Customer>  customerContext)
        {
            this.cartService = cartService;
            this.orderService = orderService;
            this.customerContext = customerContext;
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

        [Authorize]
        public ActionResult CheckOut()
        {
            Customer customer = customerContext.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);
            if (customer != null)
            {
                Order order = new Order()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Street = customer.Street,
                    City = customer.City,
                    State = customer.State,
                    ZipCode = customer.ZipCode
                };
                return View(order);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [Authorize]
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