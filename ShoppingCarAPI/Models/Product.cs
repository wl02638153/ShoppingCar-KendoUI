//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShoppingCarAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderDetail = new HashSet<OrderDetail>();
            this.ShoppingCarList = new HashSet<ShoppingCarList>();
        }
    
        public int ID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductExplain { get; set; }
        public Nullable<decimal> ProductPrice { get; set; }
        public string ProductImg { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        public byte[] ProductImg_DB { get; set; }
        public Nullable<bool> Shelf_Flag { get; set; }
        public Nullable<int> CategoryID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual Product_Category Product_Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingCarList> ShoppingCarList { get; set; }
    }
}
