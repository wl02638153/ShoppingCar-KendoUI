# BlockUI
###### tags: `ASP.NET MVC`
## 設定方法
### Nuget 安裝 jquery blockui
### BundleConfig.cs設定
```c#=0
bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery.blockUI.js"));
```
### view 引用
```html=0
<script type="text/javascript" src="~/Scripts/jquery.blockUI.js"></script>
```

## Form onSubmit 使用方法

### JS
```javascript=0
function onSubmitBlock() {
        $.blockUI({
            message: "請稍等..."
        });
    }
```
### In view submit form

Razor form
```c#=0
@using (Html.BeginForm("CreateProduct"
, "Product", FormMethod.Post
, new { enctype = "multipart/form-data"
, onsubmit = "return onSubmitBlock(),Confirm_insert()" }))    //submit執行
{
    ...
}
```
Html form
```html=0
<form method="post" action="ImportProduct" enctype="multipart/form-data" onsubmit="onSubmitBlock()">
    ...
    ...
    <input type="submit" id="ImportFileSubmit" value="匯入" class="btn btn-default" disabled="disabled" />
</form>
```