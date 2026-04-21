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
    public class CollateralsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public CollateralsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        // GET: Collaterals
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            return View(await _context.collaterals.ToListAsync());
        }

        // GET: Collaterals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var collaterals = await _context.collaterals
                .FirstOrDefaultAsync(m => m.strId == id);
            if (collaterals == null)
            {
                return NotFound();
            }

            return View(collaterals);
        }

        // GET: Collaterals/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);

            return View();
        }

        // POST: Collaterals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strCode,strDesc,strPercentageTaken")] Collaterals collaterals)
        {
            utilities.SetUpPrivileges(this);

            if (ModelState.IsValid)
            {
                _context.Add(collaterals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collaterals);
        }

        // GET: Collaterals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var collaterals = await _context.collaterals.FindAsync(id);
            if (collaterals == null)
            {
                return NotFound();
            }
            return View(collaterals);
        }

        // POST: Collaterals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("strId,strCode,strDesc,strPercentageTaken")] Collaterals collaterals)
        {
            utilities.SetUpPrivileges(this);

            if (id != collaterals.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collaterals);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollateralsExists(collaterals.strId))
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
            return View(collaterals);
        }

        // GET: Collaterals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var collaterals = await _context.collaterals
                .FirstOrDefaultAsync(m => m.strId == id);
            if (collaterals == null)
            {
                return NotFound();
            }

            return View(collaterals);
        }

        // POST: Collaterals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            utilities.SetUpPrivileges(this);

            var collaterals = await _context.collaterals.FindAsync(id);
            _context.collaterals.Remove(collaterals);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollateralsExists(int? id)
        {
            utilities.SetUpPrivileges(this);

            return _context.collaterals.Any(e => e.strId == id);
        }
    }
}
