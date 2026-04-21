using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using Microsoft.AspNetCore.Http;
using Syncfusion.EJ2.Grids;
using EASY_SACCO.Utils;

namespace EASY_SACCO.Controllers
{
    public class AccountSetupGLsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public AccountSetupGLsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        // GET: AccountSetupGLs
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

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
            utilities.SetUpPrivileges(this);

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
            utilities.SetUpPrivileges(this);

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
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString("CompanyCode");
            ViewBag.UserTypes = _context.user_Groups.Select(l => l.strGroupName).ToList();
            ViewBag.strAccountType = _context.accountTypes.Select(l=>l.strAccountType).ToList();
            ViewBag.strAccoutGroup = _context.accountGroups.Select(l => l.strAccoutGroup).ToList();
            ViewBag.strAccountSubGroup = _context.accountSubGroups.Select(l => l.strAccountSubGroup).ToList();
            ViewBag.strCurrency = _context.Currencies.Select(l => l.strCurrency).ToList();
            ViewBag.strAcCategory = _context.accountCategories.Where(b=>b.Saccocode == sacco).Select(l => l.strAcCategory).ToList();
            ViewBag.strAccSubCategory = _context.accountSubCategories.Select(l => l.strAccSubCategory).ToList();
            ViewBag.strAccountType1 = _context.AccountType1s.Select(l => l.strAccountType1).ToList();
            ViewBag.strCompanyName = _context.co_Operatives.ToList();
            ViewBag.strNormalBal = _context.normalBalances.ToList();

            return View();
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
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
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
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            //          var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(auditid))
                return Redirect("~/");
            accountSetupGL.strCompanyCode = sacco;
            accountSetupGL.strAccountSubGroup = "No";
            accountSetupGL.strAccountSubcategory = "No";
            accountSetupGL.strAccounttypename = "No";
            accountSetupGL.strOpeningBal = "0";

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
        public async Task<IActionResult> Edit(int id)
        {
            utilities.SetUpPrivileges(this);

            // Ensure Data is in the Correct Format (Text and Value)
            ViewBag.strAccountType = _context.accountTypes.Select(l => l.strAccountType).ToList();
            ViewBag.strAccoutGroup = _context.accountGroups.Select(l => l.strAccoutGroup).ToList();
            ViewBag.strCurrency = _context.Currencies.Select(l => l.strCurrency).ToList();
            ViewBag.strAcCategory = _context.accountCategories.Select(l => l.strAcCategory).ToList();
            ViewBag.strNormalBal = _context.normalBalances.Select(l => l.strNormalBal).ToList();
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
            utilities.SetUpPrivileges(this);

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
            utilities.SetUpPrivileges(this);

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
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            //          var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(auditid))
                return Redirect("~/");
            accountSetupGL.strCompanyCode = sacco;
            accountSetupGL.strAccountSubGroup = "No";
            accountSetupGL.strAccountSubcategory = "No";
            accountSetupGL.strAccounttypename = "No";
            accountSetupGL.strOpeningBal = "0";
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
            utilities.SetUpPrivileges(this);

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
            utilities.SetUpPrivileges(this);

            var accountSetupGL = await _context.accountSetupGLs.FindAsync(id);
            _context.accountSetupGLs.Remove(accountSetupGL);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountSetupGLExists(string id)
        {
            utilities.SetUpPrivileges(this);

            return _context.accountSetupGLs.Any(e => e.strId.ToString() == id.ToString());
        }
    }
}
