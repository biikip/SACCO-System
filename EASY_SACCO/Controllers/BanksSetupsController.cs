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
    public class BanksSetupsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public BanksSetupsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        // GET: BanksSetups
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.banksSetups = await _context.banksSetups.ToListAsync();
            general.accountSetupGLs = await _context.accountSetupGLs.ToListAsync();
            general.accountType1s = await _context.accountType1S.ToListAsync();
            return View(general);
        }

        // GET: BanksSetups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var banksSetup = await _context.banksSetups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (banksSetup == null)
            {
                return NotFound();
            }

            return View(banksSetup);
        }

        // GET: BanksSetups/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            ViewData["strAccountType1"] = new SelectList(_context.accountType1S, "strId", "strAccountType1");

            ViewBag.strAccountNo = _context.accountSetupGLs.ToList();
            ViewBag.strAccountType = _context.accountType1S
    .Select(x => new {
        strAccountType = x.strAccountType1  // Name used for both text & value
    })
    .ToList();

            return View();
        }

        // POST: BanksSetups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strBankCode,strBankAccno,strPhoneNo,strAssociatedGL,strBranchName,strBankName,strAccountType,strAddress")] BanksSetup banksSetup)
        {
            utilities.SetUpPrivileges(this);
            ViewData["strAccountType1"] = new SelectList(_context.accountType1S, "strId", "strAccountType1");

            ViewBag.strAccountType = _context.accountType1S.ToList();
            ViewBag.strAccountNo = _context.accountSetupGLs.ToList();
            if (ModelState.IsValid)
            {
                _context.Add(banksSetup);
                await _context.SaveChangesAsync();
                Notify("Records Saved Successfully", notificationType: NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            return View(banksSetup);
        }

        // GET: BanksSetups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);
            ViewData["strAccountType1"] = new SelectList(_context.accountType1S, "strId", "strAccountType1");

            ViewBag.strAccountNo = _context.accountSetupGLs.ToList();
            ViewBag.strAccountType = _context.accountType1S
    .Select(x => new
    {
        strAccountType = x.strAccountType1,
        strId = x.strId
    })
    .ToList();
            if (id == null)
            {
                return NotFound();
            }

            var banksSetup = await _context.banksSetups.FindAsync(id);
            if (banksSetup == null)
            {
                return NotFound();
            }
            return View(banksSetup);
        }

        // POST: BanksSetups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, BanksSetup banksSetup)
        {
            utilities.SetUpPrivileges(this);
            ViewData["strAccountType1"] = new SelectList(_context.accountType1S, "strId", "strAccountType1");
            ViewBag.strAccountNo = _context.accountSetupGLs.ToList();
            ViewBag.strAccountType = _context.accountType1S.ToList();

            if (id != banksSetup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch existing record
                    var existingBank = await _context.banksSetups.FindAsync(id);
                    if (existingBank == null)
                    {
                        return NotFound();
                    }

                    // Manually update only the allowed fields
                    existingBank.strBankCode = banksSetup.strBankCode;
                    existingBank.strBankAccno = banksSetup.strBankAccno;
                    existingBank.strPhoneNo = banksSetup.strPhoneNo;
                    existingBank.strAssociatedGL = banksSetup.strAssociatedGL;
                    existingBank.strBranchName = banksSetup.strBranchName;
                    existingBank.strBankName = banksSetup.strBankName;
                    existingBank.strAccountType = banksSetup.strAccountType;
                    existingBank.strAddress = banksSetup.strAddress;

                    _context.Update(existingBank);
                    await _context.SaveChangesAsync();

                    Notify("Records Updated Successfully", notificationType: NotificationType.success);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BanksSetupExists(banksSetup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(banksSetup);
        }

        // GET: BanksSetups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var banksSetup = await _context.banksSetups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (banksSetup == null)
            {
                return NotFound();
            }

            return View(banksSetup);
        }

        // POST: BanksSetups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            utilities.SetUpPrivileges(this);

            var banksSetup = await _context.banksSetups.FindAsync(id);
            _context.banksSetups.Remove(banksSetup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BanksSetupExists(int? id)
        {
            utilities.SetUpPrivileges(this);

            return _context.banksSetups.Any(e => e.Id == id);
        }
    }
}
