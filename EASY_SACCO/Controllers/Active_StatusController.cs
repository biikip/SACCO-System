using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


using EASY_SACCO.Context;
using EASY_SACCO.Models;
using Microsoft.EntityFrameworkCore;

namespace EASY_SACCO.Controllers
{
    public class Active_StatusController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public Active_StatusController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Active_Status
        public async Task<IActionResult> Index()
        {
            return View(await _context.Active_Statuses.ToListAsync());
        }

        // GET: Active_Status/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var active_Status = await _context.Active_Statuses
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (active_Status == null)
            {
                return NotFound();
            }

            return View(active_Status);
        }

        // GET: Active_Status/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Active_Status/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strStatus")] Active_Status active_Status)
        {
            if (ModelState.IsValid)
            {
                _context.Add(active_Status);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(active_Status);
        }

        // GET: Active_Status/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var active_Status = await _context.Active_Statuses.FindAsync(id);
            if (active_Status == null)
            {
                return NotFound();
            }
            return View(active_Status);
        }

        // POST: Active_Status/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strStatus")] Active_Status active_Status)
        {
            if (id != active_Status.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(active_Status);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Active_StatusExists(active_Status.strId.ToString()))
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
            return View(active_Status);
        }

        // GET: Active_Status/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var active_Status = await _context.Active_Statuses
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (active_Status == null)
            {
                return NotFound();
            }

            return View(active_Status);
        }

        // POST: Active_Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var active_Status = await _context.Active_Statuses.FindAsync(id);
            _context.Active_Statuses.Remove(active_Status);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Active_StatusExists(string id)
        {
            return _context.Active_Statuses.Any(e => e.strId.ToString() == id);
        }
    }
}
