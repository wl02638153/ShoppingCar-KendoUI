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
        [Display(Name ="UserID",ResourceType =typeof(App_GlobalResources.MemberResource))]
        [Required(ErrorMessageResourceName = "UserID_Required_ErrorMessage",ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
        [StringLength(15, ErrorMessageResourceName = "UserID_StringLength_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource), MinimumLength =8)]
        public string UserID { get; set; }

        [Display(Name = "Password",ResourceType = typeof(App_GlobalResources.MemberResource))]
        [Required(ErrorMessageResourceName = "Password_Required_ErrorMessage",ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
        [StringLength(15, ErrorMessageResourceName = "Password_StringLength_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource), MinimumLength = 8)]
        [RegularExpression(@"[a-zA-Z]+[a-zA-Z0-9]*$",ErrorMessageResourceName = "Password_Regular_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]  //使用正則表式的寫法
        public string Password { get; set; }
        [Display(Name ="Password2", ResourceType = typeof(App_GlobalResources.MemberResource))]
        [Required(ErrorMessageResourceName = "Password2_Required_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
        [StringLength(15, ErrorMessageResourceName = "Password2_StringLength_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource), MinimumLength = 8)]
#pragma warning disable CS0618 // 類型或成員已經過時
        [System.Web.Mvc.Compare("Password", ErrorMessageResourceName = "Password2_Compare_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
#pragma warning restore CS0618 // 類型或成員已經過時
        public string Password2 { get; set; }
        [Display(Name = "MemberName", ResourceType = typeof(App_GlobalResources.MemberResource))]
        [Required(ErrorMessageResourceName = "MemberName_Required_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
        public string MemberName { get; set; }
        [Display(Name ="Email", ResourceType = typeof(App_GlobalResources.MemberResource))]
        [Required(ErrorMessageResourceName = "Email_Required_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
        [EmailAddress(ErrorMessageResourceName = "Email_EmailAddress_ErrorMessage", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
        public string Email { get; set; }
        [Display(Name ="Phone", ResourceType = typeof(App_GlobalResources.MemberResource))]
        [Required(ErrorMessageResourceName = "Phone_Required_Message", ErrorMessageResourceType = typeof(App_GlobalResources.MemberResource))]
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