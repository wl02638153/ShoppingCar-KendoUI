using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product Get(string ProductID);
        Product Add(Product item);
        void Remove(string ProductID);
        bool Update(Product item);
    }
}
