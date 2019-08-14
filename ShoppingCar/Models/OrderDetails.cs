using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    [MetadataType(typeof(OrderDetailMetaData))]
    public partial class OrderDetail
    {
    }

    public partial class OrderDetailMetaData
    {
        public int OrderDetailID { get; set; }
        [DisplayName("訂單編號")]
        public string OrderID { get; set; }

        [DisplayName("會員ID")]
        public string UserID { get; set; }
        [DisplayName("產品編號")]
        public string ProductID { get; set; }

        [DisplayName("產品名稱")]
        public string ProductName { get; set; }

        [DisplayName("數量")]
        public Nullable<int> ProductQty { get; set; }

        [DisplayName("價錢")]
        public Nullable<decimal> TotalPrice { get; set; }

        [DisplayName("下訂日期")]
        public Nullable<System.DateTime> Create_Date { get; set; }

        public Nullable<System.DateTime> Modify_Date { get; set; }

        public Nullable<System.DateTime> Delete_Date { get; set; }

        public Nullable<bool> Delete_Flag { get; set; }

        public bool Approved_Flag { get; set; }

        public virtual OrderHeader OrderHeader { get; set; }

        public virtual Product Product { get; set; }
    }
    public class OrderDetailList
    {
        public List<OrderDetail> OrderDetails { get; set; }
    }
}