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
    public class AccountTypesController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public AccountTypesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: AccountTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.accountTypes.ToListAsync());
        }

        // GET: AccountTypes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountType = await _context.accountTypes
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountType == null)
            {
                return NotFound();
            }

            return View(accountType);
        }

        // GET: AccountTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strAccountType")] AccountType accountType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountType);
        }

        // GET: AccountTypes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountType = await _context.accountTypes.FindAsync(id);
            if (accountType == null)
            {
                return NotFound();
            }
            return View(accountType);
        }

        // POST: AccountTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strAccountType")] AccountType accountType)
        {
            if (id != accountType.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountTypeExists(accountType.strId.ToString()))
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
            return View(accountType);
        }

        // GET: AccountTypes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountType = await _context.accountTypes
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountType == null)
            {
                return NotFound();
            }

            return View(accountType);
        }

        // POST: AccountTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var accountType = await _context.accountTypes.FindAsync(id);
            _context.accountTypes.Remove(accountType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountTypeExists(string id)
        {
            return _context.accountTypes.Any(e => e.strId.ToString() == id);
        }
    }
}
