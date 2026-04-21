using System.IO;
using EASY_SACCO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EASY_SACCO.Controllers
{
    public class BaseController : Controller
    {
        protected readonly BOSA_DBContext _context;

        public BaseController(BOSA_DBContext context)
        {
            _context = context;
        }

        public void Notify(string message, string title = "",
                                    NotificationType notificationType = NotificationType.success)
        {
            var msg = new
            {
                message = message,
                title = title,
                icon = notificationType.ToString(),
                type = notificationType.ToString(),
                provider = GetProvider()
            };

            TempData["Message"] = JsonConvert.SerializeObject(msg);
        }
        protected async Task LogAudit(string actionType, string description)
        {
            var user = HttpContext.Session.GetString("UserName") ?? "Anonymous";
            var ip = HttpContext.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

            var log = new AuditLog
            {
                UserName = user,
                ActionType = actionType,
                Description = description,
                Timestamp = DateTime.UtcNow,
                IPAddress = ip
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync(); // Ensure this line is awaited
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Allow access to public routes
            if ((controller == "Home" && (action == "Login" || action == "Index" || action == "Logout" || action == "GenerateCaptcha")))
            {
                base.OnActionExecuting(context);
                return;
            }

            // Check session
            var username = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(username))
            {
                // Redirect to login if session expired
                context.Result = new RedirectToRouteResult(
                    new Microsoft.AspNetCore.Routing.RouteValueDictionary(
                        new { controller = "Home", action = "Index", area = "" }));
                return;
            }

            // Optional: Reset session idle time
            HttpContext.Session.SetString("LastAccess", DateTime.Now.ToString());

            base.OnActionExecuting(context);
        }

        private string GetProvider()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var value = configuration["NotificationProvider"];

            return value;
        }
    }
}