 
### 前言
简单说明下，APM全称Application Performance Management应用性能管理，通过各种收集请求数据，同时搭配Dashboard以实现对应用程序性能管理和故障管理的系统化解决方案。


### HttpReports 介绍

**HttpReports** 是针对.Net Core 开发的轻量级APM系统，基于MIT开源协议, 使用HttpReports可以快速搭建.Net Core环境下统计,分析,图表,监控一体化的站点，并且支持多种数据库存储，适应.Net Core WebAPI,MVC，Web项目, 通过引用Nuget构建Dashboard面板，非常适合中小项目使用。

Github地址：**https://github.com/SpringLeee/HttpReports**   感兴趣的同学欢迎 Github Star 一波...

在线预览： **https://moa.hengyinfs.com**

账号: admin 密码 123456 

### 主要功能

- 接口调用指标分析
- 多服务节点数据聚合分析
- 慢请求，错误请求分析
- 接口调用日志查询
- 趋势数据分析 (维度：小时,天，月)
- 多类型预警监控
- HTTP调用分析  
- 多数据库支持，集成方便


### 数据库支持 
 数据库 | Nuget包名称
---|---
SqlServer | HttpReports.SqlServer
MySql | HttpReports.MySQL
Oracle | HttpReports.Oracle
PostgreSQL | HttpReports.PostgreSQL
ElasticSearch | HttpReports.ElasticSearch
 
### Dashboard-UI
![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200218135947index_bg.png)

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200218133113chart1.png)

---
### 快速开始 😆 
 
#### Step1: 初始化数据库

HttpReports 需要手动创建数据库, 我这里使用 SqlServer 数据库为例，创建数据库 HttpReports, 当然数据库名称可以自由定义.

#### Step2: 集成到WebAPI应用
打开VS开发工具，新建一个 WebAPI 应用，这里 .Net Core 版本只要是2.0 以上即可，我这里用的是3.1版本,创建完成后，Nuget 包引用 **HttpReports** 

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219082809AddHttpReports.png)

引用成功后，因为我使用的是SqlServer 数据库，我们再Nuget引用 HttpReports.SqlServer包

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219083708AddHttpReportsSqlServer.png)


找到程序的 appsetting.json,修改为以下配置, 注意：这里Storage 配置的数据库名称要和新建的数据库名称一致,
```csharp
{ 
  "HttpReports": {
    "Storage": {
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;"
    } 
  }  
}

```

然后我们再修改 StartUp.cs 文件，修改为以下代码

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219084229webapiStartUp.png)

```csharp
public void ConfigureServices(IServiceCollection services)
{
	//添加HttpReports
	services.AddHttpReports().UseSQLServerStorage();

	services.AddControllers();
}

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	//使用HttpReports
	app.UseHttpReports();

	if (env.IsDevelopment())
	{
	app.UseDeveloperExceptionPage();
	}

	app.UseRouting();

	app.UseAuthorization();

	app.UseEndpoints(endpoints =>
	{
	endpoints.MapControllers();
	});
}
```

一切准备就绪后，我们启动 WebAPi，并且刷新几次页面，到这里为止，WebAPI的部分我们已经完成了 😛


#### Step3: 集成可视化 Dashboard
新建一个 .Net Core MVC 应用，新建完成后，通过Nuget包我们分别安装 HttpReports.Dashboard ，HttpReports.SqlServer

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219085452AddHttpReportsDashboard.png)

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219083708AddHttpReportsSqlServer.png)

引用完成后，修改MVC 项目的 appsetting.json 文件, 注意数据库要一致
```csharp
{
  "HttpReports": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    } 
  }
}
```

修改完成后，我们接着修改 MVC 项目的 Startup.cs 文件
```csharp
 public void ConfigureServices(IServiceCollection services)
 {
	// 添加Daashboard
	  services.AddHttpReportsDashboard().UseSQLServerStorage();

	  services.AddControllersWithViews();
}

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	// 使用Dashboard
	app.UseHttpReportsDashboard();

	if (env.IsDevelopment())
	{
	app.UseDeveloperExceptionPage();
	}
	else
	{
	app.UseExceptionHandler("/Home/Error");
	}
	app.UseStaticFiles();

	app.UseRouting();

	app.UseAuthorization();

	app.UseEndpoints(endpoints =>
	{
	endpoints.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
	});
}
```

一切准备就绪后，我们启动MVC 项目，如果没有问题的话，会跳转到Dashboard的登陆页面，默认账号：admin 密码: 123456 

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219092441HttpReportsLogin.png)


