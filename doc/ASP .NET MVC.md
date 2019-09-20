# ASP .NET MVC
###### tags: `ASP.NET MVC`
## 佈署到IIS
1. 在 方案總管，以滑鼠右鍵按一下專案。 選取 [發行]。 發佈頁面隨即出現。
2. 選取 新的設定檔。 挑選發行目標 對話方塊隨即出現。
3. 選取 IIS、 FTP 等。選取 建立設定檔。 發佈 精靈隨即出現。
4. 發佈更新Publish檔案。ShoppingCar/bin/Release/Publish 將此資料夾複製到IIS發佈
5. 到開發機開啟IIS管理員-->應用程式集區-->新增應用程式集區-->新增應用程式-->設定應用程式集區-->設定實體路徑 將Publish資料夾放到 inetpub/wwwroot/

### IIS權限設定
1. 到架設的站台-->IIS-->授權規則-->編輯權限-->將IIS寫入打開才能上傳檔案

### 資料庫遷移
https://dotblogs.com.tw/mis2000lab/2015/12/25/sql_server_export_insert_azure

### 參考資料
* https://docs.microsoft.com/zh-tw/aspnet/web-forms/overview/deployment/visual-studio-web-deployment/deploying-to-iis
* https://dotblogs.com.tw/mis2000lab/2015/12/25/sql_server_export_insert_azure


## 圖片壓縮 Image Compress
壓縮圖片主要有裡兩種方式，壓縮圖片的Quality，和去除圖片的Profile，通常圖片的Profile的大小差不多幾K到幾百K都有。
### 後端
* #### Controller : ProductController
* #### Action : CreateProduct
使用 Magick .Net，在NuGet安裝。

``` C#=0
//compress
using (ImageMagick.MagickImage oImage = new ImageMagick.MagickImage(path))//path為圖片路徑
{
   oImage.Format = ImageMagick.MagickFormat.Jpg;
   oImage.ColorSpace = ImageMagick.ColorSpace.sRGB;  //色盤採用sRGB
   oImage.Quality = 80;    //壓縮率
   oImage.Resize(200, 0);
   oImage.Strip(); //去除圖片profile
   oImage.Write(path);//重新寫入壓縮後的圖片
}
```

### 前端
* #### Controller : ProductController
* #### Action : CreateProduct
* #### View : CreateProduct.cshtml
使用Html5 canvas drawImage方法

* https://www.wfublog.com/2019/06/js-compress-resize-image-canvas.html

## LINQ
### Two list join
```c#=0
var orders = db.OrderHeader.Where(m => m.UserID == UserID).ToList();
var orderDetails = db.OrderDetail.Where(m => m.OrderID !=null && m.Delete_Flag != true&&m.UserID==UserID).ToList();
var q = from od in orderDetails
                    join os in orders on od.OrderID equals os.OrderID
                    select new
                    {
                        OrderId = os.OrderID,
                        Receiver = os.Receiver,
                        Email = os.Email,
                        Address = os.Address,
                        ProductID = od.ProductID,
                        ProductName = od.ProductName,
                        UserID = od.UserID,
                        ProductQty = od.ProductQty,
                        TotalPrice = od.TotalPrice,
                        Create_Date = od.Create_Date
                    };
```

### 參考資料
* https://www.codeproject.com/Questions/845586/merge-two-lists-in-a-single-list
* https://jimmy0222.pixnet.net/blog/post/36915103-linq-to-sql%E8%AA%9E%E5%8F%A5%284%29%E4%B9%8Bjoin

## Log

### NLog
在NuGet上安裝

#### 使用方式
```c#=0
using NLog;

private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        logger.Trace("我是追蹤:Trace");
        logger.Debug("我是偵錯:Debug");
        logger.Info("我是資訊:Info");
        logger.Warn("我是警告:Warn");
        logger.Error("我是錯誤:error");
        logger.Fatal("我是致命錯誤:Fatal");
```

### 參考資料
* https://dotblogs.com.tw/stanley14/2017/02/15/nlog


## 資料驗證

### Model 驗證

使用Model驗證需要引入
```C#=0
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
```


### 參考資料
* https://dotblogs.com.tw/mantou1201/2013/04/18/101814


## Filter

### 使用方式
1.  建立Attribute
    `命名方式 {name}Attribute.cs`
2.  引用Class 和調用
    `using {name}Attribute`
    `[{name}]`
    




### 參考資料
* https://dotblogs.com.tw/inblackbox/2013/06/07/
* https://www.cnblogs.com/oppoic/p/mvc_authorization_action_result_exception_filters.html


## 資料分頁

### 一般分頁
使用PagedList 在NuGet上安裝
使用方式
#### Controller
```C#=0
public class ProductController : Controller
{
	public object Index(int? page)
	{
		var products = MyProductDataSource.FindAllProducts(); //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

		var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
		var onePageOfProducts = products.ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize
		
		ViewBag.OnePageOfProducts = onePageOfProducts;
		return View();
	}
}
```

#### View
```c#=0
@{
	ViewBag.Title = "Product Listing"
}
@using PagedList.Mvc; //import this so we get our HTML Helper
@using PagedList; //import this so we can cast our list to IPagedList (only necessary because ViewBag is dynamic)

<!-- import the included stylesheet for some (very basic) default styling -->
<link href="/Content/PagedList.css" rel="stylesheet" type="text/css" />

<!-- loop through each of your products and display it however you want. we're just printing the name here -->
<h2>List of Products</h2>
<ul>
	@foreach(var product in ViewBag.OnePageOfProducts){
		<li>@product.Name</li>
	}
</ul>

<!-- output a paging control that lets the user navigation to the previous page, next page, etc -->
@Html.PagedListPager( (IPagedList)ViewBag.OnePageOfProducts, page => Url.Action("Index", new { page }) )
```

### 參考資料
* https://github.com/TroyGoode/PagedList