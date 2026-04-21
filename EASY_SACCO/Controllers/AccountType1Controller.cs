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
    public class AccountType1Controller : BaseController
    {
        private readonly BOSA_DBContext _context;

        public AccountType1Controller(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: AccountType1
        public async Task<IActionResult> Index()
        {
            return View(await _context.AccountType1s.ToListAsync());
        }

        // GET: AccountType1/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountType1 = await _context.AccountType1s
                .FirstOrDefaultAsync(m => m.strId == id);
            if (accountType1 == null)
            {
                return NotFound();
            }

            return View(accountType1);
        }

        // GET: AccountType1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountType1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strAccountType1")] AccountType1 accountType1)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountType1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountType1);
        }

        // GET: AccountType1/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountType1 = await _context.AccountType1s.FindAsync(id);
            if (accountType1 == null)
            {
                return NotFound();
            }
            return View(accountType1);
        }

        // POST: AccountType1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("strId,strAccountType1")] AccountType1 accountType1)
        {
            if (id != accountType1.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountType1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountType1Exists(accountType1.strId))
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
            return View(accountType1);
        }

        // GET: AccountType1/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountType1 = await _context.AccountType1s
                .FirstOrDefaultAsync(m => m.strId == id);
            if (accountType1 == null)
            {
                return NotFound();
            }

            return View(accountType1);
        }

        // POST: AccountType1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accountType1 = await _context.AccountType1s.FindAsync(id);
            _context.AccountType1s.Remove(accountType1);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountType1Exists(int id)
        {
            return _context.AccountType1s.Any(e => e.strId == id);
        }
    }
}
