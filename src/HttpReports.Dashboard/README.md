
 
####  微服务统计，分析，图表，监控一体化的HttpReports项目在.Net Core 中的使用


#  HttpReports
### 简单介绍  
HttpReports 是 .Net Core 下的一个Web项目, 适用于WebAPI，Ocelot网关应用，MVC项目，非常适合针对微服务应用使用，通过中间件的形式集成到您的项目中，可以让开发人员快速的搭建出一个 数据统计，分析，图表，监控 一体化的 Web站点。
 

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a1.png)  
![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a2.png) 
![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a3.png)   


#### 主要模块

主要包含HttpReports 中间件 和 HttpReports.Web 的MVC项目;

HttpReports： https://github.com/SpringLeee/HttpReports  
  
HttpReports.Web： https://github.com/SpringLeee/HttpReportsWeb

在线预览： http://175.102.11.117:8801 账号 admin 密码 123456

#### 支持项目类型  

😂 单个WebAPI应用  
😆 多个独立WebAPI应用   
😊 Ocelot 网关应用 
😛 单个MVC项目
😃 多个MVC项目


### 如何使用

##### 1.添加 HttpReports 中间件 

Nuget 包安装 HttpReports, 打开Startup.cs, 修改 ConfigureServices(IServiceCollection services) 方法，添加以下代码，放在 services.AddMvc() 之前都可以。

 选择您的应用类型：
 
😆 **单个WebAPI应用 或者 使用Ocelot网关的应用**
 
 修改 ConfigureServices 方法 ，
 
 ```csharp
	 public void ConfigureServices(IServiceCollection services)
	 { 
		 // 添加HttpReports中间件
		 services.AddHttpReportsMiddleware(WebType.API, DBType.SqlServer);

	     services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 
	}
```
😆 ** 多个独立的WebAPI应用 **

假设有一个 授权（Auth）API应用，和一个支付（Pay）API应用，并且没有使用网关，需要分别在两个项目的Startup.cs文件的 ConfigureServices 方法中分别添加以下代码:
 
###### 授权API应用(Auth)
 ```csharp
services.AddHttpReportsMiddleware(WebType.API, DBType.SqlServer,"Auth");
```
###### 支付Pay应用(Pay)
 ```csharp
services.AddHttpReportsMiddleware(WebType.API, DBType.SqlServer,"Pay");  
```

😆 **单个MVC应用** 

```csharp
	public void ConfigureServices(IServiceCollection services)
	{ 
		// 添加HttpReports中间件
		services.AddHttpReportsMiddleware(WebType.MVC, DBType.SqlServer);

		services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 
	}
```

😆 **多个MVC应用**

假设有一个 电商（Mall）应用，和一个支付（Pay）应用，需要分别在两个项目的Startup.cs文件的 ConfigureServices 方法中分别添加以下代码:

###### 电商MVC应用 （Mall）
 ```csharp
 services.AddHttpReportsMiddleware(WebType.MVC, DBType.SqlServer,"Mall");
```
###### 支付MVC应用 （Pay）
 ```csharp
 services.AddHttpReportsMiddleware(WebType.MVC, DBType.SqlServer,"Pay");  
```
😆 **切换数据库**

使用MySql数据库
```csharp
 services.AddHttpReportsMiddleware(WebType.API, DBType.MySql);
```
使用SqlServer数据库
```csharp
 services.AddHttpReportsMiddleware(WebType.API, DBType.SqlServer);
``` 
   
##### 2.使用 HttpReports 中间件  

修改 StartUp.cs 的 Configure 方法

.Net Core 2.2

```csharp
	public void Configure(IApplicationBuilder app, IHostingEnvironment env)
	{    
		//使用HttpReports 
		app.UseHttpReportsMiddleware();  

		app.UseMvc();
	}
```
必须要放在 UseMVC() 方法和其他中间件的前边，否则不生效。

.Net Core 3.0 和以上版本

```csharp
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{ 
		//使用HttpReports
		app.UseHttpReportsMiddleware();

		app.UseRouting(); 

		app.UseAuthorization(); 

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
```  
必须要放在 UseEndpoints() 方法和其他中间件的前边，否则不生效。

##### 3.  appsettings.json 配置连接字符串

 打开 appsetting.json, 添加数据库连接字符串,  **需要手动创建数据库 HttpReports**
 
```csharp
"ConnectionStrings": {
    "HttpReports": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;"
  }
```
##### 4. 运行Web应用
到这一步，已经配置完成了, 直接运行Web应用，如果中间有报错的话，可能是因为数据库的连接问题，请检查后再重试，如果没有报错的话，打开数据库 [HttpReports].[dbo].[RequestInfo],  如果能看到有数据记录，就说明 HttpReports 中间件的部分配置完成了，数据有了，下边开始配置 HttpReportsWeb 站点。

