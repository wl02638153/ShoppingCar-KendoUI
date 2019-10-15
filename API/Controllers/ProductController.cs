using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    /// <summary>
    /// Product API
    /// </summary>
    public class ProductController : ApiController
    {
        private IProductRepository repository = new ProductRepository();

        /// <summary>
        /// 取得所有 Product
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProdcts()
        {
            return repository.GetAll();
        }

        /// <summary>
        /// 取得單筆 Product
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public Product GetProduct(string ProductID)
        {
            Product item = repository.Get(ProductID);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        /// <summary>
        /// 新增 Product
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public HttpResponseMessage PostProduct(Product item)
        {
            item = repository.Add(item);
            var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

            string uri = Url.Link("DefaultApi", new { id = item.ProductID });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        /// <summary>
        /// 更新 Product
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="product"></param>
        public void PutProduct(string ProductID, Product product)
        {
            product.ProductID = ProductID;
            if (!repository.Update(product))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 刪除 Product
        /// </summary>
        /// <param name="ProductID"></param>
        public void DeleteProduct(string ProductID)
        {
            Product item = repository.Get(ProductID);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            repository.Remove(ProductID);
        }
    }

    
}
