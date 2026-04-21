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
using Microsoft.AspNetCore.Http;

namespace EASY_SACCO.Controllers
{
    public class SYSPARAMController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        public SYSPARAMController(BOSA_DBContext context) : base(context)
        {
            _context = context;
             utilities = new Utilities(context);
        }

        // GET: SYSPARAM
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.sYSPARAMs = await _context.SYSPARAM.ToListAsync();
            return View(general);
        }

        // GET: Beneficiaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var sysparam = await _context.SYSPARAM
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sysparam == null)
            {
                return NotFound();
            }

            return View(sysparam);
        }

        // GET: Beneficiaries/Create
        public IActionResult Create(string id)
        {
            utilities.SetUpPrivileges(this);

            //ViewBag.strRelationship = _context.relationShips.ToList();

            //ViewBag.BeneficiaryId = id;


            //ViewBag.SYSPARAMList = _context.SYSPARAM.Where(k => k.strMemberNo == id).ToList();
            return View();
        }

        // POST: Beneficiaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SYSPARAM sYSPARAM)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            var SaccoExists = _context.SYSPARAM.Any(i => i.CompanyCode == sacco);
            if (!SaccoExists)
            {
                if (sYSPARAM != null)
                {
                    var newrecords = new SYSPARAM
                    {

                        Id = 0,
                        CompanyCode = sacco,
                        CompanyName = sYSPARAM.CompanyName,
                        MinGuarantors = sYSPARAM.MinGuarantors,
                        MaxGuarantors = sYSPARAM.MaxGuarantors,
                        fax = sYSPARAM.fax,
                        telephone = sYSPARAM.telephone,
                        AuditID = auditid,
                        PhysicalAddress = sYSPARAM.PhysicalAddress,
                        address = sYSPARAM.address,
                        email = sYSPARAM.email,
                        town = sYSPARAM.town,
                        Website = sYSPARAM.Website,
                        withdrawalnotice = sYSPARAM.withdrawalnotice,
                        maturity = sYSPARAM.maturity,
                        MinAge = sYSPARAM.MinAge

                    };
                    _context.SYSPARAM.Add(newrecords);

                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                TempData["ErrorMessage"] = "sorry, the Society parameters for the sacco already Exist!";
                return View();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Beneficiaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);

          if(id == null) 
            { 
                return NotFound();
            }

            var sysparam = await _context.SYSPARAM.FindAsync(id);
           
            if (sysparam == null)
            {
                return NotFound();
            }
            return View(sysparam);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SYSPARAM ySPARAM)
        {
            utilities.SetUpPrivileges(this);

         
            if (ySPARAM == null)
            {
                return NotFound();
            }
            else {
                _context.Entry(ySPARAM).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                Notify("Records Updated Successfully!", notificationType: NotificationType.success);

            }
  
            return RedirectToAction(nameof(Index));

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(sYSPARAM);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!sysparamExists(sYSPARAM.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}

        }

        // GET: Beneficiaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);
            var sysparam = await _context.SYSPARAM
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sysparam == null)
            {
                return NotFound();
            }

            return View(sysparam);
        }

        // POST: Beneficiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            utilities.SetUpPrivileges(this);
            var sysparam = await _context.SYSPARAM.FindAsync(id);
            _context.SYSPARAM.Remove(sysparam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool sysparamExists(int id)
        {
            utilities.SetUpPrivileges(this);
            return _context.SYSPARAM.Any(e => e.Id == id);
        }
    }
}
