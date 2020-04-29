using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.ViewModel
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; }
        public List<ProductCategory> ProudctCategories { get; set; }
    }
}
