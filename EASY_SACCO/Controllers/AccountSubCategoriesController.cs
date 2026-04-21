using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;

namespace EASY_SACCO.Controllers
{
    public class AccountSubCategoriesController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public AccountSubCategoriesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: AccountSubCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.accountSubCategories.ToListAsync());
        }

        // GET: AccountSubCategories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSubCategory = await _context.accountSubCategories
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountSubCategory == null)
            {
                return NotFound();
            }

            return View(accountSubCategory);
        }

        // GET: AccountSubCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountSubCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strAccSubCategory")] AccountSubCategory accountSubCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountSubCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountSubCategory);
        }

        // GET: AccountSubCategories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSubCategory = await _context.accountSubCategories.FindAsync(id);
            if (accountSubCategory == null)
            {
                return NotFound();
            }
            return View(accountSubCategory);
        }

        // POST: AccountSubCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strAccSubCategory")] AccountSubCategory accountSubCategory)
        {
            if (id != accountSubCategory.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountSubCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountSubCategoryExists(accountSubCategory.strId.ToString()))
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
            return View(accountSubCategory);
        }

        // GET: AccountSubCategories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSubCategory = await _context.accountSubCategories
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountSubCategory == null)
            {
                return NotFound();
            }

            return View(accountSubCategory);
        }

        // POST: AccountSubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var accountSubCategory = await _context.accountSubCategories.FindAsync(id);
            _context.accountSubCategories.Remove(accountSubCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountSubCategoryExists(string id)
        {
            return _context.accountSubCategories.Any(e => e.strId.ToString() == id);
        }
    }
}
