using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class ProductRepository : IProductRepository
    {
        private ShoppingCartEntities db = new ShoppingCartEntities();

        public IEnumerable<Product> GetAll()
        {
            var products=db.Product.ToList();
            return products;
        }

        public Product Get(string ProductID)
        {
            var products = db.Product.Where(p=>p.ProductID== ProductID).FirstOrDefault();
            return products;
        }

        public Product Add(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            db.Product.Add(item);
            db.SaveChanges();
            return item;
        }

        public void Remove(string ProductID)
        {
            var products = db.Product.Where(p => p.ProductID == ProductID).FirstOrDefault();
            db.Product.Remove(products);
            db.SaveChanges();
        }

        public bool Update(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var products = db.Product.Where(p => p.ProductID == item.ProductID).FirstOrDefault();
            if (products == null)
            {
                return false;
            }
            db.Product.Remove(products);
            db.Product.Add(item);
            return true;
        }
    }
}