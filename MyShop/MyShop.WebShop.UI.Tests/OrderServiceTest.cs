using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Services;
using MyShop.WebShop.UI.Tests.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace MyShop.WebShop.UI.Tests
{
    [TestClass]
    public class OrderServiceTest
    {
        Mock<HttpRequestBase> request;
        Mock<HttpResponseBase> response;
        Mock<HttpContextBase> context;
        RequestContext rc;

        [TestInitialize]
        public void Init()
        {
            request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var cookies = new HttpCookieCollection();
            request.SetupGet(req => req.Cookies).Returns(cookies);
            var uri = new Uri("https://www.example.com/pixel?dr=https%3A%2F%2Fwww.example.com");
            request.SetupGet(req => req.Url).Returns(uri);
            var referrer = new Uri("https://www.example.com/referrer");
            request.SetupGet(req => req.UrlReferrer).Returns(referrer);
            request.SetupGet(req => req.UserAgent).Returns("my browser");


            response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            response.SetupGet(resp => resp.Cookies).Returns(cookies);
            response.Setup(resp =>
              resp.SetCookie(It.IsAny<HttpCookie>()))
                .Callback<HttpCookie>((cookie) => cookies.Add(cookie));

            context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(ctx => ctx.Request).Returns(request.Object);
            context.SetupGet(ctx => ctx.Response).Returns(response.Object);

            rc = new RequestContext(context.Object, new RouteData());
        }

        [TestMethod]
        public void CanPlaceOrder()
        {
            //Arrange
            IRepository<Order> orderContext = new MockRepository<Order>();
            IRepository<Product> productContext = new MockRepository<Product>();
            IRepository<Cart> cartContext = new MockRepository<Cart>();
            IRepository<CartItem> cartItemContext = new MockRepository<CartItem>();

            productContext.Insert(new Product { Id = "1", Price = 10});
            productContext.Insert(new Product { Id = "2", Price = 20 });

            ICartService cartService = new CartService(productContext, cartContext, cartItemContext);
            cartService.AddToCart(context.Object, "1");
            cartService.AddToCart(context.Object, "2");
            
            IOrderService orderService = new OrderService(orderContext);
            //Act
            orderService.CreateOrder(new Order() { OrderStatus = "Order Created" }, cartService.GetCartItems(context.Object));

            //Assert
            Assert.AreEqual(orderContext.Collection().ToList()[0].Items.Count, 2);
        }
    }
}
