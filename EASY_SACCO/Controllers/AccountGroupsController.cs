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
    public class AccountGroupsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        public AccountGroupsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        // GET: AccountGroups
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            return View(await _context.accountGroups.ToListAsync());
        }

        // GET: AccountGroups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var accountGroup = await _context.accountGroups
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountGroup == null)
            {
                return NotFound();
            }

            return View(accountGroup);
        }

        // GET: AccountGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AccountGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strAccoutGroup")] AccountGroup accountGroup)
        {
            utilities.SetUpPrivileges(this);

            if (ModelState.IsValid)
            {
                _context.Add(accountGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountGroup);
        }

        // GET: AccountGroups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var accountGroup = await _context.accountGroups.FindAsync(id);
            if (accountGroup == null)
            {
                return NotFound();
            }
            return View(accountGroup);
        }

        // POST: AccountGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strAccoutGroup")] AccountGroup accountGroup)
        {
            utilities.SetUpPrivileges(this);

            if (id != accountGroup.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountGroupExists(accountGroup.strId.ToString()))
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
            return View(accountGroup);
        }

        // GET: AccountGroups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var accountGroup = await _context.accountGroups
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (accountGroup == null)
            {
                return NotFound();
            }

            return View(accountGroup);
        }

        // POST: AccountGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            utilities.SetUpPrivileges(this);

            var accountGroup = await _context.accountGroups.FindAsync(id);
            _context.accountGroups.Remove(accountGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountGroupExists(string id)
        {
            utilities.SetUpPrivileges(this);

            return _context.accountGroups.Any(e => e.strId.ToString() == id);
        }
    }
}
