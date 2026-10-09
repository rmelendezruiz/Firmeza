using Firmeza.Application;
using Firmeza.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSite();
builder.AddHost();

var app = builder.Build();
app.UseHost();
app.Run();
