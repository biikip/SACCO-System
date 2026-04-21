using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EASY_SACCO.Controllers
{
    public class WithdrawalNoticeController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public WithdrawalNoticeController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        // GET: withdrawal notice
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.withdrawalNotices = _context.withdrawalNotices.ToList();
            return View(general);
        }

        // GET: Agents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var agents = await _context.withdrawalNotices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agents == null)
            {
                return NotFound();
            }

            return View(agents);
        }

        // GET: Agents/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);


            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Id, [Bind("Id, Memberno, BookingDate, noticeperiod, ExpWithdrawDate, Reason, WithdrawDate, withdrawalFee, Transactionno, CompanyCode")] WithdrawalNotice withdrawalNotice)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            if (ModelState.IsValid)
            {
                var withdrawalnoticess = _context.withdrawalNotices.Find(Id);
                if (withdrawalnoticess != null)
                {
                    ViewBag.name = withdrawalNotice.strOtherName + "  " + withdrawalNotice.strSurName;
                    ViewBag.memberno = (withdrawalNotice.Memberno).Trim();

                }
                withdrawalNotice.CompanyCode=sacco;
                _context.Add(withdrawalNotice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(withdrawalNotice);
        }

        // GET: Agents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var withdrawalNotice = await _context.withdrawalNotices.FindAsync(id);
            if (withdrawalNotice == null)
            {
                return NotFound();
            }
            return View(withdrawalNotice);
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Memberno, BookingDate, noticeperiod, ExpWithdrawDate, Reason, WithdrawDate, withdrawalFee, Transactionno, CompanyCode")] WithdrawalNotice withdrawalNoticess)
        {
            utilities.SetUpPrivileges(this);
            if (id != withdrawalNoticess.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(withdrawalNoticess);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!withdrawalExists(withdrawalNoticess.Id))
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
            return View(withdrawalNoticess);
        }

        // GET: Agents/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var withdraw = await _context.withdrawalNotices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (withdraw == null)
            {
                return NotFound();
            }

            return View(withdraw);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            utilities.SetUpPrivileges(this);

            var withdrawal = await _context.withdrawalNotices.FindAsync(id);
            _context.withdrawalNotices.Remove(withdrawal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool withdrawalExists(int id)
        {
            utilities.SetUpPrivileges(this);

            return _context.withdrawalNotices.Any(e => e.Id == id);
        }
        //[HttpPost]
        //public JsonResult getmembers([FromBody] WithdrawalNotice WithdrawalNotice, string filter)
        //{
        //    utilities.SetUpPrivileges(this);
        //    if (filter != null)
        //    {
        //    }
        //        var sacco = HttpContext.Session.GetString("CompanyCode");

        //        var members = _context.withdrawalNotices.Where(i => i.CompanyCode.ToUpper().Equals(sacco.ToUpper())).ToList();


        //        if (!string.IsNullOrEmpty(filter))
        //        {
        //            if (!string.IsNullOrEmpty(condition))
        //            {
        //                if (condition == "MemberNo")
        //                {
        //                    members = members.Where(i => i.Memberno.ToUpper().Contains(filter.ToUpper())).ToList();
        //                }
        //                if (condition == "Name")
        //                {
        //                    members = members.Where(i => i.strOtherName.ToUpper().Contains(filter.ToUpper()) || i.strSurName.ToUpper().Contains(filter.ToUpper())).ToList();
        //                }

        //            }
        //        }

        //        members = members.OrderByDescending(i => i.Memberno).Take(15).ToList();
        //        return Json(members);
            
        //    }
        }
    }

