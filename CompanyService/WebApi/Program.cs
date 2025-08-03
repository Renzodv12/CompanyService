using CompanyService.Core;
using CompanyService.Infrastructure;
using CompanyService.WebApi.Endpoints;
using CompanyService.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapCompanyEndpoints();
app.MapProductEndpoints();
app.MapProductCategoryEndpoints();
app.MapCustomerEndpoints();
app.MapSaleEndpoints();
app.MapSupplierEndpoints();
app.MapPurchaseEndpoints();
app.MapDashboardEndpoints();
app.MapReportEndpoints();
app.MapEventEndpoints(); 
app.Run();