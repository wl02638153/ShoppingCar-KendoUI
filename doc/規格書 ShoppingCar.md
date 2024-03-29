# 規格書 ShoppingCar
###### tags: `ShoppingCar規格書`
## 產品
* ### 功能
    *     Excel批量上傳產品
    *     產品列表
    *     產品詳細頁面
    *     管理產品資料 新增 刪除 修改

## 欄位/屬性說明
### 產品管理

| 欄位名稱 | 顯示格式 | 規則說明 |
| -------- | -------- | -------- | -------- |
| 產品圖片         | 圖片     |1.顯示產品圖片|
|產品編號|文字|1.顯示產品編號
|產品名稱|文字|1.顯示產品名稱
|產品說明|文字|1.顯示產品說明
|產品售價|小數|1.顯示產品售價
|建立日期|日期時間|1.顯示產品建立日期
|產品編輯|文字連結|1.點擊進入產品編擊畫面
|刪除產品|文字連結|1.點擊刪除產品<br>2.確認刪除


### 顯示產品列表


| 欄位名稱 | 顯示格式 | 規則說明 |
| -------- | -------- | -------- | -------- |
| 產品名稱  | 文字連結     |1.點擊進入詳細產品資訊頁面|
|產品售價|小數|1.顯示價錢
|產品圖片|圖片|1.點擊進入詳細產品資訊頁面
|加入購物車|按鈕|1.必須登入才顯示<br>2.點擊加入購物車後導到購物車頁面

### 新增單筆產品

| 欄位名稱 | 顯示格式 | 規則說明 |
| -------- | -------- | -------- | -------- |
| 產品編號         | 文字輸入     |1.必填<br>2.必須大於2個字元小於15個字元|
| 產品名稱         | 文字輸入     |1.必填<br>2.必須大於2個字元小於15個字元|
|產品說明|文字輸入|1.非必填<br>
|產品售價|小數|1.必填<br>
|產品圖片|圖片|1.非必填<br>2.僅限圖片格式

### Excel 新增多筆產品


| 欄位名稱 | 顯示格式 | 規則說明 |
| -------- | -------- | -------- | -------- |
| 產品編號         | 文字輸入     |1.必填<br>2.必須大於2個字元小於15個字元|
| 產品名稱        | 文字輸入     |1.必填<br>2.必須大於2個字元小於15個字元|
|產品說明|文字輸入|1.非必填<br>
|產品售價|小數|1.必填<br>

### 產品編輯

| 欄位名稱 | 顯示格式 | 規則說明 |
| -------- | -------- | -------- | -------- |
| 產品編號         | 文字     |1.顯示產品編號<br>2.不可修改|
| 產品名稱        | 文字輸入     |1.顯示產品名稱<br>2.必須大於2個字元小於15個字元<br>3.可修改<br>4.必填|
|產品說明|文字輸入|1.非必填<br>2.可修改
|產品售價|小數輸入|1.必填<br>2.可修改
|產品圖片|圖片|1.非必填<br>2.可修改<br>3.圖片格式

### 產品詳細頁面

| 欄位名稱 | 顯示格式 | 規則說明 |
| -------- | -------- | -------- | -------- |
| 產品圖片         | 圖片     |1.顯示產品圖片|
| 產品編號         | 文字     |1.顯示產品編號|
| 產品名稱        | 文字     |1.顯示產品名稱|
|產品說明|文字|1.顯示產品說明
|產品售價|小數|1.顯示產品售價

## 參考資料
* https://masonwu1762.gitbooks.io/bakerystorespec/content/52_zhu_ce_wei_hui_yuan.html


