using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    public class FileUploadValidate
    {
        public string ErrorMessage { get; set; }
        public decimal filesize { get; set; }
        public string UploadUserFile(HttpPostedFileBase file)
        {
            try
            {
                var supportedTypes = new[] { "xls", "xlsx" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    ErrorMessage = "File Extension Is InValid - Only Upload EXCEL File";
                    return ErrorMessage;
                }
                else if (file.ContentLength > (filesize * 1024))
                {
                    ErrorMessage = "File size Should Be UpTo " + filesize + "KB";
                    return ErrorMessage;
                }
                else
                {
                    ErrorMessage = "File Is Successfully Uploaded";
                    return ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Upload Container Should Not Be Empty or Contact Admin";
                return ErrorMessage;
            }
        }
    }
}