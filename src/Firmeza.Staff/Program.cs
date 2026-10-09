using Firmeza.Application;
using Firmeza.Hosting;
using Firmeza.Staff.Jobs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddStaff();
builder.AddHost();
builder.Services.AddHostedService<InspectionExpiryJob>();

var app = builder.Build();
app.UseHost();
await app.PrepareSchemaAsync();
app.Run();