例子中我用的是SqlServer 数据库，其他的数据库也是类似的，我只创建了一个WebAPI，当然HttpRrports 也支持多个WebAPI，我们只要修改appsetting.json 的 Node，你可以设置一个 Node 为 Auth,一个 Node 为 Log 等等等，到这里一个最简单集成 HttpReports 的例子已经完成了, 请尽情使用吧 😆


### ElasticSearch 存储配置
ElasticSearch 会和其他数据库有些不同，如果采用的是ES存储的话，修改程序 appsetting.json,采用下边的配置即可
```csharp
{
  "HttpReports": {
    "Storage": {
      "ConnectionString": "http://localhost:9200/",
      "UserName": "admin",
      "Password": "admin"
      
    } 
  }
}
```
### 数据异步批量延迟入库
HttpReports 默认是同步入库，有的用户考虑到性能可能要用到 异步批量入库,只需要修改 WebAPI 项目

```csharp
{
  "HttpReports": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;", 
      "EnableDefer": true, //是否启用延时写入
      "DeferTime": "00:00:30", //延时写入的时间，超过此时间将进行写入
      "DeferThreshold": 5 //延时写入的阈值，缓存超过此数目时进行写入
    },
    "Node": "Pay" 
  }
}

```

### css.js等资源文件过滤
HttpReports 在捕获Http请求时，会过滤掉资源文件，如果你不想这样做，添加或者修改appsetting.json, 设置 
`FilterStaticFiles  = false` 即可  



### Dashboard 使用根目录
HttpReports.Dashboard 默认使用 MVC 项目的根目录，比如: www.xxx.com, 如果你不想这样做，添加或者修改appsetting.json，设置 `UseHome = false` , 这样访问 dashobard 的地址为  www.xxx.com/Dashboard 

### Dashboard 预警监控

HttpReports.Dashboard 集成了预警监控功能，使用的话需要先配置 Smtp 邮箱，否则接收不到预警邮件哦

我们修改Dashboard项目的appsetting.json为下面即可

```csharp
{
  "HttpReports": {
    "Storage": { 
      "ConnectionString": "DataBase=HttpReports;Data Source=localhost;User Id=root;Password=123456" 
    }, 
    "Mail": {
      "Server": "smtp.qq.com",
      "Port": 465, //smtp端口
      "Account": "",
      "Password": "",
      "EnableSsL": true //是否使用ssl
    }
  }
}
```

监控功能主要针对以下四项监控
- 响应超时
- 请求错误
- IP异常
- 请求量监控


![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219100428addjob.png)

简单说明下，监控频率 选1小时，也就是1个小时 运行一次，然后填入预警的收件邮箱,多个邮箱用逗号隔开, aaa.qq.com,bbb.qq.com , 服务节点 可以选中单个和多个节点，默认的话，下边 4个监控都是关闭状态, 如果需要勾选启动即可,具体的话这里就不多说了.

##### 最后，贴上两个完整的配置文件供大家参考

#### WebAPI端

```csharp
{
  "HttpReports": {
    "Storage": {
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;",
      "EnableDefer": false, //是否启用延时写入
      "DeferSecond": 30, //延时入库的时间（秒），超过此时间将进行写入
      "DeferThreshold": 5 //延时写入的阈值，缓存超过此数目时进行写入
    },
    "Node": "Pay", // WebAPI 的服务名称
	"Open":true, // 开启关闭
    "FilterStaticFiles": true // 是否过滤掉css,js 等资源文件 
  }
}

```
#### Dashboard端

```csharp
{
  "HttpReportsDashboard": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    },
    "UseHome": true, // 默认使用根目录导航
    "Mail": {
      "Server": "smtp.qq.com",
      "Port": 465, //smtp端口
      "Account": "",
      "Password": "",
      "EnableSsL": true //是否使用ssl
    }
  }
}
```

### 总结

**HttpReports** 是 .Net Core环境下开源的APM系统，非常适合微服务环境中使用，如果是中小型项目的话，那么使用 HttpReports 是一个不错的选择，如果能帮助到您的话，还请希望给个Star 支持下， 感谢 😆

Github: https://github.com/SpringLeee/HttpReports


[MIT协议](https://github.com/SpringLeee/HttpReports/blob/master/LICENSE "MIT") 
 
### 交流反馈
 
如果您在项目中使用了HttpReports，或者感兴趣的可以加入QQ群，大家一起沟通，有更新也会第一时间通知，也可以添加我的微信，希望可以帮助到您
 
 ![](https://images.cnblogs.com/cnblogs_com/myshowtime/1627540/o_a15.png)  

 
