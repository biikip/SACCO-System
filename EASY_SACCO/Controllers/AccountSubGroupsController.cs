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
    public class AccountSubGroupsController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public AccountSubGroupsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: AccountSubGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.accountSubGroups.ToListAsync());
        }

        // GET: AccountSubGroups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSubGroup = await _context.accountSubGroups
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountSubGroup == null)
            {
                return NotFound();
            }

            return View(accountSubGroup);
        }

        // GET: AccountSubGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountSubGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strAccountSubGroup")] AccountSubGroup accountSubGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountSubGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountSubGroup);
        }

        // GET: AccountSubGroups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSubGroup = await _context.accountSubGroups.FindAsync(id);
            if (accountSubGroup == null)
            {
                return NotFound();
            }
            return View(accountSubGroup);
        }

        // POST: AccountSubGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strAccountSubGroup")] AccountSubGroup accountSubGroup)
        {
            if (id != accountSubGroup.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountSubGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountSubGroupExists(accountSubGroup.strId.ToString()))
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
            return View(accountSubGroup);
        }

        // GET: AccountSubGroups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountSubGroup = await _context.accountSubGroups
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountSubGroup == null)
            {
                return NotFound();
            }

            return View(accountSubGroup);
        }

        // POST: AccountSubGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var accountSubGroup = await _context.accountSubGroups.FindAsync(id);
            _context.accountSubGroups.Remove(accountSubGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountSubGroupExists(string id)
        {
            return _context.accountSubGroups.Any(e => e.strId.ToString() == id);
        }
    }
}
