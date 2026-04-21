using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EASY_SACCO.Controllers
{
    public class User_GroupController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public User_GroupController(BOSA_DBContext context, INotyfService notyf) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
        }

        // GET: User_Group
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.user_Groups.ToListAsync());
        }

        // GET: User_Group/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);
            var user_Group = await _context.user_Groups
                .FirstOrDefaultAsync(m => m.strId == id);
            if (user_Group == null)
            {
                return NotFound();
            }

            return View(user_Group);
        }

        // GET: User_Group/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: User_Group/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User_Group user_Group)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var sacco = HttpContext.Session.GetString("CompanyCode");

                user_Group.CompanyCode = sacco;
                user_Group.Setup = user_Group?.Setup ?? false;
                user_Group.Records = user_Group?.Records ?? false;
                user_Group.Transactions = user_Group?.Transactions ?? false;
                user_Group.CreditManagement = user_Group?.CreditManagement ?? false;
                user_Group.Accounting = user_Group?.Accounting ?? false;
                user_Group.Enquiries = user_Group?.Enquiries ?? false;
                user_Group.Reports = user_Group?.Reports ?? false;
                user_Group.SystemUsers = user_Group?.SystemUsers ?? false;
                user_Group.LoanTypes = user_Group?.LoanTypes ?? false;
                user_Group.ShareTypes = user_Group?.ShareTypes ?? false;
                user_Group.CigRegistration = user_Group?.CigRegistration ?? false;
                user_Group.BankSetup = user_Group?.BankSetup ?? false;
                user_Group.ChargesSetup = user_Group?.ChargesSetup ?? false;
                user_Group.MembershipRegistration = user_Group?.MembershipRegistration ?? false;
                user_Group.SaccoRegistration = user_Group?.SaccoRegistration ?? false;
                user_Group.RecruitmentAgents = user_Group?.RecruitmentAgents ?? false;
                user_Group.Beneficiaries = user_Group?.Beneficiaries ?? false;
                user_Group.MemberWithdrawal = user_Group?.MemberWithdrawal ?? false;
                user_Group.Receipting = user_Group?.Receipting ?? false;
                user_Group.JournalPosting = user_Group?.JournalPosting ?? false;
                user_Group.LoanApplications = user_Group?.LoanApplications ?? false;
                user_Group.Appraisals = user_Group?.Appraisals ?? false;
                user_Group.Endorsements = user_Group?.Endorsements ?? false;
                user_Group.LoanSchedule = user_Group?.LoanSchedule ?? false;
                user_Group.ReceiptPosting = user_Group?.ReceiptPosting ?? false;
                user_Group.ReprintMemberReceipt = user_Group?.ReprintMemberReceipt ?? false;
                user_Group.TransfersAndOffsettings = user_Group?.TransfersAndOffsettings ?? false;
                user_Group.ShareTransfer = user_Group?.ShareTransfer ?? false;
                user_Group.ShareToLoanOffsetting = user_Group?.ShareToLoanOffsetting ?? false;
                user_Group.DividendsProcessing = user_Group?.DividendsProcessing ?? false;
                user_Group.DividendsPayment = user_Group?.DividendsPayment ?? false;
                user_Group.TransactionsManagement = user_Group?.TransactionsManagement ?? false;
                user_Group.AccountSetup = user_Group?.AccountSetup ?? false;
                user_Group.GLInquiry = user_Group?.GLInquiry ?? false;
                user_Group.BookOfAccounts = user_Group?.BookOfAccounts ?? false;
                user_Group.SharesInquiry = user_Group?.SharesInquiry ?? false;
                user_Group.LoansInquiry = user_Group?.LoansInquiry ?? false;
                user_Group.CigsPerSacco = user_Group?.CigsPerSacco ?? false;
                user_Group.MembersPerSacco = user_Group?.MembersPerSacco ?? false;
                user_Group.MembersPerCounty = user_Group?.MembersPerCounty ?? false;
                user_Group.ProjectAndInvestmentManagement = user_Group?.ProjectAndInvestmentManagement ?? false;
                user_Group.ProjectAndInvestments = user_Group?.ProjectAndInvestments ?? false;
                user_Group.FundDrives = user_Group?.FundDrives ?? false;
                user_Group.SubscribeToProject = user_Group?.SubscribeToProject ?? false;

                _context.Add(user_Group);
                await _context.SaveChangesAsync();
                Notify("Records Saved Successfully", notificationType: NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(user_Group);
            }
        }
        // GET: User_Group/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);
            var user_Group = await _context.user_Groups.FindAsync(id);
            if (user_Group == null)
            {
                return NotFound();
            }
            return View(user_Group);
        }

        // POST: User_Group/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int strId, User_Group user_Groups)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var sacco = HttpContext.Session.GetString("CompanyCode");
                var role = HttpContext.Session.GetString("UserGroup");

                user_Groups.CompanyCode = sacco;
                if (role.Equals(user_Groups.strGroupName))
                {
                    Notify("Permission denied!!!", notificationType: NotificationType.success);

                    return View(user_Groups);
                }
                if (strId != user_Groups.strId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {                   
                    _context.user_Groups.Update(user_Groups);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!User_GroupExists(user_Groups.strId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(user_Groups);
        }

        // GET: User_Group/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);
            var user_Group = await _context.user_Groups
                .FirstOrDefaultAsync(m => m.strId == id);
            if (user_Group == null)
            {
                return NotFound();
            }

            return View(user_Group);
        }

        // POST: User_Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            utilities.SetUpPrivileges(this);
            var user_Group = await _context.user_Groups.FindAsync(id);
            _context.user_Groups.Remove(user_Group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool User_GroupExists(int id)
        {
            utilities.SetUpPrivileges(this);
            return _context.user_Groups.Any(e => e.strId == id);
        }
    }
}
