using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Script.Serialization;

namespace ShoppingCar.Models
{
    [MetadataType(typeof(Product_CategoryMetaData))]
    public partial class Product_Category
    {
    }
    public partial class Product_CategoryMetaData
    {
        public int CategoryID { get; set; }

        [DisplayName("產品類別")]
        [Display(Name = "CategoryName", ResourceType = typeof(App_GlobalResources.ProductResource))]
        [Required(ErrorMessageResourceName = "ProductID_Required_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [DataType(DataType.Text, ErrorMessageResourceName = "CategoryName_DataType_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.ProductResource))]
        [StringLength(15, ErrorMessage = "產品類別必須大於2個字元小於15個字元", MinimumLength = 2)]
        public string CategoryName { get; set; }


        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product { get; set; }

        



    }
}