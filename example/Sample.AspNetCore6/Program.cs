
var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddHttpReports().AddHttpTransport();

var app = builder.Build();

app.UseHttpReports();

app.MapGet("/", () => "Hello World!");

app.Run();
