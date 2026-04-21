using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



using EASY_SACCO.Context;
using EASY_SACCO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EASY_SACCO.Controllers
{
    public class Marital_StatusController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public Marital_StatusController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Marital_Status
        public async Task<IActionResult> Index()
        {
            return View(await _context.Marital_Status.ToListAsync());
        }

        // GET: Marital_Status/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marital_Status = await _context.Marital_Status
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (marital_Status == null)
            {
                return NotFound();
            }

            return View(marital_Status);
        }

        // GET: Marital_Status/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marital_Status/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strMaritalStatus")] Marital_Status marital_Status)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marital_Status);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marital_Status);
        }

        // GET: Marital_Status/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marital_Status = await _context.Marital_Status.FindAsync(id);
            if (marital_Status == null)
            {
                return NotFound();
            }
            return View(marital_Status);
        }

        // POST: Marital_Status/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strMaritalStatus")] Marital_Status marital_Status)
        {
            if (id != marital_Status.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marital_Status);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Marital_StatusExists(marital_Status.strId.ToString()))
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
            return View(marital_Status);
        }

        // GET: Marital_Status/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marital_Status = await _context.Marital_Status
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (marital_Status == null)
            {
                return NotFound();
            }

            return View(marital_Status);
        }

        // POST: Marital_Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var marital_Status = await _context.Marital_Status.FindAsync(id);
            _context.Marital_Status.Remove(marital_Status);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Marital_StatusExists(string id)
        {
            return _context.Marital_Status.Any(e => e.strId.ToString() == id);
        }
    }
}
