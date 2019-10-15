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
         public string CategoryName { get; set; }


        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product { get; set; }

        



    }
}