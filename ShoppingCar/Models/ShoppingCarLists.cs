using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    [MetadataType(typeof(ShoppingCarListMetaData))]
    public partial class ShoppingCarList
    {
    }
    public partial class ShoppingCarListMetaData
    {

        public int Id { get; set; }

        [DisplayName("產品編號")]
        public string ProductID { get; set; }

        [DisplayName("會員")]
        public string UserID { get; set; }

        [DisplayName("數量")]
        public int ProductQty { get; set; }

        [DisplayName("選擇購買")]
        public bool Order_Flag { get; set; }

        public Nullable<System.DateTime> Create_Date { get; set; }

        public virtual Member Member { get; set; }
        public virtual Product Product { get; set; }
    }
    public class ShoppingCarCheckList
    {
        public List<ShoppingCarList> ShoppingCarLists { get; set; }
    }
}