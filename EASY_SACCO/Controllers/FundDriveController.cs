using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EASY_SACCO.Utils;

namespace EASY_SACCO.Controllers
{
    public class FundDriveController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public FundDriveController(BOSA_DBContext context, INotyfService notyf) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);

            var projects = _context.Projects
                .Include(p => p.FundDriveAllocations)
                .ToList();

            return View(projects);
        }

        public IActionResult AllocatedFunds()
        {
            utilities.SetUpPrivileges(this);

            var allocations = _context.FundDriveAllocations
                .Include(a => a.Project)
                .ToList();

            var members = _context.MembersRegistrations.ToList(); // Get all members

            var grouped = allocations
                .GroupBy(a => new { a.ProjectId, a.MemberNo })
                .Select(g =>
                {
                    var project = g.First().Project;
                    var memberNo = g.Key.MemberNo;
                    var member = members.FirstOrDefault(m => m.strMemberNo == memberNo); // Manual match

                    var totalContribution = g.Sum(x => x.ContributionAmount);
                    var totalProjectContributions = allocations
                        .Where(x => x.ProjectId == g.Key.ProjectId)
                        .Sum(x => x.ContributionAmount);

                    var ownership = totalProjectContributions > 0
                        ? (totalContribution / totalProjectContributions) * 100
                        : 0;

                    return new FundDriveAllocationViewModel
                    {
                        Project = project,
                        MemberNo = memberNo,
                        MemberName = member != null ? member.strSurName + " " + member.strOtherName : "Unknown",
                        ContributionAmount = totalContribution,
                        OwnershipPercentage =Convert.ToDouble(ownership),
                        AllocationDate = g.Max(x => x.AllocationDate)
                    };
                })
                .ToList();

            return View(grouped);
        }

        public IActionResult Details(int projectId)
        {
            utilities.SetUpPrivileges(this);

            if (projectId == 0)
            {
                TempData["Error"] = "Invalid project selection!";
                return RedirectToAction("Index");
            }

            var project = _context.Projects
                .Include(p => p.ProjectSubscriptions)
                .Include(p => p.FundDriveAllocations)
                .FirstOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                TempData["Error"] = "Project not found!";
                return RedirectToAction("Index");
            }

            ViewBag.ProjectName = project.ProjectName;
            ViewBag.ProjectId = project.Id;

            if (!project.FundDriveAllocations.Any())
            {
                ViewBag.Message = "No allocations found.";
                return View("Details", new List<FundDriveAllocation>());
            }

            return View("Details", project.FundDriveAllocations.ToList());
        }

        [HttpGet]
        public IActionResult AllocateFunds(int projectId)
        {
            utilities.SetUpPrivileges(this);
            return AllocateFundsPost(projectId);
        }

        [HttpPost]
        public IActionResult AllocateFundsPost(int projectId)
        {
            utilities.SetUpPrivileges(this);

            var project = _context.Projects
                .Include(p => p.ProjectSubscriptions)
                .FirstOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                TempData["Error"] = "Project not found!";
                return RedirectToAction("Index");
            }

            var totalFunds = project.ProjectSubscriptions.Sum(s => s.ContributionAmount);

            if (totalFunds == 0)
            {
                TempData["Error"] = "No funds have been contributed yet.";
                return RedirectToAction("Details", new { projectId });
            }

            var allocations = project.ProjectSubscriptions.Select(s => new FundDriveAllocation
            {
                ProjectId = projectId,
                MemberNo = s.MemberNo,
                ContributionAmount = s.ContributionAmount,
                OwnershipPercentage = (s.ContributionAmount / totalFunds) * 100,
                AllocationDate = DateTime.Now
            }).ToList();

            _context.FundDriveAllocations.AddRange(allocations);
            _context.SaveChanges();

            TempData["Success"] = "Funds allocated successfully!";
            return RedirectToAction("Details", new { projectId });
        }
    }
}
