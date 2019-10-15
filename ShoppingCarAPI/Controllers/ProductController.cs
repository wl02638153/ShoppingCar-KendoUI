using ShoppingCarAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ShoppingCarAPI.Controllers
{
    public class ProductController : ApiController
    {
        private ShoppingCartEntities db = new ShoppingCartEntities();
        /// <summary>
        /// 取得所有 Product
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> Get()
        {
            var products = db.Product.Take(10).ToList().AsEnumerable();
            return products;
        }
    }
}
