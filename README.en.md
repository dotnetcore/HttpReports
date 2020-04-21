

<p align="center">
    <a href="README.md">中文</a> |  
    <span>English</a>
</p>

## Introduce

HttpReports is a lightweight APM system developed for Net Core. Based on MIT open source protocol, using HttpReports can quickly build an integrated site of statistics, analysis, charts, monitoring and distributed tracking under the.net Core environment, which can adapt to Net Core WebAPI,MVC, Web project, build Dashboard panel by reference to Nuget, easy to get started, suitable for use in micro-service architecture. 

Github ：**https://github.com/SpringLeee/HttpReports**   

The online preview： **https://moa.hengyinfs.com** 

account: admin password 123456 

Open source is not easy, interested students welcome a wave of Github Star...

## The main function

- Interface invocation metrics analysis
- Multi-service node data aggregation analysis
- Slow request, error request analysis
- The interface invokes the log query
- Trend data analysis (dimensions: minutes, hours, days)
- Multiple types of warning monitoring
- HTTP call analysis
- Grpc call analysis
- Distributed tracing
- Multi - database support, easy integration

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

HttpReports requires manual database creation. I use SqlServer database as an example to create database HttpReports. Of course, the database name can be defined freely.

### Step2: integrate into WebAPI application
Open the VS development tool and create a new WebAPI application. Here, as long as the version of.Net Core is 2.0 or above, I use version 3.1 here，then Nuget install HttpReports

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useHttpReports.png)

After the reference is successful, because I'm using a SqlServer database, let's Nuget reference HttpReports.SqlServer package

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useSqlServer.png)

Find the appsetting.json of the program and change it to the following configuration. Note: the database name of the Storage configuration here should be the same as the new database name


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

Once the configuration is complete, we then modify the StartUp. cs file to the following code

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
With everything in place, we launch the WebAPi and refresh the page a few times. By this point, the WebAPi part is complete 😛

### Step3: integrate visual Dashboard
Use VS to create a new.net Core MVC application, after the completion of the new, through the Nuget package we installed HttpReports.Dashboard, HttpReports.SqlServer

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/useDashboard.png)

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/usedashboardSQLServer.png)

After the reference is completed, modify the appsets. json file of the Dahboard project, and note that the database should be consistent
```csharp
{
  "HttpReportsDashboard": {
    "Storage": { 
      "ConnectionString": "Max Pool Size = 512;server=.;uid=sa;pwd=123456;database=HttpReports;" 
    } 
  }
}
```

When the changes are complete, we then modify the startup.cs file for the Dahboard project

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


When everything is ready, we start the Dashboard project. If there is no problem, it will jump to the Dashboard login page with the default account: admin password: 123456, which can be changed after login

![](https://images.cnblogs.com/cnblogs_com/myshowtime/1650023/o_200219092441HttpReportsLogin.png)

In the example, I use SqlServer database, other databases are similar, I only created a WebAPI, of course, HttpRrports also support multiple WebAPI, we just modify appsetting.json Node, you can set Node as UserService, OrderService... At this point, the simplest example of integrating HttpReports has been completed. Feel free to use it 😆

## Grpc support
The popularity of microservice Grpc, we also do the adaptation of Grpc, if you use Grpc communication in your project, you need to use your API project, Nuget package reference HttpReports.Grpc, note here is the API project reference, not the Dashboard project reference

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/usegrpc.png)

Modify the startup. Cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpReports().UseSQLServerStorage().UseGrpc(); 
   
    services.AddControllersWithViews();
}
```

## Early warning and monitoring

HttpReports.Dashboard integrates the function of early warning monitoring. If you use it, you need to configure the Smtp mailbox first, otherwise you will not receive the early warning email.
We modify the Dashboard project appsetting. Json can be as follows

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

**HttpReports**  is an open source APM system in the.net Core environment, which is very suitable for microservice environment. If it is a small or medium-sized project, then using HttpReports is a good choice, open source is not easy, if it can help you, please hope to give a Star support, thank you 😆

Github: https://github.com/SpringLeee/HttpReports

[MIT](https://github.com/SpringLeee/HttpReports/blob/master/LICENSE "MIT") 

## Communication feedback
 
If you use HttpReports in the project, or if you are interested, you can join the QQ group, we can communicate together, there will be updates will be notified at the first time, you can also add my WeChat, I hope to help you

![](https://lee-1259586045.cos.ap-shanghai.myqcloud.com/mywechat3.jpg)   

 
  

 

  
 
