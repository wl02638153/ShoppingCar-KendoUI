using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
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
        [Display(Name = "ProductID",ResourceType =typeof(App_GlobalResources.ProductResource))]
        [Required(ErrorMessageResourceName = "ProductID_Required_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [DataType(DataType.Text, ErrorMessageResourceName = "ProductID_DataType_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [StringLength(15, ErrorMessage = "產品編號必須大於2個字元小於15個字元", MinimumLength = 2)]
        public string ProductID { get; set; }

        [DisplayName("產品名稱")]
        [Display(Name = "ProductName", ResourceType = typeof(App_GlobalResources.ProductResource))]
        [Required(ErrorMessageResourceName = "ProductName_Required_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [DataType(DataType.Text, ErrorMessageResourceName = "ProductName_DataType_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [StringLength(15, ErrorMessageResourceName = "ProductName_StringLength_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource), MinimumLength = 2)]
        public string ProductName { get; set; }

        [DisplayName("產品說明")]
        [Display(Name = "ProductExplain", ResourceType = typeof(App_GlobalResources.ProductResource))]
        [DataType(DataType.Text, ErrorMessageResourceName = "ProductExplain_DataType_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [StringLength(4000, ErrorMessageResourceName = "ProductExplain_StringLength_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource), MinimumLength =0)]
        public string ProductExplain { get; set; }

        [DisplayName("產品售價")]
        [Display(Name = "ProductPrice", ResourceType = typeof(App_GlobalResources.ProductResource))]
        [Range(1,1000000, ErrorMessageResourceName = "ProductPrice_Range_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [Required(ErrorMessageResourceName = "ProductPrice_Required_Message", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [DataType(DataType.Currency, ErrorMessageResourceName = "ProductPrice_DataType_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        public Nullable<decimal> ProductPrice { get; set; }

        [DisplayName("產品圖片")]
        [Display(Name = "ProductImg", ResourceType = typeof(App_GlobalResources.ProductResource))]
        public string ProductImg { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        [Display(Name = "ProductImg_DB", ResourceType = typeof(App_GlobalResources.ProductResource))]
        public byte[] ProductImg_DB { get; set; }

        [Display(Name = "Shelf_Flag", ResourceType = typeof(App_GlobalResources.ProductResource))]
        public bool Shelf_Flag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}