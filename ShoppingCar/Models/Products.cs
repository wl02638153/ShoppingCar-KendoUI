using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    [MetadataType(typeof(ProductMetaData))]
    public partial class Product
    {
    }
    public partial class ProductMetaData
    {
        public int ID { get; set; }
        [DisplayName("產品編號")]
        public string ProductID { get; set; }
        [DisplayName("產品名稱")]
        public string ProductName { get; set; }
        [DisplayName("產品說明")]
        public string ProductExplain { get; set; }
        [DisplayName("產品售價")]
        public Nullable<decimal> ProductPrice { get; set; }
        [DisplayName("產品圖片")]
        public string ProductImg { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        public byte[] ProductImg_DB { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}