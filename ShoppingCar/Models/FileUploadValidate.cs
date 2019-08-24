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
        public bool UploadUserFile(HttpPostedFileBase file)
        {
            try
            {
                var supportedTypes = new[] { "xls", "xlsx" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    ErrorMessage = "只允許 EXCEL 檔案";
                    return false;
                }
                else if (file.ContentLength > (filesize * 1024))
                {
                    ErrorMessage = "檔案限制最高 " + filesize + "KB";
                    return false;
                }
                else
                {
                    ErrorMessage = "";//檔案合法
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "檔案不能為空的";
                return false;
            }
        }
    }
}