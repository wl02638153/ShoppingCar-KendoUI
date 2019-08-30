using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCar.Models
{
    public class ImgUploadValidate
    {
        public string ErrorMessage { get; set; }
        public decimal filesize { get; set; }
        public string contentType { get; set; }
        public bool UploadUserFile(HttpPostedFileBase file)
        {
            try
            {
                var supportedTypes = new[] { "png","PNG", "jpg","JPG","jpeg","JPEG","jfif","JFIF" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                
                if (!supportedTypes.Contains(fileExt))
                {
                    ErrorMessage = "只允許 png,jpg,jpeg,jfif 檔案";
                    return false;
                }
                else if (file.ContentLength > (filesize * 1024))
                {
                    ErrorMessage = "檔案限制最高 " + filesize + "KB";
                    return false;
                }
                else
                {
                    contentType = file.ContentType;
                    if (contentType == "image/jpg") contentType = ".jpg";
                    else if (contentType == "image/jpeg") contentType = ".jpeg";
                    else if (contentType == "image/png") contentType = ".png";
                    ErrorMessage = "";//檔案合法
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "檔案不能為空的";
                //ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}