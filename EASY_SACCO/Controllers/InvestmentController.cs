using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using NToastNotify;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EASY_SACCO.Controllers
{
    public class InvestmentController : BaseController
    {
        private readonly IToastNotification _toastNotification;

        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public InvestmentController(BOSA_DBContext context, INotyfService notyf) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var projects = _context.Projects.Where(p => p.IsActive).ToList();
            return View(projects);
        }
        [HttpGet]
        public IActionResult Subscribe(int projectId)
        {
            utilities.SetUpPrivileges(this);

            var project = _context.Projects.Find(projectId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        public IActionResult Subscribe(int projectId, string memberno, decimal amount)
        {
            utilities.SetUpPrivileges(this);

            // Fetch project along with subscriptions
            var project = _context.Projects.Include(p => p.ProjectSubscriptions).FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }

            
            var existingSubscription = _context.ProjectSubscriptions
                .FirstOrDefault(s => s.MemberNo == memberno && s.ProjectId == projectId);
            if (existingSubscription != null)
            {
                Notify("You are already subscribed to this project!", notificationType: NotificationType.error);
                return RedirectToAction("Index");
            }

            
            var Names = _context.MembersRegistrations.FirstOrDefault(m => m.strMemberNo == memberno);
            if (Names == null)
            {
                Notify("Member not found!", notificationType: NotificationType.error);
                return RedirectToAction("Index");
            }

            
            var subscription = new ProjectSubscription
            {
                MemberNo = memberno,
                ProjectId = projectId,
                ContributionAmount = amount,
                SubscriptionDate = DateTime.Now,
                Names = Names.strSurName + " " + Names.strOtherName
            };

            _context.ProjectSubscriptions.Add(subscription);
            _context.SaveChanges(); // ✅ Now `TotalFundsRaised` updates automatically

            Notify("Subscription successful!", notificationType: NotificationType.success);
            return RedirectToAction("Index");
        }
        public IActionResult FundDrives()
        {
            utilities.SetUpPrivileges(this);
            var fundDrives = _context.FundDrives.Include(f => f.Project).ToList();
            return View(fundDrives);
        }

        public IActionResult CreateFundDrive()
        {
            utilities.SetUpPrivileges(this);
            ViewBag.Projects = _context.Projects.Where(p => p.IsActive).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFundDrive(int projectId, decimal amount)
        {
            utilities.SetUpPrivileges(this);
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            var fundDrive = new FundDrive
            {
                ProjectId = projectId,
                TotalCollected = amount,
                DateHeld = DateTime.Now
            };

            _context.FundDrives.Add(fundDrive);
            await _context.SaveChangesAsync();

            // Distribute funds to members based on contribution
            var members = _context.MemberInvestments.Where(m => m.ProjectId == projectId).ToList();
            decimal totalProjectContribution = members.Sum(m => m.TotalContributed);

            if (totalProjectContribution > 0)
            {
                foreach (var member in members)
                {
                    member.OwnershipPercentage = (member.TotalContributed / totalProjectContribution) * 100;
                    _context.MemberInvestments.Update(member);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("FundDrives");
        }

        [HttpGet]
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                Notify("Project Saved Successfully", notificationType: NotificationType.success);
                return RedirectToAction("Index"); 
            }
            return View(project);
        }
    }
}