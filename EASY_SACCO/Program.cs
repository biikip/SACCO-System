using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using EASY_SACCO.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.SqlServer;

using System;
using System.Text;
using EASY_SACCO.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Serialization;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using NToastNotify;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.Office.Interop.Excel;
using System.IO;
using EASY_SACCO;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
FastReport.Utils.RegisteredObjects.AddConnection(typeof(FastReport.Data.MsSqlDataConnection));

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopCenter;
});
builder.Services.AddDistributedMemoryCache();
var pdfConverter = new SynchronizedConverter(new PdfTools());


builder.Services.AddSingleton(typeof(IConverter), pdfConverter);

builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopRight,
    PreventDuplicates = true,
    CloseButton = true
});
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews();
builder.Services.AddFastReport(); 

var connectionString = builder.Configuration.GetConnectionString("BOSA");

var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
connectionStringBuilder.TrustServerCertificate = true;

var updatedConnectionString = connectionStringBuilder.ToString();

builder.Services.AddDbContext<BOSA_DBContext>(options =>
{
    options.UseSqlServer(updatedConnectionString);
});



var key = Encoding.ASCII.GetBytes("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,

        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero



    };
});

//builder.WebHost.UseUrls("http://192.168.1.26:8081");
builder.WebHost.UseUrls("http://192.168.1.26:8083");
//builder.WebHost.UseUrls("http://0.0.0.0:8081");
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDM0MjEzQDMxMzkyZTMxMmUzMGtBR1h5Rk56UkRXbXZ6NUFSN2M0OXc0ZXlaSEZFeGlhQlZDdW9hdDZwQUU9;NDM0MjE0QDMxMzkyZTMxMmUzMFptdlZOck5kQlRoa3JubHhkaFNSVGhjTnd3QUhGeXBBZ2tpb21VbEYxRzQ9;NDM0MjE1QDMxMzkyZTMxMmUzMGFQQloveEM4WEpNSWFYYWE3RUtzVGkyRkhJamwvQ2dtcHhRaXlaVmZUV1k9");

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/Home/Index", "?statusCode={0}"); app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();
app.UseNotyf();
app.UseFastReport();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
