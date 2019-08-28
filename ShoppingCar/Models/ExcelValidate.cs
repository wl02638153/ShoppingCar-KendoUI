using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace ShoppingCar.Models
{
    public class ExcelValidate
    {
        public string ErrorMessage { get; set; }
        public ExcelWorkbook workbook { get; set; }
        public ExcelWorksheet worksheet { get; set; }
        public ExcelPackage ep { get; set; }
        public bool CheckExcelData(HttpPostedFileBase ImportFile)
        {
            ep = new ExcelPackage(ImportFile.InputStream);
            workbook = ep.Workbook;
            if (workbook != null)
            {
                if (workbook.Worksheets.Count > 0)
                {
                    return true;
                }
            }
            ErrorMessage = "檔案內容不符合";
            return false;
        }
    }
}