------------ 

#### HttpReports.Web部分

github源码：https://github.com/SpringLeee/HttpReportsWeb 
有需要的也可以下载源码后编译，默认的git分支是Core 2.2 版本，还有一个 core 3.0的分支；

这里提供 core2.2 和 3.0 的发布版本下载：  

Core 2.2 发布版本：   https://files.cnblogs.com/files/myshowtime/HttpReports2.2.zip 
Core 3.0 发布版本：https://files.cnblogs.com/files/myshowtime/HttpReports3.0.zip

这里以 .Net Core2.2 版本为例, 下载发布版本后，解压文件, 找到 appsettings.json文件，并修改

```csharp
{
  "ConnectionStrings": {
    "HttpReports": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;"   
  },
  "HttpReportsConfig": {
    "DBType": "SqlServer", // MySql Or SqlServer
    "UserName": "admin",
    "Password": "123456"
  }
}
```
 |  字段 | 说明  |
| ------------ | ------------ |
| HttpReports  | 数据库连接字符串，要和上边配置的中间件的数据库一致  |
| DBType  | 数据库类型 SqlServer MySql , 注意没有空格  |
| UserName  | Web站点后台登录名，可修改  |
| Password  | Web站点后台登录密码，可修改  |

 修改数据库类型和连接字符串, 然后打开命令行，启动程序，或者部署到站点也可以
 
 ```csharp
dotnet HttpReports.Web.dll
```
跳到登录页，输入默认账号 admin 密码 123456，登录到系统，看一下主要的几个页面 
 
#### 主页面

主要是Web应用 请求次数, 请求时间, 请求错误，错误率TOP, 响应最快和响应最慢等， 按天，月，年进行趋势分析,  服务节点 点击可以选中和取消，并且可以切换亮色和暗色主题 

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a5.png)

#### 预警监控

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a6.png)

HttpReports 监控预警主要针对以下几点：  

😃 响应超时 
😃 请求错误
😃 IP异常
😃 请求量监控
 
 **如何添加监控：**
 
![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a7.png)
 
这里演示添加一个监控，监控频率 选1小时，也就是1个小时 运行一次，然后填入预警的收件邮箱,可填写多个邮箱, 服务节点 可以选中单个和多个节点，默认的话，下边 4个监控都是关闭状态, 如果需要勾选启动即可 

##### 响应超时监控配置
 
 预防一段时间内接口大量超时，设置超时时间为4000ms ， 超时率为0.05% (最多支持两位小数,设置值要带上%号)

 ![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a8.png) 
 
##### 请求错误监控配置
 
   预防一段时间内接口大量错误，设置错误HTTP状态码为500,503， 超时率为20% 
 
 ![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a9.png) 
 
##### IP异常监控配置
 
 预防机器人请求，防止一段时间大量重复IP请求，设置IP重复率为15% 
 
 ![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_b11.png) 
 
##### 请求量监控
 
 预防短时间内接口新增大量的请求，造成系统异常，设置 单位时间 请求量为100000，当请求量达到这个值触发预警
 
 ![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_b12.png) 
 
 保存任务，任务自动运行，监控频率可以逐渐修改，找到适合系统的预警值， 如果数据达到预警值时,您就会收到HttpReports 发送给您的预警通知邮件   
 

### 项目环境基本要求

使用HttpReports中间件的.Net Core 版本 2.2, 3.0, 3.1;

HttpReports.Web 的core版本为 2.2 , 3.0 

### 性能事项

HttpReports 中间件存储数据是异步操作，所以对api接口请求的时间可以忽略, 存储数据是也只是存储基本信息，对请求内容和响应内容不作记录，后台监控任务采用Quartz.Net实现

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

 说明 | 请求次数  | 平均响应时间 ms
-|-|-
原生API|1000|32.535
使用中间件|1000|32.899 

### 总结

HttpReports  后台使用简单三层，前端使用BootStrap，如果你想给你的程序，快速的添加一套分析，图表，监控系统 ，那么使用HttpReports 是一个不错的选择，如果能帮助到您的话，还请希望给个Star， 感谢 😆

https://github.com/SpringLeee/HttpReports 

[MIT](https://github.com/SpringLeee/HttpReports/blob/master/LICENSE "MIT")

### 维护和更新
 
  [ToDoList](https://github.com/SpringLeee/HttpReports/blob/master/ToDoList.md "ToDoList")

### 交流反馈
 
如果您在项目中使用了HttpReports，或者感兴趣的可以加入QQ群，大家一起沟通，有更新也会第一时间通知，也可以添加我的微信，希望可以帮助到您
 
 ![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a15.png)
 
 

 





