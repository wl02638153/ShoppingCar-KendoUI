###### tags: `ASP.NET MVC`,`多國語言`
# Localization 多國語言
## 流程圖
實作多國語言有兩種方式，第一種是透過URL的方式傳入語言，最經典的例子就是微軟的官網。
* https://www.microsoft.com/zh-tw
* https://www.microsoft.com/en-US

第二種方式是透過判斷cookie的方式去設置語言。
![](https://i.imgur.com/P811FFv.png)
## 建立Resource檔
實作多國語言首先要建立Resource檔，可以使用 Resx Manager 來編輯Resource檔。
### Resx Manager安裝
https://marketplace.visualstudio.com/items?itemName=TomEnglert.ResXManager
安裝後重啟VS可以在 **工具** 看到ResX Manager

* 相關操作  https://dotblogs.com.tw/wasichris/archive/2015/06/19/151598.aspx

### 建立 App_GlobalResources 資料夾
在專案按右鍵-->加入-->加入ASP.NET資料夾-->App_GlobalResources
### 建立 Resource檔
App_GlobalResources右鍵-->加入-->資源檔
* Resource.resx 預設語言
* Resource.zh-TW.resx 繁體中文
* Resource.en-US.resx

## URL 傳入語言
設定Route
```csharp=0
routes.maproute(
    name: "default",
    url: "{culture}/{controller}/{action}/{id}",
    defaults: new { culture = "zn-ch", controller = "home", action = "index", id = urlparameter.optional },
    namespaces: new[] { "shoppingcar.controllers" }
);
```
寫一個filter來設定URL傳入的語言
```csharp=0
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCar.Filters
{
    public class CultureActionFilter : ActionFilterAttribute
    {
        private string culture;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.RouteData.Values.ContainsKey("culture"))
            {
                culture = filterContext.RouteData.Values["culture"].ToString();
            }
            else
                culture = "en-US";

            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }
    }
}
```
## Html 調用
```htmlmixed=0
<h3>@ShoppingCar.App_GlobalResources.CreateProductResource.Data_format</h3>
```

## 用 cookie 判斷
 https://dotblogs.com.tw/wasichris/2015/06/20/151608
## 參考資料
* Route帶語言方式 https://stephenyy.wordpress.com/2013/10/26/asp-net-mvc-%E7%B0%A1%E5%96%AE%E5%A4%9A%E5%9C%8B%E8%AA%9E%E8%A8%80%E5%AF%A6%E4%BD%9C/
* Route不帶語言方式 https://dotblogs.com.tw/wasichris/2015/06/20/151608
* 如呵讓 Resource檔 變成公開類別 https://blog.miniasp.com/post/2011/09/07/App_GlobalResources-PublicResXFileCodeGenerator
* Resx Manager https://dotblogs.com.tw/kinanson/2017/07/11/082615