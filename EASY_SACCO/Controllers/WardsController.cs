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
    public class WardsController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public WardsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Wards
        public async Task<IActionResult> Index()
        {
            return View(await _context.wards.ToListAsync());
        }

        // GET: Wards/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.wards
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // GET: Wards/Create
        public async Task<IActionResult> Create( string id)
        {
            var subCounty = await _context.sub_Counties.FirstOrDefaultAsync(k => k.strId.ToString() == id);
            var CountyId = "";
            if (subCounty != null)
            {

                CountyId = subCounty.strCountyId;

            }

            ViewBag.CountyId = CountyId;
            ViewBag.subcountyId = id;
            ViewBag.ward = _context.wards.Where(k => k.strSubCountId == id).ToList();
            return View();
        }

        // POST: Wards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strCountyId,strSubCountId,strWardName")] Ward ward,string id)
        {
            var subCounty = await _context.sub_Counties.FirstOrDefaultAsync(k => k.strId.ToString() == id);
            var CountyId = "";
            if (subCounty != null)
            {
               
                    CountyId = subCounty.strCountyId;
                
            }
            ViewBag.CountyId = CountyId;
            ViewBag.subcountyId = id;
            ViewBag.ward = _context.wards.Where(k => k.strSubCountId == id).ToList();
            if (ModelState.IsValid)
            {
                _context.Add(ward);
                await _context.SaveChangesAsync();
                return Redirect("~/Wards/Create/" + id);
            }
            return View(ward);
        }

        // GET: Wards/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.wards.FindAsync(id);
            if (ward == null)
            {
                return NotFound();
            }
            return View(ward);
        }

        // POST: Wards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strCountyId,strSubCountId,strWardName")] Ward ward)
        {
            if (id != ward.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardExists(ward.strId.ToString()))
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
            return View(ward);
        }

        // GET: Wards/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.wards
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // POST: Wards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ward = await _context.wards.FindAsync(id);
            _context.wards.Remove(ward);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardExists(string id)
        {
            return _context.wards.Any(e => e.strId.ToString() == id);
        }
    }
}
