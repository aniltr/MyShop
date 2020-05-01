using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class CartService: ICartService
    {
        IRepository<Product> productContext;
        IRepository<Cart> cartContext;
        IRepository<CartItem> cartItemContext;

        const string CartSessionName = "eCommerceCartId";

        public CartService(IRepository<Product> productContext, IRepository<Cart> cartContext, IRepository<CartItem> cartItemContext)
        {
            this.productContext = productContext;
            this.cartContext = cartContext;
            this.cartItemContext = cartItemContext;
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
                    CartId = shoppingCart.Id,
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

        public List<ShoppingCartItemViewModel> GetCartItems(HttpContextBase httpContext)
        {
            Cart shoppingCart = GetCart(httpContext, false);

            if(shoppingCart != null)
            {
                var results = (from c in shoppingCart.CartItems
                              join p in productContext.Collection() on c.ProductId equals p.Id
                              select new ShoppingCartItemViewModel
                              {
                                  Id = c.Id,
                                  Quantity = c.Quantity,
                                  ProductName = p.Name,
                                  Price = p.Price,
                                  Image = p.Image
                              }).ToList();
                return results;
            }
            else
            {
                return new List<ShoppingCartItemViewModel>();
            }
        }

        public CartSummaryViewModel GetCartSummary(HttpContextBase httpContext)
        {
            Cart shoppingCart = GetCart(httpContext, false);
            CartSummaryViewModel viewModel = new CartSummaryViewModel(0, 0);
            if(shoppingCart != null)
            {
                int? cartItemCount = (from item in shoppingCart.CartItems
                                      select item.Quantity).Sum();
                decimal? cartTotalAmount = (from item in shoppingCart.CartItems
                                            join p in productContext.Collection() on item.ProductId equals p.Id
                                            select item.Quantity * p.Price).Sum();

                viewModel.CartCount = cartItemCount ?? 0;
                viewModel.CartTotal = cartTotalAmount ?? decimal.Zero;
            }
            return viewModel;
        }

        public void ClearCart(HttpContextBase httpContext)
        {
            Cart shoppingCart = GetCart(httpContext, false);
            shoppingCart.CartItems.Clear();
            cartContext.Commit();
        }
    }
}
