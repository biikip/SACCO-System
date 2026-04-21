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
    public class RelationShipsController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public RelationShipsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: RelationShips
        public async Task<IActionResult> Index()
        {
            return View(await _context.relationShips.ToListAsync());
        }

        // GET: RelationShips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationShip = await _context.relationShips
                .FirstOrDefaultAsync(m => m.strId == id);
            if (relationShip == null)
            {
                return NotFound();
            }

            return View(relationShip);
        }

        // GET: RelationShips/Create
        public IActionResult Create()
        {
         

            return View();
        }

        // POST: RelationShips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strRelationship")] RelationShip relationShip)
        {
            ViewBag.strRelationship = _context.relationShips.ToList();
            if (ModelState.IsValid)
            {
                _context.Add(relationShip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(relationShip);
        }

        // GET: RelationShips/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationShip = await _context.relationShips.FindAsync(id);
            if (relationShip == null)
            {
                return NotFound();
            }
            return View(relationShip);
        }

        // POST: RelationShips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("strId,strRelationship")] RelationShip relationShip)
        {
            if (id != relationShip.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relationShip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelationShipExists(relationShip.strId))
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
            return View(relationShip);
        }

        // GET: RelationShips/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationShip = await _context.relationShips
                .FirstOrDefaultAsync(m => m.strId == id);
            if (relationShip == null)
            {
                return NotFound();
            }

            return View(relationShip);
        }

        // POST: RelationShips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var relationShip = await _context.relationShips.FindAsync(id);
            _context.relationShips.Remove(relationShip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelationShipExists(int id)
        {
            return _context.relationShips.Any(e => e.strId == id);
        }
    }
}
