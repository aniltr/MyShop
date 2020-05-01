using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> orderContext;
        public OrderService(IRepository<Order> orderContext)
        {
            this.orderContext = orderContext;
        }
        public void CreateOrder(Order baseOrder, List<ShoppingCartItemViewModel> cartItems)
        {
            foreach (var item in cartItems)
            {
                baseOrder.Items.Add(new OrderItem
                {
                    ProductId = item.Id,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Image = item.Image
                });
            }
            orderContext.Insert(baseOrder);
            orderContext.Commit();
        }
    }
}
