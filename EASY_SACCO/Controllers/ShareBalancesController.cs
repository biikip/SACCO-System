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
    public class ShareBalancesController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public ShareBalancesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: ShareBalances
        public async Task<IActionResult> Index()
        {
            General general = new General();
            general.shareBalances = await _context.shareBalances.ToListAsync();
            general.ShareTypes = await _context.shareTypes.ToListAsync();
            return View(general);
           
        }

        // GET: ShareBalances/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shareBalance = await _context.shareBalances
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (shareBalance == null)
            {
                return NotFound();
            }

            return View(shareBalance);
        }

        // GET: ShareBalances/Create
        public IActionResult Create()
        {

            ViewBag.strShareType = _context.shareTypes.ToList();
            return View();
        }

        // POST: ShareBalances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strShareType,strDescription,strDate,strBalance")] ShareBalance shareBalance)
        {
            ViewBag.strShareType = _context.shareTypes.ToList();
            if (ModelState.IsValid)
            {
                _context.Add(shareBalance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shareBalance);
        }

        // GET: ShareBalances/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.strShareType = _context.shareTypes.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var shareBalance = await _context.shareBalances.FindAsync(id);
            if (shareBalance == null)
            {
                return NotFound();
            }
            return View(shareBalance);
        }

        // POST: ShareBalances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strShareType,strDescription,strDate,strBalance")] ShareBalance shareBalance)
        {
            ViewBag.strShareType = _context.shareTypes.ToList();
            if (id != shareBalance.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shareBalance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShareBalanceExists(shareBalance.strId.ToString()))
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
            return View(shareBalance);
        }

        // GET: ShareBalances/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shareBalance = await _context.shareBalances
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (shareBalance == null)
            {
                return NotFound();
            }

            return View(shareBalance);
        }

        // POST: ShareBalances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var shareBalance = await _context.shareBalances.FindAsync(id);
            _context.shareBalances.Remove(shareBalance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShareBalanceExists(string id)
        {
            return _context.shareBalances.Any(e => e.strId.ToString() == id);
        }
    }
}
