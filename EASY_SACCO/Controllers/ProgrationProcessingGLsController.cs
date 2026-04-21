using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Syncfusion.EJ2.Linq;
using Syncfusion.EJ2.PivotView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static EASY_SACCO.ViewModels.AccountingVm;
using static System.Net.WebRequestMethods;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EASY_SACCO.Controllers
{
    public class ProgrationProcessingGLsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        public ProgrationProcessingGLsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
        }
        // GET: AccountSetupGLs
        public async Task<IActionResult> Index()
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            //          var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(auditid))
                return Redirect("~/");
            General general = new General();
            general.accountSetupGLs =await _context.accountSetupGLs.ToListAsync();
            general.accountTypes = await _context.accountTypes.ToListAsync();
            general.accountGroups = await _context.accountGroups.ToListAsync();
            general.accountCategories = await _context.accountCategories.Where(b=>b.Saccocode == sacco).ToListAsync();
            general.co_Operatives = await _context.co_Operatives.Where(v=>v.strCompanyCode== sacco).ToListAsync();
            general.normalBalances = await _context.normalBalances.ToListAsync();
            

            return View(general);
        }
        public async Task<IActionResult> IndexCategories()
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            //          var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(auditid))
                return Redirect("~/");
            var general = await _context.accountCategories.Where(b=>b.Saccocode== sacco).ToListAsync();
            return View(general);
        }
        // GET: AccountSetupGLs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSetupGL = await _context.accountSetupGLs
                .FirstOrDefaultAsync(m => m.strId.ToString() == id.ToString());
            if (accountSetupGL == null)
            {
                return NotFound();
            }

            return View(accountSetupGL);
        }

        // GET: AccountSetupGLs/Create
        public IActionResult Create()
        {
            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";
            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            ViewBag.glAccounts = _context.ProgrationProcessingOptions.ToList();

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> save( DateTime transDate, decimal dcapital, decimal wtax, string payopt, decimal deposits)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            decimal ratio = 0; decimal depositsmade = 0; decimal waited = 0; decimal shareinterestamount = 0; decimal tax =0;
            string sharetype = _context.ProgrationProcessingOptions.FirstOrDefault(s => s.Name.Trim() == payopt.Trim()).ShareType ?? "";
            var startDate = new DateTime(transDate.Year, 1, 1);
            var enDate = startDate.AddMonths(11).AddDays(-1);
            var assignlist = new List<ProgrationProcessing>();
            var getmberslist = _context.MembersRegistrations.Where(n=>n.strCompanyCode ==sacco)
                .Select(c=>c.strMemberNo.Trim().ToLower()).ToList();
            var getopenningshares = _context.Customer_Balance.Where(v=>v.CompanyCode == sacco 
            && getmberslist.Contains(v.Memberno.Trim().ToLower())).ToList();//D003
            getopenningshares.GroupBy(b => b.Memberno).ToList().ForEach(z =>
            {
                var getonemember = z.FirstOrDefault();
                    var openningshares = z.Where(b => b.TransDate < startDate).Sum(l => l.Amount);
                var listofsharespermonth = z.Where(c => c.TransDate >= startDate).GroupBy(x => x.TransDate.ToString("MM"));
                listofsharespermonth.ForEach(f =>
                {
                    int myInt = int.Parse(f.Key);
                    ratio = (12 - myInt);
                    ratio = Math.Round(ratio / 12, 2, MidpointRounding.AwayFromZero);
                    depositsmade = f.Sum(l => l.Amount);
                    waited = depositsmade * ratio ;
                    shareinterestamount = waited * deposits/100;
                    tax = shareinterestamount * wtax / 100;
                   _context.ProgrationProcessing.Add( new ProgrationProcessing
                    {
                        MNo = z.Key, 
                        Date = f.FirstOrDefault().TransDate,
                        Ration = ratio, 
                        Deposits = depositsmade, 
                        PercentageUsed = deposits, 
                        ShareType = sharetype,
                        Weighted = waited, 
                        Gross=0, 
                        Tax= tax, 
                        Netpay=0,
                        AuditDate=DateTime.Now,
                        Saccocode = sacco
                    });
                });      
            });
            //intakes = intakes.OrderByDescending(i => i.TransDate).ToList();
            await _context.SaveChangesAsync();
            return Json("");
        }

        //accountCategories
        public IActionResult CreateCategories()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategories(AccountCategory accountCategory)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                accountCategory.Saccocode = sacco;
                _context.Add(accountCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexCategories));
            }
            return View(accountCategory);
        }
        // POST: AccountSetupGLs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountSetupGL accountSetupGL)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            //          var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(auditid))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            accountSetupGL.strCompanyCode = sacco;
            accountSetupGL.strAccountSubGroup = "No";
            accountSetupGL.strAccountSubcategory = "No";
            accountSetupGL.strAccounttypename = "No";

            ViewBag.strAccountType = _context.accountTypes.ToList();
            ViewBag.strAccoutGroup = _context.accountGroups.ToList();
            ViewBag.strAccountSubGroup = _context.accountSubGroups.ToList();
            ViewBag.strCurrency = _context.Currencies.ToList();
            ViewBag.strAcCategory = _context.accountCategories.ToList();
            ViewBag.strAccSubCategory = _context.accountSubCategories.ToList();
            ViewBag.strAccountType1 = _context.AccountType1s.ToList();
            ViewBag.strCompanyName = _context.co_Operatives.ToList();
            ViewBag.strNormalBal = _context.normalBalances.ToList();
            if (ModelState.IsValid)
            {
                _context.Add(accountSetupGL);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountSetupGL);
        }

        // GET: AccountSetupGLs/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.strAccountType = _context.accountTypes.ToList();
            ViewBag.strAccoutGroup = _context.accountGroups.ToList();
            ViewBag.strAccountSubGroup = _context.accountSubGroups.ToList();
            ViewBag.strCurrency = _context.Currencies.ToList();
            ViewBag.strAcCategory = _context.accountCategories.ToList();
            ViewBag.strAccSubCategory = _context.accountSubCategories.ToList();
            ViewBag.strAccountType1 = _context.AccountType1s.ToList();
            ViewBag.strCompanyName = _context.co_Operatives.ToList();
            ViewBag.strNormalBal = _context.normalBalances.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var accountSetupGL = await _context.accountSetupGLs.FindAsync(id);
            if (accountSetupGL == null)
            {
                return NotFound();
            }
            return View(accountSetupGL);
        }
        //EditCategory
        public async Task<IActionResult> EditCategory(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSetupGL = await _context.accountCategories.FindAsync(id);
            if (accountSetupGL == null)
            {
                return NotFound();
            }
            return View(accountSetupGL);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(long id, AccountCategory accountCategory)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            //          var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(auditid))
                return Redirect("~/");
            accountCategory.Saccocode = sacco;
            if (id != accountCategory.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountSetupGLExists(accountCategory.strId.ToString()))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexCategories));
            }
            return View(accountCategory);
        }

        // POST: AccountSetupGLs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AccountSetupGL accountSetupGL)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            //          var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(auditid))
                return Redirect("~/");
            accountSetupGL.strCompanyCode = sacco;
            accountSetupGL.strAccountSubGroup = "No";
            accountSetupGL.strAccountSubcategory = "No";
            accountSetupGL.strAccounttypename = "No";
            ViewBag.strAccountType = _context.accountTypes.ToList();
            ViewBag.strAccoutGroup = _context.accountGroups.ToList();
            ViewBag.strAccountSubGroup = _context.accountSubGroups.ToList();
            ViewBag.strCurrency = _context.Currencies.ToList();
            ViewBag.strAcCategory = _context.accountCategories.Where(v=>v.Saccocode == sacco).ToList();
            ViewBag.strAccSubCategory = _context.accountSubCategories.ToList();
            ViewBag.strAccountType1 = _context.AccountType1s.ToList();
            ViewBag.strCompanyName = _context.co_Operatives.ToList();
            ViewBag.strNormalBal = _context.normalBalances.ToList();
            if (id != accountSetupGL.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountSetupGL);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountSetupGLExists(accountSetupGL.strId.ToString()))
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
            return View(accountSetupGL);
        }

        // GET: AccountSetupGLs/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSetupGL = await _context.accountSetupGLs
                .FirstOrDefaultAsync(m => m.strId.ToString() == id.ToString());
            if (accountSetupGL == null)
            {
                return NotFound();
            }

            return View(accountSetupGL);
        }

        // POST: AccountSetupGLs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var accountSetupGL = await _context.accountSetupGLs.FindAsync(id);
            _context.accountSetupGLs.Remove(accountSetupGL);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountSetupGLExists(string id)
        {
            return _context.accountSetupGLs.Any(e => e.strId.ToString() == id.ToString());
        }
    }
}
