using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;

namespace EASY_SACCO.Controllers
{
    public class AccountCodesController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public AccountCodesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        // GET: AccountCodes
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.accountCodes= await _context.accountCodes.ToListAsync();
            general.accountTypes = await _context.accountTypes.ToListAsync();
            general.accountGroups = await _context.accountGroups.ToListAsync();



            return View(general);
        }

        // GET: AccountCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var accountCodes = await _context.accountCodes
                .FirstOrDefaultAsync(m => m.strId == id);
            if (accountCodes == null)
            {
                return NotFound();
            }

            return View(accountCodes);
        }

        // GET: AccountCodes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strAccoutGroup = _context.accountGroups.ToList();
            ViewBag.strAccountType = _context.accountTypes.ToList();
            
            return View();
        }

        // POST: AccountCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strCategoryID,strMainGroup,strAccountGroup,strDescriptions")] AccountCodes accountCodes)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strAccoutGroup = _context.accountGroups.ToList();
            ViewBag.strAccountType = _context.accountTypes.ToList();
           
            if (ModelState.IsValid)
            {
                _context.Add(accountCodes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountCodes);
        }

        // GET: AccountCodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strAccoutGroup = _context.accountGroups.ToList();
            ViewBag.strAccountType = _context.accountTypes.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var accountCodes = await _context.accountCodes.FindAsync(id);
            if (accountCodes == null)
            {
                return NotFound();
            }
            return View(accountCodes);
        }

        // POST: AccountCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("strId,strCategoryID,strMainGroup,strAccountGroup,strDescriptions")] AccountCodes accountCodes)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strAccoutGroup = _context.accountGroups.ToList();
            ViewBag.strAccountType = _context.accountTypes.ToList();
            if (id != accountCodes.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountCodes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountCodesExists(accountCodes.strId))
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
            return View(accountCodes);
        }

        // GET: AccountCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var accountCodes = await _context.accountCodes
                .FirstOrDefaultAsync(m => m.strId == id);
            if (accountCodes == null)
            {
                return NotFound();
            }

            return View(accountCodes);
        }

        // POST: AccountCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            utilities.SetUpPrivileges(this);

            var accountCodes = await _context.accountCodes.FindAsync(id);
            _context.accountCodes.Remove(accountCodes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountCodesExists(int? id)
        {
            utilities.SetUpPrivileges(this);

            return _context.accountCodes.Any(e => e.strId == id);
        }
    }
}
