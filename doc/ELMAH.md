###### tags: `ASP.NET MVC`,`ELMAH`
# ELMAH
Elmah 是一個錯誤紀錄工具，提供一個完整的錯誤紀錄介面。

## 安裝
到 Nuget 上安裝 
* Arebis.Web.Mvc.ElmahDashBoard
* Elmah.Mvc
* Elmah.corelibrary(安裝Elmah.Mvc後會自動安裝)

## 建立 Sql Server Log資料庫 

這裡能下再到各種DB的 script 檔
http://elmah.github.io/downloads/

### SSMS .sql 檔建立方法
https://dotblogs.com.tw/mis2000lab/2015/12/25/sql_server_export_insert_azure

## 設定 Web.config檔
相關設定會自動帶入只要改值，跟連線字串設定。
```xml=0
<connectionStrings>
    <add name="elmah" connectionString="Data Source=yourhost\SQLServer2017;initial catalog=yourDBName;user id=jack;password=password;" providerName="System.Data.SqlClient"/>
</connectionStrings>
<appSettings>
    <add key="MvcElmahDashboardConnectionName" value="elmah" />
    <add key="MvcElmahDashboardCulture" value="zh-tw" />
    <add key="MvcElmahDashboardLogCount" value="5" />
    <add key="MvcElmahDashboardHeartBeatUrl" value="" />
    <add key="MvcElmahDashboardHeartBeatInterval" value="00:01:00" />
    <add key="MvcElmahDashboardHeartBeatDirectFromBrowser" value="true" />
    <add key="MvcElmahDashboardUserAgentInfoProvider" value="Caption=User Agent String.Com;InfoUrl=http://www.useragentstring.com/;ServiceUrl=http://www.useragentstring.com/?uas={value}&amp;getJSON=all" />
    <add key="MvcElmahDashboardRemoteAddressInfoProvider" value="Caption=FreeGeoIP.net;InfoUrl=http://freegeoip.net/;ServiceUrl=https://freegeoip.net/json/{value};Latitude=latitude;Longitude=longitude" />
</appSettings>
```

## 安全設定
### 修改預設路徑
到MvcElmahDashboardAreaRegistration.cs 修改預設路徑避免遭有心人士攻擊。
### 身分驗證

### 遠端是否可存取

* https://dzone.com/articles/elmah-security-and-allowremoteaccess-explained-1
* https://blog.elmah.io/elmah-security-and-allowremoteaccess-explained/

## 到 App_Start/RouteConfig.cs
加入
```csharp=0
namespaces:new[] {"ShoppingCar.Controllers"}
```
```csharp=0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShoppingCar
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces:new[] {"ShoppingCar.Controllers"}
            );
        }
    }
}

```
## 設定好後就可以直接測試
隨便輸入一個會發生錯誤的url後到 http://localhost:XXXX/MvcElmahDashboard
就可以瀏覽錯誤訊息

![](https://i.imgur.com/YvWBWGz.png)


## 參考資料
* http://kevintsengtw.blogspot.com/2016/05/aspnet-mvc-elmah-dashboard-elmah-sql.html
* https://dotblogs.com.tw/mis2000lab/2015/12/25/sql_server_export_insert_azure
* http://www.andyfrench.info/2014/07/configuring-elmah-to-use-sql-server.html
* http://elmah.github.io/downloads/
* http://www.arebis.be/MvcElmahDashboard/MvcElmahDashboard_Readme.html