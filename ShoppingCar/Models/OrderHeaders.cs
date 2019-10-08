using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ShoppingCar.Models
{

    [MetadataType(typeof(OrderHeaderMetaData))]
    public partial class OrderHeader
    {
    }

    public partial class OrderHeaderMetaData
    {
        public int ID { get; set; }
        [DisplayName("訂單編號")]
        public string OrderID { get; set; }
        [DisplayName("會員ID")]
        public string UserID { get; set; }
        [DisplayName("收件人")]
        public string Receiver { get; set; }
        [DisplayName("電子郵件")]
        public string Email { get; set; }
        [DisplayName("地址")]
        public string Address { get; set; }
        [DisplayName("派送日期")]
        public Nullable<System.DateTime> Delivery_Date { get; set; }
        [DisplayName("下單日期")]
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }
        [DisplayName("總價")]
        public Nullable<decimal> Price { get; set; }

        public virtual Member Member { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ScriptIgnore]
        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}