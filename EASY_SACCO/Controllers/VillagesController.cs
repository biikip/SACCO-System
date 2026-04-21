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
    public class VillagesController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public VillagesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Villages
        public async Task<IActionResult> Index()
        {
            return View(await _context.villages.ToListAsync());
        }

        // GET: Villages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var villages = await _context.villages
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (villages == null)
            {
                return NotFound();
            }

            return View(villages);
        }

        // GET: Villages/Create
        public async Task<IActionResult> Create(string id)
        { var CountyId = "";
            var subcountyId = "";

            var ward = await _context.wards.FirstOrDefaultAsync(k => k.strId.ToString() == id);
            if (ward != null)
            {
                CountyId = ward.strCountyId;
                subcountyId = ward.strSubCountId;

            }
            ViewBag.CountyId = CountyId;
            ViewBag.subcountyId = subcountyId;
            ViewBag.wardId = id;
            ViewBag.Villages = _context.villages.Where(k => k.strWardId == id).ToList();
            return View();
        }

        // POST: Villages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strCountyId,strSubCountId,strWardId,strVillageName")] Villages villages,string id)
        {
            var CountyId = "";
            var subcountyId = "";

            var ward = await _context.wards.FirstOrDefaultAsync(k => k.strId.ToString() == id);
            if (ward != null)
            {
                CountyId = ward.strCountyId;
                subcountyId = ward.strSubCountId;

            }
            ViewBag.CountyId = CountyId;
            ViewBag.subcountyId = subcountyId;
            ViewBag.wardId = id;
            ViewBag.Villages = _context.villages.Where(k => k.strWardId == id).ToList();
            if (ModelState.IsValid)
            {
                _context.Add(villages);
                await _context.SaveChangesAsync();
                return Redirect("~/Villages/Create/" + id);
            }
            return View(villages);
        }

        // GET: Villages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var villages = await _context.villages.FindAsync(id);
            if (villages == null)
            {
                return NotFound();
            }
            return View(villages);
        }

        // POST: Villages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strCountyId,strSubCountId,strWardId,strVillageName")] Villages villages)
        {
            if (id != villages.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(villages);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VillagesExists(villages.strId.ToString()))
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
            return View(villages);
        }

        // GET: Villages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var villages = await _context.villages
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (villages == null)
            {
                return NotFound();
            }

            return View(villages);
        }

        // POST: Villages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var villages = await _context.villages.FindAsync(id);
            _context.villages.Remove(villages);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VillagesExists(string id)
        {
            return _context.villages.Any(e => e.strId.ToString() == id);
        }
    }
}
