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
    public class WhatToChangesController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public WhatToChangesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: WhatToChanges
        public async Task<IActionResult> Index()
        {
            return View(await _context.whatToChanges.ToListAsync());
        }

        // GET: WhatToChanges/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whatToChange = await _context.whatToChanges
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (whatToChange == null)
            {
                return NotFound();
            }

            return View(whatToChange);
        }

        // GET: WhatToChanges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WhatToChanges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strWhatToChange")] WhatToChange whatToChange)
        {
            if (ModelState.IsValid)
            {
                _context.Add(whatToChange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(whatToChange);
        }

        // GET: WhatToChanges/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whatToChange = await _context.whatToChanges.FindAsync(id);
            if (whatToChange == null)
            {
                return NotFound();
            }
            return View(whatToChange);
        }

        // POST: WhatToChanges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strWhatToChange")] WhatToChange whatToChange)
        {
            if (id != whatToChange.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(whatToChange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WhatToChangeExists(whatToChange.strId.ToString()))
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
            return View(whatToChange);
        }

        // GET: WhatToChanges/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var whatToChange = await _context.whatToChanges
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (whatToChange == null)
            {
                return NotFound();
            }

            return View(whatToChange);
        }

        // POST: WhatToChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var whatToChange = await _context.whatToChanges.FindAsync(id);
            _context.whatToChanges.Remove(whatToChange);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WhatToChangeExists(string id)
        {
            return _context.whatToChanges.Any(e => e.strId.ToString() == id);
        }
    }
}
