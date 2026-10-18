
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using ShopWeb.Filters;
using ShopWeb.Options;
using ShopWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ShopApi settings
builder.Services
    .AddOptions<ShopApiOptions>()
    .Bind(builder.Configuration.GetSection(ShopApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// JWT settings
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// JWT token service
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IShopApiTokenService, ShopApiTokenService>();

builder.Services.AddTransient<ShopApiAuthHandler>();

// HTTP client for ShopApi
builder.Services.AddHttpClient<IShopApiClient, ShopApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ShopApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
})
.AddHttpMessageHandler<ShopApiAuthHandler>()
.AddStandardResilienceHandler(options =>
{
    // Do not retry POST, PUT or DELETE requests
    options.Retry.DisableForUnsafeHttpMethods();
});

// MVC controllers and exception filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ShopApiExceptionFilter>();
});

var app = builder.Build();

app.UseRequestLocalization("en-IE");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    // Dev only: copy this token into ShopApi.http to call the API by hand
    app.MapGet("/dev/api-token", (HttpContext http, IShopApiTokenService tokens) =>
        tokens.CreateToken(http.User, TimeSpan.FromMinutes(60)))
        .RequireAuthorization();
}

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
