using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompareAttribute = System.Web.Mvc.CompareAttribute;

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
        [Required(ErrorMessage ="帳號不可空白")]
        [StringLength(15,ErrorMessage ="帳號必須大於8個字元小於15個字元",MinimumLength =8)]
        public string UserID { get; set; }
        [DisplayName("密碼")]
        [Required(ErrorMessage = "密碼不可空白")]
        [StringLength(15, ErrorMessage = "密碼必須大於8個字元小於15個字元", MinimumLength = 8)]
        [RegularExpression(@"[a-zA-Z]+[a-zA-Z0-9]*$", ErrorMessage = "密碼僅能有英文或數字，且開頭需為英文字母！")]  //使用正則表式的寫法
        public string Password { get; set; }
        [DisplayName("確認密碼")]
        [Required(ErrorMessage = "確認密碼不可空白")]
        [StringLength(15, ErrorMessage = "確認密碼必須大於8個字元小於15個字元", MinimumLength = 8)]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "兩次輸入的密碼必須相符！")]
        public string Password2 { get; set; }
        [DisplayName("姓名")]
        [Required(ErrorMessage = "姓名不可空白")]
        public string MemberName { get; set; }
        [DisplayName("電子郵件")]
        [Required(ErrorMessage = "電子郵件不可空白")]
        [EmailAddress(ErrorMessage ="Email格式有誤")]
        public string Email { get; set; }
        [DisplayName("電話號碼")]
        [Required(ErrorMessage = "電子郵件不可空白")]
        [Phone]
        public string Phone { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Modify_Date { get; set; }
        public string Delete_Date { get; set; }
        public Nullable<bool> Delete_Flag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderHeader> OrderHeader { get; set; }
    }
}