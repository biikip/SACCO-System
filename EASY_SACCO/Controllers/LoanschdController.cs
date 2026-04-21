using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace EASY_SACCO.Controllers
{
    public class LoanschdController : BaseController
    {
        private readonly IToastNotification _toastNotification;
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        public LoanschdController(BOSA_DBContext context, IToastNotification toastNotification) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _toastNotification = toastNotification;
        }

        // GET: Genders
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.LOANSCHD.ToListAsync());
        }

        // GET: Genders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var sched = await _context.LOANSCHD
                .FirstOrDefaultAsync(m => m.ID.ToString() == id);
            if (sched == null)
            {
                return NotFound();
            }

            return View(sched);
        }

        // GET: Genders/Create
        public IActionResult Create()
        {
            var auditid = HttpContext.Session.GetString("UserID");
            var memberno = HttpContext.Session.GetString("MemberNo");
            ViewBag.memberno = memberno;
            var loantypes = new SelectList(_context.loanTYpes, "strloancode", "strLoanType").ToList();
            ViewBag.loantypes = loantypes;
            utilities.SetUpPrivileges(this);
            return View();
        }
        [HttpPost]
        public IActionResult populateInterestrate(string LoanCode, string MemberNo)
        {
            if (string.IsNullOrEmpty(LoanCode) || string.IsNullOrEmpty(MemberNo))
            {
                return Json(new { error = "LoanCode or MemberNo is missing." });
            }

            var loancount = "";

            int loancodenoCount = _context.Customer_Balance
                .Where(k => k.Memberno == MemberNo && k.Loanno == LoanCode)
                .Count();

            if (loancodenoCount > 0)
            {
                loancount = LoanCode + MemberNo + "-" + (loancodenoCount + 1);
            }
            else
            {
                loancount = LoanCode + MemberNo;
            }

            var intrestrates = _context.loanTYpes.FirstOrDefault(k => k.strloancode == LoanCode);

            if (intrestrates == null)
            {
                return Json(new { error = "Loan type not found." });
            }

            var result = new
            {
                loancode = intrestrates.strloancode.Trim(),
                repaypriod = intrestrates.strRPeriod,
                interestrate = Math.Round(decimal.Parse(intrestrates.strInterestRate), 2),
                repaymethod = intrestrates.strRepaymentMethod?.Trim(),
                loancount = loancount,
                loantoshareratios = intrestrates.LoanToShareRatio
            };

            return Json(result);
        }
        // POST: Genders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Loanschd/GenerateSchedule")]
        public IActionResult GenerateSchedule(string MemberNo, decimal InitialAmount, decimal InterestRate, int Period, string RepaymentMethod, DateTime StartDate)
        {
            utilities.SetUpPrivileges(this);
            List<Loanschd> schedule = new List<Loanschd>();

            decimal monthlyInterestRate = InterestRate / 100 / 12;
            decimal balance = InitialAmount;
            decimal monthlyPayment = 0;

            if (RepaymentMethod == "AMRT")  // Amortized Loan
            {
                monthlyPayment = (InitialAmount * monthlyInterestRate) / (1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -Period));
                
                decimal totalRepayable = Period * monthlyPayment;
                DateTime dateDeposited = StartDate;

                for (int i = 1; i <= Period; i++)
                {
                    decimal interest = (InterestRate / 12 / 100) * balance; // Monthly interest
                    decimal principal = monthlyPayment - interest;        // Principal portion of repayment

                    balance -= principal; // Reduce balance by principal paid

                    // Round values to 2 decimal places
                    principal = Math.Round(principal, 2);
                    interest = Math.Round(interest, 2);
                    balance = Math.Round(balance, 2);

                    schedule.Add(new Loanschd
                    {
                        MemberNo = MemberNo,
                        Period = i,
                        Principal = principal,
                        Interest = interest,
                        Balance = balance,
                        Comments = "Loan Repayment Schedule",
                        FmtPer = dateDeposited.ToString("MMM yyyy")
                    });

                    dateDeposited = dateDeposited.AddMonths(1);
                }
            }
            else if (RepaymentMethod == "STL")  // Straight Line
            {
                decimal totalRepayable = InitialAmount + (InitialAmount * (InterestRate / 12 / 100) * Period);
                decimal monthlyPrincipal = InitialAmount / Period;
                DateTime dateDeposited = StartDate;

                for (int i = 1; i <= Period; i++)
                {
                    totalRepayable = balance + (balance * (InterestRate / 12 / 100) * Period);
                    monthlyPrincipal = (InitialAmount / Period);
                    decimal monthlyInterest = (InitialAmount * (InterestRate / 12 / 100));
                    decimal mAmount = monthlyPrincipal + monthlyInterest;
                    
                    totalRepayable = totalRepayable - mAmount;
                    balance = balance - monthlyPrincipal;
                    totalRepayable = Math.Round(balance, 2);
                    monthlyPrincipal = Math.Round(monthlyPrincipal, 2);
                    monthlyInterest = Math.Round(monthlyInterest, 2);

                    schedule.Add(new Loanschd
                    {
                        MemberNo = MemberNo,
                        Period = i,
                        Principal = monthlyPrincipal,
                        Interest = monthlyInterest,
                        Balance = balance,
                        Comments = "Loan Repayment Schedule",
                        FmtPer = $"{StartDate.AddMonths(i - 1):MMM yyyy}"
                    });
                }
            }
            else if (RepaymentMethod == "RBAL") // Reducing Balance
            {
                balance = InitialAmount; // Set initial loan balance
                DateTime dateDeposited = StartDate;

                for (int i = 1; i <= Period; i++)
                {
                    decimal principal = InitialAmount / Period; // Fixed principal amount
                    decimal interest = (InterestRate / 12 / 100) * balance; // Interest on reducing balance

                    balance -= principal; // Reduce balance by principal paid

                    // Round values to 2 decimal places
                    principal = Math.Round(principal, 2);
                    interest = Math.Round(interest, 2);
                    balance = Math.Round(balance, 2);

                    schedule.Add(new Loanschd
                    {
                        MemberNo = MemberNo,
                        Period = i,
                        Principal = principal,
                        Interest = interest,
                        Balance = balance,
                        Comments = "Loan Repayment Schedule",
                        FmtPer = dateDeposited.ToString("MMM yyyy")
                    });

                    dateDeposited = dateDeposited.AddMonths(1);
                }
            }


            return Json(schedule);
        }


        // GET: Genders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.LOANSCHD.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            return View(schedule);
        }

        // POST: Genders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,MemberNo,Period,Principal,Interest,Balance,Comments,FmtPer,contrib,Sharebalance,companycode")] Loanschd loanschd)
        {
            utilities.SetUpPrivileges(this);
            if (id != loanschd.ID.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loanschd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanschExists(loanschd.ID.ToString()))
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
            return View(loanschd);
        }

        // GET: Genders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var gender = await _context.LOANSCHD
                .FirstOrDefaultAsync(m => m.ID.ToString() == id);
            if (gender == null)
            {
                return NotFound();
            }

            return View(gender);
        }

        // POST: Genders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            utilities.SetUpPrivileges(this);
            var gender = await _context.LOANSCHD.FindAsync(id);
            _context.LOANSCHD.Remove(gender);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanschExists(string id)
        {
            utilities.SetUpPrivileges(this);
            return _context.LOANSCHD.Any(e => e.ID.ToString()   == id);
        }
    }
}
