using Newtonsoft.Json;
using ShoppingCarAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CompareAttribute = System.Web.Mvc.CompareAttribute;
using System.Web.Script.Serialization;

namespace ShoppingCar.Models
{
    [System.ComponentModel.DataAnnotations.MetadataType(typeof(MemberMetaData))]
    public partial class Member : AuthorizeAttribute
    {
    }

    public partial class MemberMetaData : AuthorizeAttribute
    {
        public int ID { get; set; }
        public string UserID { get; set; }

        public string Password { get; set; }
        public string Password2 { get; set; }
        public string MemberName { get; set; }
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        public string Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }
        public string Role { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<OrderHeader> OrderHeader { get; set; }
    }
}