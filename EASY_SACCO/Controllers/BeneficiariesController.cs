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
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EASY_SACCO.Controllers
{
    public class BeneficiariesController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public BeneficiariesController(BOSA_DBContext context, INotyfService notyf) : base(context)
        {
            _context = context;
             utilities = new Utilities(context);
            _notyf = notyf;

        }

        // GET: Beneficiaries
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.beneficiaries = await _context.beneficiaries.ToListAsync();
            general.membersRegistrations = await _context.MembersRegistrations.ToListAsync();
            general.relationShips = await _context.relationShips.ToListAsync();
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

            var beneficiaries = await _context.beneficiaries
                .FirstOrDefaultAsync(m => m.strId == id);
            if (beneficiaries == null)
            {
                return NotFound();
            }

            return View(beneficiaries);
        }

        // GET: Beneficiaries/Create
        public IActionResult Create(int id)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strRelationship = _context.relationShips.ToList();

            ViewBag.BeneficiaryId = id;


            ViewBag.beneficiaryList = _context.beneficiaries.Where(k => k.strMemberNo == id.ToString()).ToList();
            return View();
        }

        // POST: Beneficiaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strMemberNo,strKinNo,strRelationShip,strTelNo,strSignDate,strPercentage,strWithnessBy,strKinNames,strAddress,strOfficeTelNo,strBirthCert,strComments")] Beneficiaries beneficiaries,int id)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strRelationship = _context.relationShips.ToList();
            ViewBag.BeneficiaryId = id;
            if (ModelState.IsValid)
            {
                _context.Add(beneficiaries);
                await _context.SaveChangesAsync();
                Notify("Records Saved Successfully", notificationType: NotificationType.success);
                return RedirectToAction(nameof(Index));
                
            }           
            return View(beneficiaries);
        }

        // GET: Beneficiaries/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            utilities.SetUpPrivileges(this);
            ViewBag.strRelationship = _context.relationShips.ToList();

            ViewBag.BeneficiaryId = id;


            ViewBag.beneficiaryList = _context.beneficiaries.Where(k => k.strMemberNo == id.ToString()).ToList();
            if (id == null)
            {
                return NotFound();
            }

            var beneficiaries = await _context.beneficiaries.FindAsync(id);
            if (beneficiaries == null)
            {
                return NotFound();
            }
            return View(beneficiaries);
        }

        // POST: Beneficiaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("strId,strMemberNo,strKinNo,strRelationShip,strTelNo,strSignDate,strPercentage,strWithnessBy,strKinNames,strAddress,strOfficeTelNo,strBirthCert,strComments")] Beneficiaries beneficiaries)
        {
            utilities.SetUpPrivileges(this);
            ViewBag.strRelationship = _context.relationShips.ToList();

            ViewBag.BeneficiaryId = id;


            if (id != beneficiaries.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beneficiaries);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficiariesExists(beneficiaries.strId))
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
            return View(beneficiaries);
        }

        // GET: Beneficiaries/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);
            var beneficiaries = await _context.beneficiaries
                .FirstOrDefaultAsync(m => m.strId == id);
            if (beneficiaries == null)
            {
                return NotFound();
            }

            return View(beneficiaries);
        }

        // POST: Beneficiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            utilities.SetUpPrivileges(this);
            var beneficiaries = await _context.beneficiaries.FindAsync(id);
            _context.beneficiaries.Remove(beneficiaries);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeneficiariesExists(int id)
        {
            utilities.SetUpPrivileges(this);
            return _context.beneficiaries.Any(e => e.strId == id);
        }
    }
}
