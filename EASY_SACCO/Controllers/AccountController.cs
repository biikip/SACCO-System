using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EASY_SACCO.Controllers
{
    public class AccountController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public AccountController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
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
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { token, email = user.strEmail }, Request.Scheme);

            // TODO: Send email here (SMTP or email service)
            // await _emailService.SendResetLink(user.strEmail, callbackUrl);

            return RedirectToAction("ForgotPasswordConfirmation");
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

            // Hash password (or just assign directly if you're not hashing)
            user.strPassword = model.NewPassword;

            _context.system_Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("ResetPasswordConfirmation");
        }


        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    }
}