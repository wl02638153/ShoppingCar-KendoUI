using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    public partial class ShoppingCarLists
    {
    }
    public partial class ShoppingCarListsMetaData
    {
        public int Id { get; set; }
        public string ProductID { get; set; }
        public string UserID { get; set; }
        public Nullable<int> ProductQty { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<bool> Order_Flag { get; set; }

        public virtual Member Member { get; set; }
        public virtual Product Product { get; set; }
    }
}