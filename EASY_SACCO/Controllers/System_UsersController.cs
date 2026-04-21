using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Utils;
using EASY_SACCO.ViewModels;
using Syncfusion.EJ2.Inputs;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Net.Mail;

namespace EASY_SACCO.Controllers
{
    public class System_UsersController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public System_UsersController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }
        // GET: System_Users
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();
            general.locations = _context.Location.ToList();
            general.co_Operatives = _context.co_Operatives.ToList();
            return View(general);
        }
        // GET: System_Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var system_Users = await _context.system_Users
                .FirstOrDefaultAsync(m => m.strId == id);
            if (system_Users == null)
            {
                return NotFound();
            }

            return View(system_Users);
        }
        [HttpGet]
        public async Task<IActionResult> GetMemberDetails(string memberNo)
        {
            if (string.IsNullOrWhiteSpace(memberNo))
                return Json(new { success = false, message = "Invalid Member No" });

            var member = await _context.MembersRegistrations
                   .FirstOrDefaultAsync(m =>
                       m.strMemberNo == memberNo && m.IsApproved == true && m.HasPaidRegFee == true);

            if (member == null)
                return Json(new { success = false, message = "You are not eligible to register. Please ensure your membership is approved and you have paid the registration fee." });

            return Json(new
            {
                success = true,
                firstName = member.strOtherName, // or whatever fields you want
                lastName = member.strSurName,
                phoneNo = member.strPhoneNo,
                email = member.strEmail
            });
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.UserTypes = await _context.user_Groups
        .Select(l => new { strGroupName = l.strGroupName, strId = l.strId }).ToListAsync();

            // Load default SACCO
            var sacco = await _context.co_Operatives.FirstOrDefaultAsync(); 
            ViewBag.SaccoList = await _context.co_Operatives
                .Select(c => new { strCompanyName = c.strCompanyName, strCompanyCode = c.strCompanyCode })
                .ToListAsync();

            ViewBag.DefaultSaccoId = sacco?.strCompanyCode; 

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(System_Users model)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                }
            }
            if (model.strUserType == "MEMBER")
            {
                model.strUserName = model.MemberNo;
            }
            
            if (ModelState.IsValid)
            {
                model.strPassword = BCrypt.Net.BCrypt.HashPassword(model.strPassword);
                var member = await _context.MembersRegistrations
                    .FirstOrDefaultAsync(m =>
                        m.strMemberNo == model.strUserName &&m.IsApproved==true && m.HasPaidRegFee==true);

                if (member == null)
                {
                    ModelState.AddModelError(string.Empty, "You are not eligible to register. Please ensure your membership is approved and you have paid the registration fee.");

                    ViewBag.UserTypes = _context.user_Groups
                        .Select(l => new { strGroupName = l.strGroupName, strId = l.strId }).ToList();

                    ViewBag.SaccoList = _context.co_Operatives
                        .Select(c => new { strCompanyName = c.strCompanyName, strCompanyCode = c.strCompanyCode }).ToList();

                    return View(model);
                }
                model.DateCreated = DateTime.Now;
                _context.system_Users.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Member registered successfully!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserTypes = _context.user_Groups
                .Select(l => new { strGroupName = l.strGroupName, strId = l.strId }).ToList();

            ViewBag.SaccoList = _context.co_Operatives
                .Select(c => new { strCompanyName = c.strCompanyName, strCompanyCode = c.strCompanyCode }).ToList();

            return View(model);
        }
        // GET: System_Users/Create
        public IActionResult Create()
        {
            //string hostName = Dns.GetHostName();
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            //ViewBag.MYIP = myIP;

            //var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            //var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            //var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            //ViewBag.strCounty = strCountry;
            //ViewBag.strSubCounty = strSub_Counties;
            //ViewBag.strWord = strWards;
            utilities.SetUpPrivileges(this);

            ViewBag.UserTypes = _context.user_Groups.Select(l => l.strGroupName).ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            return View();
        }

        // POST: System_Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( System_Users system_Users)
        {
            utilities.SetUpPrivileges(this);


            //var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            //var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            //var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            //ViewBag.strCounty = strCountry;
            //ViewBag.strSubCounty = strSub_Counties;
            //ViewBag.strWord = strWards;
            ViewBag.UserTypes = _context.user_Groups.Select(l=>l.strGroupName).ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            
            //Decryptor.Decript_String(userAccount.Password);
            if (ModelState.IsValid)
            {
                system_Users.strPassword = BCrypt.Net.BCrypt.HashPassword(system_Users.strPassword);
                //system_Users.strId = "";
                _context.Add(system_Users);
                await _context.SaveChangesAsync();
                Notify("Records Saved Successfully", notificationType: NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            return View(system_Users);
        }

        // GET: System_Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            utilities.SetUpPrivileges(this);

            var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            ViewBag.strCounty = strCountry;
            ViewBag.strSubCounty = strSub_Counties;
            ViewBag.strWord = strWards;
            ViewBag.UserTypes = _context.user_Groups.Select(l => l.strGroupName).ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var system_Users = await _context.system_Users.FindAsync(id);
            if (system_Users == null)
            {
                return NotFound();
            }
            return View(system_Users);
        }

        // POST: System_Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, System_Users system_Users)
        {
            utilities.SetUpPrivileges(this);

            var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            ViewBag.strCounty = strCountry;
            ViewBag.strSubCounty = strSub_Counties;
            ViewBag.strWord = strWards;
            ViewBag.UserTypes = _context.user_Groups.Select(l => l.strGroupName).ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();

            if (id != system_Users.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    system_Users.strPassword= BCrypt.Net.BCrypt.HashPassword(system_Users.strPassword);
                    _context.Update(system_Users);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!System_UsersExists(system_Users.strId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(system_Users);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.system_Users
                .FirstOrDefaultAsync(u =>
                    u.strUserName == model.UsernameOrEmail ||
                    u.strEmail == model.UsernameOrEmail);

            if (user == null)
            {
                // Don't reveal user existence
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            // Generate token (you could store and verify this token)
            var token = Guid.NewGuid().ToString();

            // Build the reset link
            var callbackUrl = Url.Action("ResetPassword", "System_Users",
                new { token, email = user.strEmail }, Request.Scheme);

            string subject = "SACCO Password Reset";
            string body = $@"
            <p>Hello {user.strUserName},</p>
            <p>We received a request to reset your password. Please click the link below to reset it:</p>
            <p><a href='{callbackUrl}'>Reset Password</a></p>
            <p>If you didn’t request this, you can safely ignore this email.</p>
            <br />
            <p>Thank you,<br/>Your SACCO Team</p>";
            SendEmail(user.strEmail, subject, body);

            return RedirectToAction("ForgotPasswordConfirmation");
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();  
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("A token and email are required for password reset.");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {          

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.system_Users
                .FirstOrDefaultAsync(u => u.strEmail == model.Email);

            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            user.strPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _context.system_Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("ResetPasswordConfirmation");
        }
        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("notifications@cargen.com", "lwqhgbyqbucukzei"), // Use secure credentials
                EnableSsl = true,
            };

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
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email Sending Failed: " + ex.Message);
            }
        }
        // GET: System_Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var system_Users = await _context.system_Users
                .FirstOrDefaultAsync(m => m.strId == id);
            if (system_Users == null)
            {
                return NotFound();
            }

            return View(system_Users);
        }

        // POST: System_Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            utilities.SetUpPrivileges(this);

            var system_Users = await _context.system_Users.FindAsync(id);
            _context.system_Users.Remove(system_Users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool System_UsersExists(int id)
        {
            utilities.SetUpPrivileges(this);

            return _context.system_Users.Any(e => e.strId == id);
        }
    }
}
