using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Models;
using MyShop.Services;
using MyShop.WebShop.UI.Tests.Mock;
using Moq;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;
using MyShop.Core.ViewModel;
using System.Linq;

namespace MyShop.WebShop.UI.Tests
{
    [TestClass]
    public class CartServiceTest
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
        public void AddToCartTest()
        {
            //Arrange
            MockRepository<Product> productContext = new MockRepository<Product>();
            MockRepository<Cart> cartContext = new MockRepository<Cart>();
            MockRepository<CartItem> cartItemContext = new MockRepository<CartItem>();

            CartService cart = new CartService(productContext, cartContext, cartItemContext);

            //Act
            cart.AddToCart(context.Object, "2");

            //Assert
            Assert.IsTrue((cartContext.Collection().ToList()[0]).CartItems.Count == 1);
        }

        [TestMethod]
        public void RemoveFromCartTest()
        {
            //Arrange
            MockRepository<Product> productContext = new MockRepository<Product>();
            MockRepository<Cart> cartContext = new MockRepository<Cart>();
            MockRepository<CartItem> cartItemContext = new MockRepository<CartItem>();

            CartService cart = new CartService(productContext, cartContext, cartItemContext);
            cart.AddToCart(context.Object, "2");

            var cartId = (cartContext.Collection().ToList()[0]).CartItems.ToList()[0].Id;

            //Act
            cart.RemoveFromCart(context.Object, cartId);

            //Assert
            Assert.IsTrue((cartContext.Collection().ToList()[0]).CartItems.Count == 0);
        }

        [TestMethod]
        public void GetCartItemsTest()
        {
            //Arrange
            MockRepository<Product> productContext = new MockRepository<Product>();
            MockRepository<Cart> cartContext = new MockRepository<Cart>();
            MockRepository<CartItem> cartItemContext = new MockRepository<CartItem>();

            productContext.Insert(new Product { Id = "2" });
            CartService cart = new CartService(productContext, cartContext, cartItemContext);
            cart.AddToCart(context.Object, "2");

            //Act
            List<ShoppingCartViewModel> items = cart.GetCartItems(context.Object);

            //Assert
            Assert.IsTrue(items.Count == 1);
        }

        [TestMethod]
        public void GetCartSummary()
        {
            //Arrange
            MockRepository<Product> productContext = new MockRepository<Product>();
            MockRepository<Cart> cartContext = new MockRepository<Cart>();
            MockRepository<CartItem> cartItemContext = new MockRepository<CartItem>();

            productContext.Insert(new Product { Id = "1", Price = 10 });
            productContext.Insert(new Product { Id = "2", Price = 20 });
            
            //Act
            CartService cart = new CartService(productContext, cartContext, cartItemContext);
            cart.AddToCart(context.Object, "1");
            cart.AddToCart(context.Object, "2");
            cart.AddToCart(context.Object, "2");

            //Assert
            CartSummaryViewModel result = cart.GetCartSummary(context.Object);
            Assert.IsTrue(result.CartCount == 3 && result.CartTotal == 50);
        }
    }
}
