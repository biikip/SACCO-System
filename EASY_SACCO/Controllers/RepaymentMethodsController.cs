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
    public class RepaymentMethodsController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public RepaymentMethodsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: RepaymentMethods
        public async Task<IActionResult> Index()
        {
            return View(await _context.RepaymentMethods.ToListAsync());
        }

        // GET: RepaymentMethods/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repaymentMethod = await _context.RepaymentMethods
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (repaymentMethod == null)
            {
                return NotFound();
            }

            return View(repaymentMethod);
        }

        // GET: RepaymentMethods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RepaymentMethods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strMethod")] RepaymentMethod repaymentMethod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(repaymentMethod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(repaymentMethod);
        }

        // GET: RepaymentMethods/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repaymentMethod = await _context.RepaymentMethods.FindAsync(id);
            if (repaymentMethod == null)
            {
                return NotFound();
            }
            return View(repaymentMethod);
        }

        // POST: RepaymentMethods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strMethod")] RepaymentMethod repaymentMethod)
        {
            if (id != repaymentMethod.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(repaymentMethod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepaymentMethodExists(repaymentMethod.strId.ToString()))
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
            return View(repaymentMethod);
        }

        // GET: RepaymentMethods/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repaymentMethod = await _context.RepaymentMethods
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (repaymentMethod == null)
            {
                return NotFound();
            }

            return View(repaymentMethod);
        }

        // POST: RepaymentMethods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var repaymentMethod = await _context.RepaymentMethods.FindAsync(id);
            _context.RepaymentMethods.Remove(repaymentMethod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RepaymentMethodExists(string id)
        {
            return _context.RepaymentMethods.Any(e => e.strId.ToString() == id);
        }
    }
}
