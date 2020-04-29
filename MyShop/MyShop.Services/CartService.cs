using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class CartService
    {
        IRepository<Product> productContext;
        IRepository<Cart> cartContext;

        const string CartSessionName = "eCommerceCartId";

        public CartService(IRepository<Product> productContext, IRepository<Cart> cartContext)
        {
            this.productContext = productContext;
            this.cartContext = cartContext;
        }

        private Cart GetCart(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(CartSessionName);
            Cart shoppingCart = new Cart();
            if(cookie != null)
            {
                string cartId = cookie.Value;
                if (!string.IsNullOrEmpty(cartId))
                {
                    shoppingCart = cartContext.Find(cartId);
                }
                else
                {
                    if(createIfNull)
                        shoppingCart = CreateNewCart(httpContext);
                }
            }
            else
            {
                if (createIfNull)
                    shoppingCart = CreateNewCart(httpContext);
            }
            return shoppingCart;
        }

        private Cart CreateNewCart(HttpContextBase httpContext)
        {
            Cart shoppingCart = new Cart();
            cartContext.Insert(shoppingCart);
            cartContext.Commit();

            HttpCookie cookie = new HttpCookie(CartSessionName);
            cookie.Value = shoppingCart.Id;
            cookie.Expires = DateTime.Now.AddDays(1);

            httpContext.Response.Cookies.Add(cookie);
            return shoppingCart;
        }

        public void AddToCart(HttpContextBase httpContext, string productId)
        {
            Cart shoppingCart = GetCart(httpContext, true);
            CartItem item = shoppingCart.CartItems.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity = item.Quantity + 1;
            } 
            else
            {
                item = new CartItem
                {
                    Id = shoppingCart.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                shoppingCart.CartItems.Add(item);
            }
            cartContext.Commit();
        }

        public void RemoveFromCart(HttpContextBase httpContext, string itemId)
        {
            Cart shoppingCart = GetCart(httpContext, true);
            CartItem item = shoppingCart.CartItems.FirstOrDefault(c => c.Id == itemId);
            if(item != null)
            {
                shoppingCart.CartItems.Remove(item);
                cartContext.Commit();
            }
        }
    }
}
