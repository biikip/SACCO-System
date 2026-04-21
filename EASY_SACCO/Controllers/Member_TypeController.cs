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
    public class Member_TypeController : BaseController
    {
        private readonly BOSA_DBContext _context;

        public Member_TypeController(BOSA_DBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Member_Type
        public async Task<IActionResult> Index()
        {
            return View(await _context.Member_Type.ToListAsync());
        }

        // GET: Member_Type/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member_Type = await _context.Member_Type
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (member_Type == null)
            {
                return NotFound();
            }

            return View(member_Type);
        }

        // GET: Member_Type/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Member_Type/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strMembertype")] Member_Type member_Type)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member_Type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(member_Type);
        }

        // GET: Member_Type/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member_Type = await _context.Member_Type.FindAsync(id);
            if (member_Type == null)
            {
                return NotFound();
            }
            return View(member_Type);
        }

        // POST: Member_Type/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("strId,strMembertype")] Member_Type member_Type)
        {
            if (id != member_Type.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member_Type);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Member_TypeExists(member_Type.strId.ToString()))
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
            return View(member_Type);
        }

        // GET: Member_Type/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member_Type = await _context.Member_Type
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (member_Type == null)
            {
                return NotFound();
            }

            return View(member_Type);
        }

        // POST: Member_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var member_Type = await _context.Member_Type.FindAsync(id);
            _context.Member_Type.Remove(member_Type);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Member_TypeExists(string id)
        {
            return _context.Member_Type.Any(e => e.strId.ToString() == id);
        }
    }
}
