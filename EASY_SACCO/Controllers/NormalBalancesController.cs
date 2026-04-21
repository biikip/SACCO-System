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
    public class NormalBalancesController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public NormalBalancesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: NormalBalances
        public async Task<IActionResult> Index()
        {
            return View(await _context.normalBalances.ToListAsync());
        }

        // GET: NormalBalances/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var normalBalance = await _context.normalBalances
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (normalBalance == null)
            {
                return NotFound();
            }

            return View(normalBalance);
        }

        // GET: NormalBalances/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NormalBalances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strNormalBal")] NormalBalance normalBalance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(normalBalance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(normalBalance);
        }

        // GET: NormalBalances/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var normalBalance = await _context.normalBalances.FindAsync(id);
            if (normalBalance == null)
            {
                return NotFound();
            }
            return View(normalBalance);
        }

        // POST: NormalBalances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strNormalBal")] NormalBalance normalBalance)
        {
            if (id != normalBalance.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(normalBalance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NormalBalanceExists(normalBalance.strId.ToString()))
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
            return View(normalBalance);
        }

        // GET: NormalBalances/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var normalBalance = await _context.normalBalances
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (normalBalance == null)
            {
                return NotFound();
            }

            return View(normalBalance);
        }

        // POST: NormalBalances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var normalBalance = await _context.normalBalances.FindAsync(id);
            _context.normalBalances.Remove(normalBalance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NormalBalanceExists(string id)
        {
            return _context.normalBalances.Any(e => e.strId.ToString() == id);
        }
    }
}
