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
    public class MembersDefaultsController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public MembersDefaultsController(BOSA_DBContext context) : base(context)
        {
           
            _context = context;
        }

        // GET: MembersDefaults
        public async Task<IActionResult> Index()
        {
            General general = new General();
            general.membersDefaults =await _context.membersDefaults.ToListAsync();
            general.ShareTypes =await _context.shareTypes.ToListAsync();
            return View(general);
        }

        // GET: MembersDefaults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membersDefaults = await _context.membersDefaults
                .FirstOrDefaultAsync(m => m.Id == id);
            if (membersDefaults == null)
            {
                return NotFound();
            }

            return View(membersDefaults);
        }

        // GET: MembersDefaults/Create
        public IActionResult Create()
        {
            ViewBag.strSharecode = _context.shareTypes.ToList();
            return View();
        }

        // POST: MembersDefaults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strAccNo,strAmount,strSharesCode,strContribution")] MembersDefaults membersDefaults)
        {
            ViewBag.strSharecode = _context.shareTypes.ToList();
            if (ModelState.IsValid)
            {
                _context.Add(membersDefaults);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membersDefaults);
        }

        // GET: MembersDefaults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.strSharecode = _context.shareTypes.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var membersDefaults = await _context.membersDefaults.FindAsync(id);
            if (membersDefaults == null)
            {
                return NotFound();
            }
            return View(membersDefaults);
        }

        // POST: MembersDefaults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("strId,strAccNo,strAmount,strSharesCode,strContribution")] MembersDefaults membersDefaults)
        {
            ViewBag.strSharecode = _context.shareTypes.ToList();
            if (id != membersDefaults.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membersDefaults);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembersDefaultsExists(membersDefaults.Id))
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
            return View(membersDefaults);
        }

        // GET: MembersDefaults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membersDefaults = await _context.membersDefaults
                .FirstOrDefaultAsync(m => m.Id == id);
            if (membersDefaults == null)
            {
                return NotFound();
            }

            return View(membersDefaults);
        }

        // POST: MembersDefaults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var membersDefaults = await _context.membersDefaults.FindAsync(id);
            _context.membersDefaults.Remove(membersDefaults);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembersDefaultsExists(int? id)
        {
            return _context.membersDefaults.Any(e => e.Id == id);
        }
    }
}
