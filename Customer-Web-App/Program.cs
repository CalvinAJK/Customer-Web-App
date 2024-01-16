using Customer_Web_App.Controllers;
using Auth0.AspNetCore.Authentication;
using Customer_Web_App.Services.Products;
using Polly;
using Polly.Extensions.Http;    

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
if (builder.Configuration.GetValue<bool>("Services:Products:UseFake", true))
{
    builder.Services.AddTransient<IProductsService, FakeProductService>();
}
else
{
    builder.Services.AddHttpClient<ProductService>()
                .AddPolicyHandler(GetRetryPolicy());
    builder.Services.AddTransient<IProductsService, ProductService>();
    builder.Services.AddAuth0WebAppAuthentication(options => {
        options.Domain = builder.Configuration["Auth:Domain"];
        options.ClientId = builder.Configuration["Auth:ClientId"];
    });

}






builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(5, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}