using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Net.Mail;
using System.Net;
using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace EASY_SACCO.Controllers
{
    public class MEMBERSController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public MEMBERSController(BOSA_DBContext context, INotyfService notyf) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            return View();
        }
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MEMBERS mEMBERS)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please correct the highlighted errors.";
                    return View(mEMBERS);
                }

                // Generate a unique token
                var token = Guid.NewGuid().ToString();

                
                mEMBERS.RegistrationToken = token;
                _context.MEMBERS.Add(mEMBERS);
                await _context.SaveChangesAsync();

                
                var registrationLink = Url.Action("CompleteRegistration", "OnlineMembersRegistrations", new { token = token }, protocol: HttpContext.Request.Scheme);

                
                var subject = "Complete Your Membership Registration";
                var body = $"Please complete your registration by clicking on the link: <a href='{registrationLink}'>Complete Registration</a>";

                
                await SendEmailAsync(mEMBERS.Email, subject, body);

                Notify("Records Saved Successfully", notificationType: NotificationType.success);
                return View();
            }

            catch (Exception ex)
            {
                // You can log the exception here if needed
                Notify("An error occurred: " + ex.Message, notificationType: NotificationType.error);
                return View();
            }
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("notifications@cargen.com", "lwqhgbyqbucukzei"),
                EnableSsl = true,
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("notifications@cargen.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Email Sending Failed: " + ex.Message);
                    throw;
                }
            }
        }
        [HttpGet]
        public JsonResult CheckUnique(string field, string value)
        {
            bool exists = false;

            switch (field)
            {
                case "Phone":
                    exists = _context.MEMBERS.Any(m => m.PhoneNo == value);
                    break;
                case "ID":
                    exists = _context.MEMBERS.Any(m => m.IDNO == value);
                    break;
                case "Email":
                    exists = _context.MEMBERS.Any(m => m.Email == value);
                    break;
            }

            return Json(!exists); // jQuery Validate expects 'true' for valid
        }
    }
}
