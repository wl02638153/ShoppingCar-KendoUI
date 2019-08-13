using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    [System.ComponentModel.DataAnnotations.MetadataType(typeof(MemberMetaData))]
    public partial class Member
    {
    }

    public partial class MemberMetaData
    {
        public int ID { get; set; }
        [DisplayName("帳號")]
        public string UserID { get; set; }
        [DisplayName("密碼")]
        public string Password { get; set; }
        [DisplayName("姓名")]
        public string MemberName { get; set; }
        [DisplayName("電子郵件")]
        public string Email { get; set; }
        [DisplayName("電話號碼")]
        public string Phone { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        public string Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderHeader> OrderHeader { get; set; }
    }
}