
#  HttpReports
### 简单介绍 
HttpReports 是 .Net Core下的一个Web组件，适用于 WebAPI 项目和 API 网关项目，通过中间件的形式集成到您的项目中, 通过HttpReports，可以让开发人员快速的搭建出一个 API 性能分析的基础报表网站。

 ![](https://raw.githubusercontent.com/SpringLeee/HttpReportsWeb/master/HttpReports.Web/wwwroot/Content/img/git/a3.png) 

主要包含 HttpReports 中间件 和 HttpReports.Web 报表项目：  

HttpReports： https://github.com/SpringLeee/HttpReports 
  
HttpReports.Web： https://github.com/SpringLeee/HttpReportsWeb

### 如何使用

##### 1.运行 HttpReports.Web
 在github下载 HttpReports.Web 项目，项目地址：https://github.com/SpringLeee/HttpReportsWeb, Web项目是.Net Core MVC 项目，使用三层实现。
 
 ![](https://raw.githubusercontent.com/SpringLeee/HttpReportsWeb/master/HttpReports.Web/wwwroot/Content/img/git/a1.png)
 
 
 
 下载完成后，在VS中打开，然后还原NuGet程序包，完成后首先 appsettings.json 
#### appsettings.json
```
{
  "ConnectionStrings": {
    "HttpReports": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;"
  }, 
  "HttpReportsConfig": {
    "DBType": "SqlServer",
    "UserName": "admin",
    "Password": "123456"
  }
}

``` 
主要参数：
- HttpReports：配置一个可用的连接字符串；
- DBType：数据库类型，支持SqlServer和MySql;
- UserName: Web项目的登录名；
- Password: Web项目的登录密码；

假设我们使用的是SqlServer 数据库，需要先配置ConnectionStrings，然后手动创建数据库 HttpReports（Web项目会根据数据库自动创建表，并且在第一次运行的时候Mock一些数据 ），我们直接F5运行项目， 没有问题的话，会直接跳到登录页面，输入用户名密码 admin 123456，登录后，应该可以看到下面的页面

 ![](https://raw.githubusercontent.com/SpringLeee/HttpReportsWeb/master/HttpReports.Web/wwwroot/Content/img/git/a3.png) 
 
现在可以看到项目有 auth,payment，sms 三个服务节点，服务节点的定义如下：
 
请求地址 | 服务节点  | 说明 
-|-|-
https://www.abc.com/auth/api/user/login | auth  |
https://www.abc.com/log/api/user/login | log  |
https://www.abc.com/api/user/login | default  |  如果没有前缀的话，就是default节点

如果你的项目是单个WebAPI项目，那么服务节点只有一个 default，如果你的项目是 GateWay 网关项目，那么Web项目就可以读取到多个服务节点，比如 auth 认证，payment支付等。


##### 2.在API项目中使用

首先要删除 Web 项目的Mock数据，打开数据库 HttpReports，打开表 RequestInfo,清空数据，执行Sql
```
  Delete * From [HttpReports].[dbo].[RequestInfo]
```
###### 配置数据库连接字符串
HttpReports 适用单个API项目和网关项目，这里使用 Ocelot网关项目为例.

我们打开appsetting.json, 配置数据库连接字符串，需要和Web项目一致

![](https://raw.githubusercontent.com/SpringLeee/HttpReportsWeb/master/HttpReports.Web/wwwroot/Content/img/git/a6.png)

###### Nuget引用HttpReports

安装nuget包 **HttpReports** ，打开StartUp

在ConfigureServices 方法下添加： 
```csharp
services.AddHttpReportsMiddlewire();
```
如果是MySql数据库，则添加：
 ```csharp
 services.AddHttpReportsMiddlewire(options =>{ 
    options.DBType = DBType.MySql; 
 });
```

加入到 Configure 方法 ，需要放在 app.UseMVC() 或者 app.UseOcelot().Wait() 的前面，要不然不生效
```csharp
app.UseHttpReportsMiddlewire();
```
ConnectionStrings 配置的连接字符串和数据库类型要一致，全部完成了以后，我们就可以使用 Web 项目了。

### 项目环境基本要求

WebAPI或者网关项目支持的.Net Core 版本 2.2, 3.0, 3.1;

HttpReports.Web 的core版本为 2.2  

### 性能事项

HttpReports 中间件是异步操作，所以对api接口请求的时间可以忽略，但是由于实质使用的是数据库存储，所以要注意直接请求到数据库的压力。

下面是用PostMan做的一个简单测试：

WebAPI内的方法：

```csharp
        public string Sql1()
        {
            SqlConnection con = new SqlConnection(
                "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HyBasicData;");

            var list1 =  con.Query(" select * from [HyBasicData].[dbo].[Customers] ");

            var list2 = con.Query(" select * from [HyBasicData].[dbo].[Customers] ");

            var list3 = con.Query(" select * from [HyBasicData].[dbo].[Customers] "); 

            return list1.Count().ToString();
        } 
```
PostMan分别对添加中间件和不添加中间件的 API请求 1000次，每300ms请求一次

 说明 | 请求次数  | 平均响应时间 
-|-|-
原生API|1000|32.535
使用中间件|1000|32.899  

### 总结

HttpReports 的实现原理并不复杂，如果你想给你的 WebAPI项目，快速的添加一套分析系统 ，那么使用HttpReports 是一个不错的选择

 
### 联系作者
 
 如果您在使用过程中遇到了什么问题或者有好的建议的话，可以添加我的微信，希望可以帮助到您
 ![](https://raw.githubusercontent.com/SpringLeee/HttpReportsWeb/master/HttpReports.Web/wwwroot/Content/img/git/a9.jpg)
 
 

 





