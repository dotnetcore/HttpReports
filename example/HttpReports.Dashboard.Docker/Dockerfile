FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

COPY . . 
EXPOSE 8801  

ENV TZ=Asia/Shanghai

ENTRYPOINT ["dotnet", "HttpReports.Dashboard.Docker.dll"]
