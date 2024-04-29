using EntityEquity.Common;
using EntityEquity.Common.Payment;
using EntityEquity.Data;
using EntityEquity.Hubs;
using EntityEquity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
   options.UseSqlServer(connectionString)
   .EnableSensitiveDataLogging());
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddHttpClient("DefaultHttpClient", client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAddress")));

builder.Services.AddTransient<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>().HttpContext.User);
builder.Services.AddTransient<CookieBridge>();
builder.Services.AddTransient<CookieBridgeConnection>();
builder.Services.AddSingleton<EntityEquity.Common.Payment.AchTransaction>();

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
}).AddNewtonsoftJsonProtocol();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<EmailService>();

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();
app.MapBlazorHub();
app.MapControllers();
app.MapHub<EntityHub>("/entityhub");
app.Run();
