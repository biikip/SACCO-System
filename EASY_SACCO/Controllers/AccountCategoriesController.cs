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
    public class AccountCategoriesController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public AccountCategoriesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
        }

        // GET: AccountCategories
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            return View(await _context.accountCategories.ToListAsync());
        }

        // GET: AccountCategories/Details/5
        public async Task<IActionResult> Details(long id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var accountCategory = await _context.accountCategories
                .FirstOrDefaultAsync(m => m.strId == id);
            if (accountCategory == null)
            {
                return NotFound();
            }

            return View(accountCategory);
        }

        // GET: AccountCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strAcCategory")] AccountCategory accountCategory)
        {
            if (ModelState.IsValid)
            {
                utilities.SetUpPrivileges(this);

                _context.Add(accountCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountCategory);
        }

        // GET: AccountCategories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var accountCategory = await _context.accountCategories.FindAsync(id);
            if (accountCategory == null)
            {
                return NotFound();
            }
            return View(accountCategory);
        }

        // POST: AccountCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("strId,strAcCategory")] AccountCategory accountCategory)
        {
            if (id != accountCategory.strId)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountCategoryExists(accountCategory.strId))
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
            return View(accountCategory);
        }

        // GET: AccountCategories/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var accountCategory = await _context.accountCategories
                .FirstOrDefaultAsync(m => m.strId == id);
            if (accountCategory == null)
            {
                return NotFound();
            }

            return View(accountCategory);
        }

        // POST: AccountCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            utilities.SetUpPrivileges(this);

            var accountCategory = await _context.accountCategories.FindAsync(id);
            _context.accountCategories.Remove(accountCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountCategoryExists(long id)
        {
            utilities.SetUpPrivileges(this);

            return _context.accountCategories.Any(e => e.strId == id);
        }
    }
}
