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
    public class Sub_CountyController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public Sub_CountyController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Sub_County
        public async Task<IActionResult> Index()
        {
            return View(await _context.sub_Counties.ToListAsync());
        }

        // GET: Sub_County/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_County = await _context.sub_Counties
                .FirstOrDefaultAsync(m => m.strId.ToString()     == id);
            if (sub_County == null)
            {
                return NotFound();
            }

            return View(sub_County);
        }

        // GET: Sub_County/Create
        public async Task<IActionResult> Create(string id)
        {
            ViewBag.CountyId = id;
       
            
            ViewBag.sub_CountiesList = _context.sub_Counties.Where(k => k.strCountyId == id).ToList();
            return View();
        }

        // POST: Sub_County/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strCountyId,strSubCountyName")] Sub_County sub_County,string id)
        {
            ViewBag.CountyId = id;
            if (ModelState.IsValid)
            {
                _context.Add(sub_County);
                await _context.SaveChangesAsync();
                return Redirect("~/Sub_County/Create/"+id);
            }
            return View(sub_County);
        }

        // GET: Sub_County/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_County = await _context.sub_Counties.FindAsync(id);
            if (sub_County == null)
            {
                return NotFound();
            }
            return View(sub_County);
        }

        // POST: Sub_County/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strCountyId,strSubCountyName")] Sub_County sub_County)
        {
            if (id != sub_County.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sub_County);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sub_CountyExists(sub_County.strId.ToString()))
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
            return View(sub_County);
        }

        // GET: Sub_County/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_County = await _context.sub_Counties
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (sub_County == null)
            {
                return NotFound();
            }

            return View(sub_County);
        }

        // POST: Sub_County/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sub_County = await _context.sub_Counties.FindAsync(id);
            _context.sub_Counties.Remove(sub_County);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Sub_CountyExists(string id)
        {
            return _context.sub_Counties.Any(e => e.strId.ToString() == id);
        }
    }
}
