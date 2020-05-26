

<p align="center">
    <a href="README.md">中文</a> |  
    <span>English</a>
</p>

[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/dotnetcore/HttpReports/blob/master/LICENSE) 

## Introduce

HttpReports is a lightweight APM system developed for Net Core. Based on MIT open source protocol, using HttpReports can quickly build an integrated site of statistics, analysis, charts, monitoring and distributed tracking under the .net Core environment, which can adapt to Net Core WebAPI,MVC, Web project, build Dashboard panel by adding nuget package, easy to get started, it is also suitable for use in a micro-service architecture. 

Github ：**https://github.com/dotnetcore/HttpReports**   

The online preview： **https://moa.hengyinfs.com** 

account: admin password 123456 

Open source is not easy, interested students welcome a wave of Github Star...

## Main features

- Interface invocation metrics analysis
- Multi-service node data aggregation analysis
- Slow request, error request analysis
- The interface invokes the log query
- Trend data analysis (dimensions: minutes, hours, days)
- Multiple types of warning monitoring
- HTTP call analysis
- Grpc call analysis
- Distributed tracing
- Multi database support, easy integration

## Database support
 Database | Nuget 
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
## Quick start  😆 
 
### Step1: initialize the database

HttpReports requires manual database creation. In the example SqlServer is used to store reports into database. Of course, the database name can be defined freely.

### Step2: integrate into WebAPI application
Open the VS development tool and create a new WebAPI application. HttpReports requires .Net Core 2.0 or higher, In the example version 3.1 is used. After project is created add HttpReport nuget package to the project.

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useHttpReports.png)

Depending on the database you use choose HttpReport database package for example HttpReports.SqlServer package

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useSqlServer.png)

Edit the appsetting.json in the project and add HttpReports setting in the configuration as shown below. Note: Database name in the connection string should be the same as you named the database when you have manaully created the database. 


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

Once the configuration is complete modify the Startup.cs file:

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
After you have modified Startup.cs launch the WebAPi and refresh the page a few times so you get some reports into the database. 

### Step3: integrate visual Dashboard
- Add new .net Core MVC application to the solution and add following nuget packages to the project HttpReports.Dashboard, HttpReports.SqlServer

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useDashboard.png)

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/usedashboardSQLServer.png)

-Modify the appsettings.json file of the Dahboard project note that the database name should be consistent
```csharp
{
  "HttpReportsDashboard": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    } 
  }
}
```

When the changes are complete modify the startup.cs file in the Dahboard project

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


When everything is completed, start the Dashboard project. Dashboard login page should be displayed. Login with the default account: admin password: 123456, which can be changed after login

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219092441HttpReportsLogin.png)

In the example, SqlServer database is used, other databases configurations are similar. In the example WebAPI is used, HttpRrports also support multiple WebAPIs, we just modify appsetting.json Node setting, you can set Node as UserService, OrderService... At this point, the simplest example of integrating HttpReports has been completed. Feel free to use it 😆

## Grpc support
Grpc microservices are also supported. if you want to use Grpc in your project, you need to  Nuget package HttpReports.Grpc to your project. Note  You should add nuget package to the API project and not to the Dashboard project.

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/usegrpc.png)

Modify the Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpReports().UseSQLServerStorage().UseGrpc(); 
   
    services.AddControllersWithViews();
}
```

## Early warning and monitoring

HttpReports.Dashboard integrates the functionality of early warning monitoring. If you want to use it, you need to configure the Smtp  first, otherwise you will not receive early warnings on the email.
Modify the Dashboard project appsettings.json as shown bellow

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


Monitoring functions are mainly for the following four monitoring
- Response timeout
- Request error
- IP anomalies
- Request volume monitoring 

Under the simple description, monitoring frequency choose 1 hour, which is 1 hour running time, and then fill in the early warning of the receiver mailbox, multiple E-mail use commas, aaa.qq.com, bbb.qq.com, select the single and multiple service node, the default, below 4 monitoring is closed, if need to check the startup, specific said there is not much.

Warning support WebHook, after configuration can automatically push the warning information to the address you defined, push mode is Post push
```csharp
{
 "Title":"...",
 "Content":"..."  
}


``` 


Finally, two complete configuration files are posted for your reference

### WebAPI

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

Parameter:
EnableDefer
DeferSecond
DeferThreshold 
Node 
Switch  
FilterStaticFiles

### Dashboard

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
Parameter:
UseHome  
ExpireDay  


## Conclusion

**HttpReports**  is an open source APM system in the.net Core environment, which is very suitable for microservice environment. If it is a small or medium-sized project, then HttpReports is a good choice, open source is not easy, if it can help you, please  give us a star, thank you 😆

Github: https://github.com/dotnetcore/HttpReports

[MIT](https://github.com/dotnetcore/HttpReports/blob/master/LICENSE "MIT") 

## Communication feedback
 
If you use HttpReports in the project, or if you are interested, you can join the QQ group, News about HttpReports are published in QQ group. If You have any questions you can also add me on WeChat. 

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/mywechat3.jpg)   

 
  

 

  
 
