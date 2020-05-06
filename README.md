

<p align="center">
    <span>中文</span> |  <a href="README.en.md">English</a>
</p>

[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/dotnetcore/HttpReports/blob/master/LICENSE) 

## 前言介绍 

 **HttpReports** 是针对.Net Core 开发的轻量级APM系统，基于MIT开源协议, 使用HttpReports可以快速搭建.Net Core环境下统计,分析,图表,监控，分布式追踪一体化的站点， 适应.Net Core WebAPI,MVC，Web项目, 通过引用Nuget构建Dashboard面板，上手简单，适合在微服务架构中使用。


Github地址：**https://github.com/dotnetcore/HttpReports**   

在线预览： **https://moa.hengyinfs.com** 

账号: admin 密码 123456 

开源不易，感兴趣的同学欢迎 Github Star 一波...

## 主要功能

- 接口调用指标分析
- 多服务节点数据聚合分析
- 慢请求，错误请求分析
- 接口调用日志查询
- 趋势数据分析 (维度：分钟,小时,天)
- 多类型预警监控
- HTTP调用分析 
- Grpc调用分析 
- 分布式追踪
- 多数据库支持，集成方便

## 数据库支持 
 数据库 | Nuget包名称
---|---
**SqlServer** | HttpReports.SqlServer
**MySql** | HttpReports.MySQL
**Oracle** | HttpReports.Oracle
**PostgreSQL** | HttpReports.PostgreSQL 



## HttpReports Dashboard-UI
![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/index_bg.png)

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/chart1.png)

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/tracePage.png)

---
## 快速开始 😆 
 
### Step1: 初始化数据库

HttpReports 需要手动创建数据库, 我这里使用 SqlServer 数据库为例，创建数据库 HttpReports, 当然数据库名称可以自由定义, 后边程序要和这个数据库名字对应。

### Step2: 集成到WebAPI应用
打开VS开发工具，新建一个 WebAPI 应用，这里 .Net Core 版本只要是2.0 以上即可，我这里用的是3.1版本,创建完成后，Nuget 包引用 **HttpReports** 

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useHttpReports.png)

引用成功后，因为我使用的是SqlServer 数据库，我们再Nuget引用 **HttpReports.SqlServer** 包

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useSqlServer.png)

找到程序的 appsetting.json,修改为以下配置, 注意：这里Storage 配置的数据库名称要和新建的数据库名称一致

```csharp
{
  "HttpReports": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    },
    "Node": "UserService"
  } 
} 

```

配置完成后，然后我们再修改 StartUp.cs 文件，修改为以下代码

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/startup.png)

```csharp
public void ConfigureServices(IServiceCollection services)
{
	 
	services.AddHttpReports().UseSQLServerStorage();

	services.AddControllers();
}

 
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	 
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

### Step3: 集成可视化 Dashboard
使用VS新建一个 .Net Core MVC 应用, 新建完成后，通过Nuget包我们分别安装 **HttpReports.Dashboard** ，**HttpReports.SqlServer**

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useDashboard.png)

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/usedashboardSQLServer.png)

引用完成后，修改Dahboard项目的 appsetting.json 文件, 注意数据库要一致
```csharp
{
  "HttpReportsDashboard": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    } 
  }
}
```

修改完成后，我们接着修改 Dahboard 项目的 Startup.cs 文件

```csharp
 public void ConfigureServices(IServiceCollection services)
 { 
	  services.AddHttpReportsDashboard().UseSQLServerStorage();

	  services.AddControllersWithViews();
}

 
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{ 
	app.UseHttpReportsDashboard(); 
	...
}
```



一切准备就绪后，我们启动Dashboard 项目，如果没有问题的话，会跳转到Dashboard的登陆页面
默认账号：admin 密码: 123456 , 登陆后可修改

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219092441HttpReportsLogin.png)

例子中我用的是SqlServer 数据库，其他的数据库也是类似的，我只创建了一个WebAPI，当然HttpRrports 也支持多个WebAPI，我们只要修改appsetting.json 的 Node，你可以设置 Node 为 UserService, OrderService... ，到这里一个最简单集成 HttpReports 的例子已经完成了, 请尽情使用吧 😆

## Grpc 支持
微服务Grpc的流行，我们也做了Grpc的适配，如果您的项目中使用Grpc通信的话，需要在你的api项目中，Nuget包引用 **HttpReports.Grpc**，注意这里是api项目引用，不是Dashboard 项目引用

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/usegrpc.png)

修改startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpReports().UseSQLServerStorage().UseGrpc(); 
   
    services.AddControllersWithViews();
}
```

## 预警监控

HttpReports.Dashboard 集成了预警监控功能，使用的话需要先配置 Smtp 邮箱，否则接收不到预警邮件哦，


我们修改Dashboard项目的appsetting.json为下面即可

```csharp
{
  "HttpReportsDashboard": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    }, 
    "Mail": {
      "Server": "smtp.qq.com",
      "Port": 465,  
      "Account": "",
      "Password": "",
      "EnableSsL": true  
    }
  }
}
```

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/jobwarring.png)


监控功能主要针对以下四项监控
- 响应超时
- 请求错误
- IP异常
- 请求量监控

简单说明下，监控频率 选1小时，也就是1个小时 运行一次，然后填入预警的收件邮箱,多个邮箱用逗号隔开, aaa.qq.com,bbb.qq.com , 服务节点 可以选中单个和多个节点，默认的话，下边 4个监控都是关闭状态, 如果需要勾选启动即可,具体的话这里就不多说了.

预警支持 WebHook，配置后可以自动把预警信息推送到您定义的地址,推送方式为Post推送
```csharp
{
 "Title":"...",
 "Content":"..."  
}


``` 


最后，贴上两个完整的配置文件供大家参考

### WebAPI端

```csharp
{
  "HttpReports": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;", 
      "EnableDefer": false,
      "DeferSecond": 20,
      "DeferThreshold": 3
    },
    "Node": "UserService",
    "Switch": true,
    "FilterStaticFiles": true
  }
}

```

参数说明：
**EnableDefer** 开启为异步入库，默认false
**DeferSecond** 异步入库的秒数
**DeferThreshold** 异步入库的条数
**Node** 服务节点名称
**Switch** 是否开始数据收集，默认true
**FilterStaticFiles** 收集数据是否过滤静态文件，默认true

### Dashboard端

```csharp
{
  "HttpReportsDashboard": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    },
    "UseHome": true,
    "ExpireDay":7,
    "Mail": {
      "Server": "smtp.qq.com",
      "Port": 465, 
      "Account": "",
      "Password": "",
      "EnableSsL": true  
    }
  }
}
```
参数说明：
**UseHome** Dashboard使用主页路由，默认为true，false 的话，路由为 localhost/Dashboard
**ExpireDay** 收集数据的有效期，默认30天 


## 总结

**HttpReports** 是 .Net Core环境下开源的APM系统，非常适合微服务环境中使用，如果是中小型项目的话，那么使用 HttpReports 是一个不错的选择, 开源不易，如果能帮助到您的话，还请希望给个Star 支持下， 感谢 😆

Github: https://github.com/dotnetcore/HttpReports

[MIT协议](https://github.com/dotnetcore/HttpReports/blob/master/LICENSE "MIT") 

## 交流反馈
 
如果您在项目中使用了HttpReports，或者感兴趣的可以加入QQ群 897216102， 大家一起沟通，有更新也会第一时间通知，也可以添加我的微信，希望可以帮助到您

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/mywechat3.jpg)   

 

 

  
 
