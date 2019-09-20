# Excel 新增產品
###### tags: `ASP.NET MVC`,`Excel`,`data validation`
1. 檢查
```c#=0
[HttpPost]
[CreateProductFilter]
public ActionResult ImportProduct(HttpPostedFileBase ImportFile)
{
    //check file format
    FileUploadValidate fs = new FileUploadValidate();
    fs.filesize = 2000;
    string message = "";
    bool fileCheck = fs.UploadUserFile(ImportFile);
    if (fileCheck)  
    {
        ExcelPackage ep = new ExcelPackage(ImportFile.InputStream);
        var workbook = ep.Workbook;
        if (workbook != null)
        {
            if (workbook.Worksheets.Count > 0)
            {
                var currentWorkSheet = workbook.Worksheets.First();
                if (currentWorkSheet.Cells[102,1].Value!=null)
                {
                    message = "最多上傳100筆產品";
                    TempData["ExcelResultMessage"] = message;
                    return View("CreateProduct", Session["UserTag"].ToString());
                }
                object colHeader = currentWorkSheet.Cells[2, 2].Value;
                int col = 1;
                int row = 2;
                foreach (var item in currentWorkSheet.Cells)
                {
                    Product product = new Product();
                    if (currentWorkSheet.Cells[row, col].Value != null)
                    {
                        try
                        {
                            product.ProductID = currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductName = currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductExplain = currentWorkSheet.Cells[row, col++].Value.ToString();
                            product.ProductPrice = Convert.ToDecimal((double)currentWorkSheet.Cells[row, col++].Value);
                            product.Create_Date = DateTime.Now;
                            product.Delete_Flag = false;
                            byte[] temp = BitConverter.GetBytes(0);
                            product.ProductImg_DB = temp;

                            if (db.Product.Any(p => p.ProductID.Equals(product.ProductID)))    //判斷資料是否重複
                            {
                                message += "<p>[第"+row+"列]" + product.ProductID + "資料已重複<p/>";   //ViewBag.DBResultErrorMessage
                                continue;
                            }
                                db.Product.Add(product);
                                db.SaveChanges();
                                message += "<p>[第" + row + "列]" + product.ProductID + "上傳成功 <p/>";
                        }
                        catch (DbEntityValidationException ex)
                        {
                            logger.Error(ex.Message);
                            TempData["ExcelResultErrorMessage"] = "請確認資料格式是否正確";
                            foreach(var err in ex.EntityValidationErrors)
                            {
                                foreach(var erro in err.ValidationErrors)
                                {
                                    var ErrID = erro.PropertyName;
                                    if (ErrID == "ProductID") ErrID = "產品編號";
                                    else if (ErrID == "ProductName") ErrID = "產品名稱";
                                    else if (ErrID == "ProductExplain") ErrID = "產品說明";
                                    else if (ErrID == "ProductPrice") ErrID = "產品價錢";
                                    message += "<p>[第" + row + "列," + ErrID + "]" + erro.ErrorMessage+"<p/>";
                                }
                            }
                            db.Product.Remove(product); //移除錯誤實體避免判斷錯誤
                        }
                        catch (InvalidCastException ex)
                        {
                            var ErrID="";
                            if ((col-1) == 1) ErrID = "產品編號";
                            else if ((col - 1) == 2) ErrID = "產品名稱";
                            else if ((col - 1) == 3) ErrID = "產品說明";
                            else if ((col - 1) == 4) ErrID = "產品價錢";
                            message += "<p>[" + row +","+ ErrID + "]資料型態輸入錯誤</p>";
                        }
                        catch(Exception ex)
                        {
                            TempData["ExcelResultErrorMessage"] = ex.Message;
                        }
                        finally
                        {
                            col = 1;
                            row++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                TempData["ExcelResultMessage"] = message;
            }
        }
    }
    else
    {//檔案驗證失敗
        TempData["ExcelResultErrorMessage"] = fs.ErrorMessage;
    }

    return View("CreateProduct", Session["UserTag"].ToString());
}
```
