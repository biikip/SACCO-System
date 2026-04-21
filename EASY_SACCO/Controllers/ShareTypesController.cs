using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;

namespace EASY_SACCO.Controllers
{
    public class ShareTypesController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public ShareTypesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        // GET: ShareTypes
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.ShareTypes = _context.shareTypes.ToList();
            general.co_Operatives = _context.co_Operatives.ToList();
            return View(general);
        }

        // GET: ShareTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var shareType = await _context.shareTypes
                .FirstOrDefaultAsync(m => m.strId == id);
            if (shareType == null)
            {
                return NotFound();
            }

            return View(shareType);
        }

        // GET: ShareTypes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            ViewBag.glaccno= _context.accountSetupGLs.ToList();
            ViewBag.strCompany = _context.co_Operatives.ToList();
            return View();
        }

        // POST: ShareTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strSharecode,strShareType,strMinimum,strloanstoshareratio,strCompany,strismainshares,strwithrawable,strCanB_OffletLoan,strCanB_Garantee,GLaccno")] ShareType shareType)
        {
            utilities.SetUpPrivileges(this);
            ViewBag.glaccno = _context.accountSetupGLs.ToList();
            ViewBag.strCompany = _context.co_Operatives.ToList();
            if (ModelState.IsValid)
            {
                _context.Add(shareType);
                await _context.SaveChangesAsync();
                Notify("Records Saved Successfully", notificationType: NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            return View(shareType);
        }

        // GET: ShareTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);
            ViewBag.glaccno = _context.accountSetupGLs.ToList();
            ViewBag.strCompany = _context.co_Operatives.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var shareType = await _context.shareTypes.FindAsync(id);
            if (shareType == null)
            {
                return NotFound();
            }
            return View(shareType);
        }

        // POST: ShareTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("strId,strSharecode,strShareType,strMinimum,strloanstoshareratio,strCompany,strismainshares,strwithrawable,strCanB_OffletLoan,strCanB_Garantee,GLaccno")] ShareType shareType)
        {
            utilities.SetUpPrivileges(this);
            ViewBag.glaccno = _context.accountSetupGLs.ToList();
            ViewBag.strCompany = _context.co_Operatives.ToList();
            if (id != shareType.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shareType);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShareTypeExists(shareType.strId))
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
            return View(shareType);
        }

        // GET: ShareTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var shareType = await _context.shareTypes
                .FirstOrDefaultAsync(m => m.strId == id);
            if (shareType == null)
            {
                return NotFound();
            }

            return View(shareType);
        }

        // POST: ShareTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            utilities.SetUpPrivileges(this);
            
            var shareType = await _context.shareTypes.FindAsync(id);
            var sharetypeExist = _context.Customer_Balance.Where(k => k.Transactioncode == shareType.strSharecode);
            if (sharetypeExist != null)
            {
                Notify("Sorry, The Sharetype is Already Being used, It cannot be deleted!!!", notificationType: NotificationType.error);
                return RedirectToAction(nameof(Index));
            }
            _context.shareTypes.Remove(shareType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShareTypeExists(int? id)
        {
            utilities.SetUpPrivileges(this);

            return _context.shareTypes.Any(e => e.strId == id);
        }
    }
}
