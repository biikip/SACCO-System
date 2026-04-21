using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using DinkToPdf.Contracts;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Syncfusion.EJ2.Linq;
using iText.Layout.Properties;
using System.Net.Mail;
using System.Net;

namespace EASY_SACCO.Controllers
{
    public class BasicMemberRegistrationController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConverter _converter;
        public BasicMemberRegistrationController(BOSA_DBContext context, INotyfService notyf, IConverter converter, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
            _converter = converter;
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.hostingEnvironment = hostingEnvironment;

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembersRegistration membersRegistration)
        {
            utilities.SetUpPrivileges(this);
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the highlighted errors.";
                return View(membersRegistration);
            }

            var token = Guid.NewGuid().ToString();
            var registrationLink = Url.Action("CompleteRegistration", "MembersRegistrations", new { token = token }, protocol: HttpContext.Request.Scheme);

            var subject = "Complete Your Membership Registration";
            var body = $"Please complete your registration by clicking on the link: <a href='{registrationLink}'>Register Here</a>";

            await SendEmailAsync(membersRegistration.strEmail, subject, body);

            TempData["SuccessMessage"] = "Registration link sent successfully!";
            return RedirectToAction("BasicMemberRegistration");
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
       
    }
}
