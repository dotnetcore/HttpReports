FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

RUN /bin/cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime \
&& echo 'Asia/Shanghai' >/etc/timezone
 
COPY . . 
EXPOSE 8801  
ENTRYPOINT ["dotnet", "HttpReports.Dashboard.Docker.dll"]