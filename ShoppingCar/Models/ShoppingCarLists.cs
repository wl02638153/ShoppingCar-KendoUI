using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCar.Models
{
    [MetadataType(typeof(ProductMetaData))]
    public partial class ShoppingCarList
    {
    }
    public partial class ShoppingCarListMetaData
    {
        public int ShoppingCarID { get; set; }

        [DisplayName("會員")]
        public string UserID { get; set; }
        [DisplayName("產品名稱")]
        public string ProductID { get; set; }
        [DisplayName("數量")]
        public Nullable<int> ProductQty { get; set; }
        [DisplayName("建立日期")]
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<bool> Order_Flag { get; set; }

        public virtual Member Member { get; set; }
        public virtual Product Product { get; set; }
    }
}