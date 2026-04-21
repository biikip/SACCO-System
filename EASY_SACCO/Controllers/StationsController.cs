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
    public class StationsController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public StationsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Stations
        public async Task<IActionResult> Index()
        {
            return View(await _context.stations.ToListAsync());
        }

        // GET: Stations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stations = await _context.stations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stations == null)
            {
                return NotFound();
            }

            return View(stations);
        }

        // GET: Stations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strStationName")] Stations stations)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stations);
        }

        // GET: Stations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stations = await _context.stations.FindAsync(id);
            if (stations == null)
            {
                return NotFound();
            }
            return View(stations);
        }

        // POST: Stations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("strId,strStationName")] Stations stations)
        {
            if (id != stations.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StationsExists(stations.Id))
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
            return View(stations);
        }

        // GET: Stations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stations = await _context.stations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stations == null)
            {
                return NotFound();
            }

            return View(stations);
        }

        // POST: Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var stations = await _context.stations.FindAsync(id);
            _context.stations.Remove(stations);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StationsExists(int? id)
        {
            return _context.stations.Any(e => e.Id == id);
        }
    }
}
