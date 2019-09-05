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
        [Required(ErrorMessage = "產品編號不可空白")]
        [DataType(DataType.Text, ErrorMessage = "請輸入文字格式")]
        [StringLength(15, ErrorMessage = "產品編號必須大於2個字元小於15個字元", MinimumLength = 2)]
        public string ProductID { get; set; }

        [DisplayName("產品名稱")]
        [Required(ErrorMessage = "產品名稱不可空白")]
        [DataType(DataType.Text, ErrorMessage = "請輸入文字格式")]
        [StringLength(15, ErrorMessage = "產品名稱必須大於2個字元小於15個字元", MinimumLength = 2)]
        public string ProductName { get; set; }

        [DisplayName("產品說明")]
        [DataType(DataType.Text,ErrorMessage ="請輸入文字格式")]
        [StringLength(4000,ErrorMessage ="字數最多為4000字",MinimumLength =0)]
        public string ProductExplain { get; set; }

        [DisplayName("產品售價")]
        [Range(1,1000000,ErrorMessage = "產品售價必須介於1-1000000")]
        [Required(ErrorMessage = "產品售價不可空白")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> ProductPrice { get; set; }

        [DisplayName("產品圖片")]
        public string ProductImg { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        [DisplayName("產品圖片")]
        public byte[] ProductImg_DB { get; set; }
        [DisplayName("上架")]
        public bool Shelf_Flag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}