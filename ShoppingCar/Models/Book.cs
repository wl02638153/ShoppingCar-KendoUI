//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShoppingCar.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Nullable<System.DateTime> PubDate { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Genre { get; set; }
        public Nullable<int> AuthorId { get; set; }
    
        public virtual Author Author { get; set; }
    }
}
