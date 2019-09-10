using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    public class CustomizedPaging
    {
        public List<Product> Products { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
    }
}