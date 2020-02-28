FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS publish
WORKDIR /src
COPY ./src .
WORKDIR "/src/HttpReports.Dashboard.Web"
RUN dotnet restore "HttpReports.Dashboard.Web.csproj"
RUN dotnet publish "HttpReports.Dashboard.Web.csproj" -f netcoreapp3.1 -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HttpReports.Dashboard.Web.dll"]