using ShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCar.Service
{
    public static class ProductCustomBindingExtensions
    {
        public static IQueryable<Product> ApplyPaging(this IQueryable<Product> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }
    }
}