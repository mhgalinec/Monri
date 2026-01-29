using Monri.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureBuilder();

var app = builder.Build();
app.ConfigurePipeline();
app.Run();
