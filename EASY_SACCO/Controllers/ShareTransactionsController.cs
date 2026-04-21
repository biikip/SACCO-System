using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using EASY_SACCO.Utils;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Security.Principal;
using static EASY_SACCO.Controllers.HomeController;
using Syncfusion.EJ2.Schedule;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreHero.ToastNotification.Abstractions;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;

namespace EASY_SACCO.Controllers
{
    public class LoanRepayViewModel
    {
        public List<Customer_Balance> customerBalances { get; set; }

        public int Id { get; set; }
        public string LoanNo { get; set; }
        public string Loancode { get; set; }
        public string MemberNo { get; set; }
        public string CompanyCode { get; set; }
        public DateTime DateReceived { get; set; }
        public int PaymentNo { get; set; }
        public decimal TotalRepayment { get; set; }
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Penalty { get; set; }
        public decimal LoanBalance { get; set; }
        public string ReceiptNo { get; set; }
        public string Remarks { get; set; }
        public string AuditID { get; set; }
        public DateTime nextduedate { get; set; }
        public DateTime AuditTime { get; set; }
        public string RepaymentMethod { get; set; }
        public int RepaymentPeriod { get; set; }
        public decimal InterestRate { get; set; }


    }


    public class ShareTransactionsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public ShareTransactionsController(BOSA_DBContext context, INotyfService notyf) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
        }

        // GET: ShareTransactions
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            var customerBalances = await _context.Customer_Balance
                .Select(cb => new CustomerBalanceViewModel
                {
                    DocumentNumber = cb.Documentnumber ?? "",
                    MemberNo = cb.Memberno ?? "",
                    MemberName = cb.MemberName ?? "",
                    Amount = cb.Amount,
                    CRAccNo = cb.CRaccno ?? "",
                    DRAccNo = cb.DRaccno ?? "",
                    AccountNo = cb.Accountno ?? "",
                    TransDate = cb.TransDate
                })
                .ToListAsync();

            return View(customerBalances);
        }
        [HttpGet]
        public async Task<IActionResult> GetDataTableData()
        {
            var draw = Request.Query["draw"].FirstOrDefault();
            var start = Request.Query["start"].FirstOrDefault();
            var length = Request.Query["length"].FirstOrDefault();
            var searchValue = Request.Query["search[value]"].FirstOrDefault();
            var startDate = Request.Query["StartDate"].FirstOrDefault();
            var endDate = Request.Query["EndDate"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            DateTime? startDt = string.IsNullOrEmpty(startDate) ? null : DateTime.Parse(startDate);
            DateTime? endDt = string.IsNullOrEmpty(endDate) ? null : DateTime.Parse(endDate);

            // Query your data (example)
            var query = _context.Customer_Balance.AsQueryable();

            if (startDt.HasValue && endDt.HasValue)
            {
                query = query.Where(c => c.TransDate >= startDt && c.TransDate <= endDt);
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(c =>
                    c.Documentnumber.Contains(searchValue) ||
                    c.Memberno.Contains(searchValue) ||
                    c.MemberName.Contains(searchValue) ||
                    c.Accountno.Contains(searchValue) ||
                    c.CRaccno.Contains(searchValue) ||
                    c.DRaccno.Contains(searchValue)
                );
            }

            var recordsTotal = await query.CountAsync();

            var data = await query
                .OrderByDescending(c => c.TransDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(c => new
                {
                    documentNumber = c.Documentnumber,
                    memberNo = c.Memberno,
                    memberName = c.MemberName,
                    amount = c.Amount,
                    crAccNo = c.CRaccno,
                    drAccNo = c.DRaccno,
                    accountNo = c.Accountno,
                    transDate = c.TransDate
                })
                .ToListAsync();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public IActionResult Shares_loansEnquiry()
        {
            utilities.SetUpPrivileges(this);
            return View();
            //ViewBag.shareslist= 
            //ViewBag.loaninquiry= await _context.loanAplications.Where(k=>k.status==4).ToListAsync();

        }



        public async Task<IActionResult> Statement(int? Id)
        {
            var sacco = HttpContext.Session.GetString("CompanyName");
            utilities.SetUpPrivileges(this);
            if (Id != null)
            {
                var memberno = await _context.loanAplications.FindAsync(Id);
                var names = await _context.MembersRegistrations.Where(k => k.strMemberNo == memberno.MemberNo).FirstOrDefaultAsync();

                if (memberno != null)
                {
                    var naems = names.strSurName + " " + names.strOtherName;
                    ViewBag.namss = naems;
                    ViewBag.saccos = sacco;
                    ViewBag.loans = memberno.ApprovedAmount;
                    ViewBag.loano = memberno.LoanNo;
                    ViewBag.memberno = memberno.MemberNo;
                    ViewBag.repaypriod = memberno.RepaymentPeriod;

                    return View(await _context.REPAY.Where(k => k.MemberNo == memberno.MemberNo).ToListAsync());
                }

            }
            return View();

        }

        public async Task<IActionResult> Getcontributions(int? Id)
        {
            utilities.SetUpPrivileges(this);
            if (Id != null)
            {
                var memberno = await _context.Customer_Balance.FindAsync(Id);
                if (memberno != null)
                {

                    return View(await _context.Customer_Balance.Where(k => k.Memberno == memberno.Memberno && k.Transactioncode != "Loan").ToListAsync());
                }

            }
            return View();

        }
        // GET: ShareTransactions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shareTransaction = await _context.shareTransactions
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (shareTransaction == null)
            {
                return NotFound();
            }

            return View(shareTransaction);
        }
        [HttpPost]
        public IActionResult populateDescription(string Sharecode)
        {

            var error = "erro!";
            if (Sharecode != null)
            {

                var Descrip = _context.shareTypes.FirstOrDefault(k => k.strSharecode == Sharecode);


                var myResult = new
                {
                    Descriptnames = Descrip.strShareType.Trim(),
                };

                var alldata = myResult.ToString().Replace("{", "").Replace("}", "");
                var finalldata = alldata.ToString()
                          .Replace("Descriptnames", "").Trim()

                          .Replace("=", "");

                return Json(finalldata);
            }
            return Json(error);
        }
        [HttpPost]
        public IActionResult populateNames(string Name)
        {


            if (Name != null)
            {

                var Names = _context.MembersRegistrations.FirstOrDefault(k => k.strOtherName.Contains(Name) || k.strSurName.Contains(Name));


                var myResult = new
                {
                    names = Names.strOtherName.Trim() + "  " + Names.strSurName.Trim(),
                    memberno1 = Names.strMemberNo.Trim(),


                };

                var alldata = myResult.ToString().Replace("{", "").Replace("}", "");
                var finalldata = alldata.ToString()
                          .Replace("names", "").Trim()
                          .Replace("memberno1", "").Trim()
                          .Replace("=", "");

                return Json(finalldata);
            }
            return View();
        }

        [HttpPost]
        public JsonResult Populateloandetails([FromBody] LoanAplication loanAplication, int loanId)
        {


            var sacco = HttpContext.Session.GetString("CompanyCode");
            var loans = _context.loanAplications.Find(loanId);
            var memberno = loans.MemberNo;
            var Loanno = "";
            var Loancode = "";
            int Repayperiod = 0;
            var Repaymemthod = "";
            decimal InterestRate = 0;

            loans = _context.loanAplications.Where(k => k.status == 4 && k.MemberNo == memberno).FirstOrDefault();

            Random r = new Random();
            DateTime dateTime = DateTime.Now;
            var date = dateTime.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
            var y = r.Next(10, 99);
            var finalcode = "RCP" +"-000" + y;
            var ifexist = _context.shareTransactions.FirstOrDefaultAsync(k => k.strDocnumber == finalcode);
            if (ifexist != null)
            {
                finalcode = "RCP"+"-000" + y;
            }
            else
            {
                y = r.Next(10, 99);
                finalcode = "RCP"+"-000" + y;
            }

            var Receiptno = finalcode;

            if (loans != null)
            {
                Loanno = loans.LoanNo;
                Loancode = loans.LoanCode;
                Repayperiod = loans.RepaymentPeriod ?? 0;
                Repaymemthod = loans.RepaymentMethod;
                InterestRate = loans.InterestRate;


            }

            return Json(new { Loanno, Loancode, Repayperiod, Repaymemthod, InterestRate, Receiptno });
        }

        public class LoanScheduleItem
        {
            public DateTime DueDate { get; set; }
            public double Principal { get; set; }
            public double Interest { get; set; }
            public double TotalPayment { get; set; }
            public double RemainingBalance { get; set; }
            public string Memberno { get; set; }
        }

        [HttpPost]
        public JsonResult populateloan([FromBody] Customer_Balance customer_Balance, string memberno, double principal)
        {
            var loans = _context.loanAplications.Where(k => k.MemberNo == memberno).FirstOrDefault();
            var loannos = "";
            var loancodes = "";
            var repaymethods = "";
            decimal interestrates = 0;
            int repayperiods = 0;



            var result = "Erro";

            if (loans != null)
            {
                loannos = loans.LoanNo;
                loancodes = loans.LoanCode;
                repaymethods = loans.RepaymentMethod;
                interestrates = loans.InterestRate;
                repayperiods = loans.RepaymentPeriod ?? 0;


                double remainingLoanAmount = Convert.ToDouble(loans.AppliedAmount);
                double monthlyInterestRate = Convert.ToDouble(loans.InterestRate / 12 / 100);
                int numberOfPayments = loans.RepaymentPeriod ?? 0;
                double principals = 0;
                double totalPayment = 0;


                DateTime dueDate = loans.LastRepayDate;

                for (int i = 0; i < numberOfPayments; i++)
                {
                    double interest = remainingLoanAmount * monthlyInterestRate;

                    var memberNos = loans.MemberNo;


                    if (loans.RepaymentMethod == "AMRT")
                    {


                        principals = (Convert.ToDouble(loans.AppliedAmount) * monthlyInterestRate) / (1 - Math.Pow(1 + monthlyInterestRate, -numberOfPayments));

                        principals = principals - interest;
                    }
                    else if (loans.RepaymentMethod == "RBAL")
                    {
                        interest = (Convert.ToDouble(loans.LoanBalance) * monthlyInterestRate);
                        principals = (Convert.ToDouble(loans.AppliedAmount) / numberOfPayments);
                    }
                    else if (loans.RepaymentMethod == "STL")
                    {
                        principals = (Convert.ToDouble(loans.AppliedAmount) / numberOfPayments);
                        interest = (Convert.ToDouble(loans.AppliedAmount) * monthlyInterestRate);
                    }

                    totalPayment = interest + principals;
                    remainingLoanAmount -= principals;
                }


                return Json(new { loannos, loancodes, repaymethods, interestrates, repayperiods, totalPayment });

            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult getActiveloans(string filter)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var loans = _context.loanAplications
                .Where(k => k.MemberNo == filter && k.CompanyCode == sacco && k.status == 4) // assuming 4 = Approved/Active
                .Select(k => new
                {
                    loanNo = k.LoanNo,
                    loanBalance = k.LoanBalance,
                    approvedAmount = k.ApprovedAmount,
                    repaymentPeriod = k.RepaymentPeriod,
                    interestRate = k.InterestRate,
                    repaymentMethod = k.RepaymentMethod
                })
                .ToList();

            return Json(loans);
        }
        [HttpPost]
        public JsonResult generateNextSchedule(string loanNo)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var loan = _context.loanAplications.FirstOrDefault(k => k.LoanNo == loanNo);

            if (loan != null)
            {
                var schedule = new List<LoanScheduleItem>();
                double remainingLoanAmount = Convert.ToDouble(loan.LoanBalance); // ✅ use LoanBalance now
                double monthlyInterestRate = Convert.ToDouble(loan.InterestRate / 12 / 100);
                int numberOfPayments = loan.RepaymentPeriod ?? 0;
                double principal = 0;
                DateTime dueDate = loan.LastRepayDate.AddMonths(1); // ✅ Move forward by 1 month

                double interest = remainingLoanAmount * monthlyInterestRate;

                if (loan.RepaymentMethod == "AMRT")
                {
                    // recalculate EMI
                    double emi = (remainingLoanAmount * monthlyInterestRate) / (1 - Math.Pow(1 + monthlyInterestRate, -numberOfPayments));
                    principal = emi - interest;
                }
                else if (loan.RepaymentMethod == "RBAL")
                {
                    principal = Convert.ToDouble(loan.AppliedAmount) / numberOfPayments;
                    interest = remainingLoanAmount * monthlyInterestRate;
                }
                else if (loan.RepaymentMethod == "STL")
                {
                    principal = remainingLoanAmount / numberOfPayments;
                    interest = remainingLoanAmount * monthlyInterestRate;
                }

                double totalPayment = principal + interest;
                double newRemainingBalance = remainingLoanAmount - principal;

                schedule.Add(new LoanScheduleItem
                {
                    DueDate = dueDate,
                    Principal = principal,
                    Interest = interest,
                    TotalPayment = totalPayment,
                    RemainingBalance = newRemainingBalance,
                    Memberno = loan.MemberNo
                });

                var schedules = schedule.Select(item => new
                {
                    DueDate = item.DueDate.ToShortDateString(),
                    Principal = item.Principal,
                    Interest = item.Interest,
                    TotalPayment = item.TotalPayment,
                    RemainingBalance = item.RemainingBalance,
                    Memberno = item.Memberno,
                }).ToList();

                return Json(new
                {
                    success = true,
                    schedule = schedules,
                    loanDetails = new
                    {
                        LoanCode = loan.LoanCode,
                        RepayPeriod = loan.RepaymentPeriod,
                        RepayMethod = loan.RepaymentMethod,
                        InterestRate = Math.Round(Convert.ToDecimal(loan.InterestRate), 2)
                    }
                });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public JsonResult getmemberstatement([FromBody] Customer_Balance customer_Balance, string filter)
        {


            var sacco = HttpContext.Session.GetString("CompanyCode");
            //filter = HttpContext.Session.GetString("MemberNo");
            var loans = _context.loanAplications.Where(k => k.status == 4 && k.MemberNo==filter).ToList();
            var members = _context.MembersRegistrations.Where(i => i.strCompanyCode.ToUpper().Equals(sacco.ToUpper()) && i.strMemberNo==filter).ToList();
            var Names = "";
            decimal Sharecapital = 0;
            decimal Deposit = 0;
            decimal loanbal = 0;
            int outstandingloan = 0;
            //shares
            var individualTotals = _context.Customer_Balance
             .Where(x => x.Transactioncode != "Loan" && x.Memberno==filter)
            .GroupBy(k => k.Memberno)
            .Select(group => new
            {
                Id = group.First().Id,
                MemberNo = group.First().Memberno,
                Name = group.First().MemberName,
                TotalAmount = group.Sum(k => k.Amount),

            }).ToList();
            if (filter != null)
            {
                Deposit = _context.Customer_Balance.Where(k => (k.Transactioncode.Contains("D") || k.Transdescription.Contains("DEPOSIT") || k.Transdescription.Contains("SAVING")) && k.Memberno == filter).Sum(k => k.Amount);

                Sharecapital = _context.Customer_Balance.Where(k => (k.Transactioncode.Contains("SC") || k.Transdescription.Contains("share capital") || k.Transdescription.Contains("capital")) && k.Memberno == filter).Sum(k => k.Amount);

            }

            if (!string.IsNullOrEmpty(filter))
            {

                if (filter != null)
                {
                    loans = _context.loanAplications.Where(k => k.status == 4 && k.MemberNo == filter).ToList();
                    loanbal = _context.loanAplications.Where(k => k.MemberNo == filter && k.status == 4).Sum(k => k.LoanBalance) ?? 0;
                    outstandingloan = _context.loanAplications.Where(k => k.MemberNo == filter && k.LoanBalance > 0 && k.status==4).Count();
                    members = _context.MembersRegistrations.Where(i => i.strCompanyCode.ToUpper().Equals(sacco.ToUpper()) && i.strMemberNo == filter).ToList();
                    Names = members.FirstOrDefault().strSurName + " " + members.FirstOrDefault().strOtherName;

                    individualTotals = individualTotals.Where(i => i.MemberNo == filter).ToList();
                }


            }
            individualTotals = individualTotals.OrderByDescending(k => k.MemberNo).Take(15).ToList();


            return Json(new
            {
                Sharecapital,
                Deposit,
                Names,
                individualTotals,
                loans,
                loanbal,
                outstandingloan
            });
        }

        [HttpPost]
        public JsonResult getmembers([FromBody] MembersRegistration membersRegistration, string filter, string condition, int? memberId, string membernoss, string memberno1)
        {

            var sacco = HttpContext.Session.GetString("CompanyCode");

            var members = _context.MembersRegistrations.Where(i => i.strCompanyCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var names = "";
            var memberno = "";
            if (memberId != null)
            {
                var memberss = _context.MembersRegistrations.Find(memberId);
                names = memberss.strOtherName + "  " + memberss.strSurName;
                memberno = memberss.strMemberNo;

            }
            if (memberno1 != null)
            {
                var memberss = _context.MembersRegistrations
                    .FirstOrDefault(K => K.strMemberNo == memberno1);

                if (memberss != null)
                {
                    names = memberss.strOtherName + " " + memberss.strSurName;
                }
                else
                {
                    names = string.Empty; // or a fallback like "Unknown Member"
                }
            }

            if (!string.IsNullOrEmpty(filter))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    if (condition == "MemberNo")
                    {
                        members = members.Where(i => i.strMemberNo.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    if (condition == "Name")
                    {
                        members = members.Where(i => i.strOtherName.ToUpper().Contains(filter.ToUpper()) || i.strSurName.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    if (condition == "IdNo")
                    {
                        members = members.Where(i => i.strIdNo.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    if (condition == "PhoneNo")
                    {
                        members = members
                            .Where(i => !string.IsNullOrEmpty(i.strPhoneNo) && i.strPhoneNo.ToUpper().Contains(filter.ToUpper()))
                            .ToList();
                    }


                }
            }

            members = members.OrderByDescending(i => i.strMemberNo).Take(5).ToList();
            return Json(new { members, names, memberno });
        }
        [HttpPost]
        public async Task<IActionResult> GetSharesTransaction([FromBody] DateFilterModel filter)
        {
            if (filter == null || filter.StartDate == null || filter.EndDate == null)
            {
                return BadRequest("Invalid date range");
            }

            var data = await _context.Customer_Balance
                .Where(cb => cb.TransDate >= filter.StartDate && cb.TransDate <= filter.EndDate)
                .Select(cb => new CustomerBalanceViewModel
                {
                    DocumentNumber = cb.Documentnumber ?? "",
                    MemberNo = cb.Memberno ?? "",
                    MemberName = cb.MemberName ?? "",
                    Amount = cb.Amount,
                    CRAccNo = cb.CRaccno ?? "",
                    DRAccNo = cb.DRaccno ?? "",
                    AccountNo = cb.Accountno ?? "",
                    TransDate =Convert.ToDateTime(cb.TransDate.ToString("dd-MM-yyyy"))
                })
                .ToListAsync();

            return Json(data);
        }

        public class Member
        {
            public string MemberNo { get; set; }
            public string MemberName { get; set; }
        }

        // GET: ShareTransactions/Create
        public async Task<IActionResult> Create(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id != null)
            {
                var membersRegistration = await _context.MembersRegistrations.FindAsync(id);
                if (membersRegistration != null)
                {
                    ViewBag.name = membersRegistration.strOtherName + " " + membersRegistration.strSurName;
                    ViewBag.memberno = membersRegistration.strMemberNo;
                }
            }

            ViewBag.sharecode = new SelectList(_context.shareTypes, "strSharecode", "strShareType").ToList();
            ViewBag.strBankName = await _context.banksSetups.ToListAsync();
            ViewBag.strSharecode = await _context.shareTypes.ToListAsync();

            ViewBag.Code = await GenerateReceiptNo();

            return View();
        }

        private async Task<string> GenerateReceiptNo()
        {
            // Get current time with leading zeros
            string timePart = DateTime.Now.ToString("HHmmss"); // e.g., "033045"

            // Find the latest receipt that matches the current time
            var lastReceipt = await _context.ShareTransaction
                .Where(s => s.Docnumber.StartsWith($"RCP-{timePart}-"))
                .OrderByDescending(s => s.Docnumber)
                .Select(s => s.Docnumber)
                .FirstOrDefaultAsync();

            int lastNumber = 0;

            if (!string.IsNullOrEmpty(lastReceipt))
            {
                // Extract the last 4 digits (after last hyphen)
                var parts = lastReceipt.Split('-');
                if (parts.Length >= 3)
                {
                    int.TryParse(parts[2], out lastNumber);
                }
            }

            int newNumber = lastNumber + 1;

            // Format: RCP-HHMMSS-0001
            string newReceipt = $"RCP-{timePart}-{newNumber.ToString("D4")}";

            return newReceipt;
        }


        // POST: ShareTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShareTransaction shareTransaction)
        {


            ViewBag.strMemberNo = await _context.MembersRegistrations.ToListAsync();
            ViewBag.strBankName = await _context.banksSetups.ToListAsync();
            ViewBag.strSharecode = await _context.shareTypes.ToListAsync();
            Guid guid = Guid.NewGuid();
            shareTransaction.strAuditId = guid.ToString();
            shareTransaction.strDate = DateTime.UtcNow.AddHours(3);

            ViewBag.Code = await GenerateReceiptNo();
            if (ModelState.IsValid)
            {
                _context.Add(shareTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shareTransaction);
        }
        private async Task RefreshGuarantorsAsync(string loanNo)
        {
            try
            {
                // 1. Get the current LoanApplication record
                var loan = await _context.loanAplications
                    .FirstOrDefaultAsync(x => x.LoanNo == loanNo);

                if (loan == null)
                {
                    throw new Exception($"Loan {loanNo} not found.");
                }

                decimal loanBalance = loan.LoanBalance ?? 0;

                // 2. Get all guarantors for this loan
                var guarantors = await _context.Loanguars
                    .Where(lg => lg.LoanNo == loanNo)
                    .ToListAsync();

                if (!guarantors.Any())
                {
                    // No guarantors found, exit
                    return;
                }

                decimal totalGuaranteed = guarantors.Sum(x => x.AmountGuaranteed);

                foreach (var guarantor in guarantors)
                {
                    decimal amountGuaranteed = guarantor.AmountGuaranteed;

                    decimal updatedLoanBalance = loanBalance < 1 ? 0 : loanBalance;

                    // Recalculate each guarantor's balance
                    decimal newGuarantorBalance = totalGuaranteed > 0
                        ? (amountGuaranteed / totalGuaranteed) * updatedLoanBalance
                        : 0;

                    // Update guarantor's balance
                    guarantor.Balance = newGuarantorBalance;

                    _context.Loanguars.Update(guarantor);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error refreshing guarantors for Loan {loanNo}: {ex.Message}", ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveOrder2([FromBody] LoanRepayViewModel loanRepays)
        {
            try
            {
                var sacco = HttpContext.Session.GetString("CompanyCode");
                var auditid = HttpContext.Session.GetString("UserID");

                DateTime date = DateTime.UtcNow.AddHours(3);

                string result = "Error! Record Is Not Complete!";
                decimal Principal1 = 0;
                decimal Principal2 = 0;
                decimal Interest = 0;

                if (loanRepays.MemberNo != null)
                {
                    if (loanRepays.RepaymentMethod == "STL")
                    {
                        var lastrepaydates = _context.loanAplications.FirstOrDefault(k => k.LoanNo == loanRepays.LoanNo);
                        var Datereceived = loanRepays.DateReceived;

                        
                        var loanapplied = _context.loanAplications.Where(k => k.LoanNo == loanRepays.LoanNo).FirstOrDefault();
                        var AmountApproved = loanapplied.ApprovedAmount;

                        double totalDays = (loanRepays.DateReceived - lastrepaydates.LastRepayDate).TotalDays;

                        int monthsElapsed = (int)Math.Ceiling(Math.Max(totalDays, 1) / 45.0);

                        decimal monthlyInterestRate = loanRepays.InterestRate / 12m / 100m;

                        Interest = (AmountApproved ?? 0m) * monthlyInterestRate * monthsElapsed;



                        var loanbal = _context.loanAplications.Where(k => k.LoanNo == loanRepays.LoanNo).FirstOrDefault();
                        var loanbalance = loanbal.LoanBalance;



                        Principal1 = (AmountApproved / loanRepays.RepaymentPeriod) ?? 0;


                        var AmountRepay = loanRepays.Amount;

                        Principal2 = AmountRepay - Interest;



                        var loanbalances = (loanbalance - Principal2);


                        var norepay = _context.REPAY.Where(k => k.LoanNo == loanRepays.LoanNo).Count();
                        var list = loanRepays.customerBalances.FirstOrDefault();


                        var repay = new RepayMentTable();
                        repay.LoanNo = loanRepays.LoanNo;
                        repay.MemberNo = loanRepays.MemberNo;
                        repay.Loancode = loanRepays.Loancode;
                        repay.Principal = Principal2;
                        repay.Interest = Interest;
                        repay.TotalRepayment = (Principal2 + Interest);
                        repay.CompanyCode = sacco;
                        repay.nextduedate = date.AddMonths(1);
                        repay.Penalty = 0;
                        repay.DateReceived = loanRepays.DateReceived;
                        repay.AuditID = auditid;
                        repay.ReceiptNo = loanRepays.ReceiptNo;
                        repay.PaymentNo = norepay + 1;
                        repay.Remarks = loanRepays.Remarks.Trim();
                        repay.LoanBalance = loanbalances ?? 0;
                        _context.REPAY.Add(repay);
                        var transaction = new ShareTransactionss
                        {
                            MemberNo = loanRepays.MemberNo,
                            Docnumber = loanRepays.ReceiptNo,
                            TransAmount = loanRepays.Amount,
                            TransDate = DateTime.Now,
                            TransDescription = loanRepays.Remarks.Trim(),
                        };
                        _context.ShareTransaction.Add(transaction);
                        var updateloanapplication = _context.loanAplications.FirstOrDefault(k => k.LoanNo == repay.LoanNo);
                        if (updateloanapplication != null)
                        {
                            _context.Entry(updateloanapplication).State = EntityState.Modified;
                            updateloanapplication.LoanBalance = loanbalances;
                            updateloanapplication.LastRepayDate = loanRepays.DateReceived;
                            _context.SaveChanges();
                        }

                        _context.SaveChanges();

                        var glaccoun = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strLoanAccount;
                        var interestacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strInterestAccount;
                        var penaltyacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strPenaltyAccount;


                        var accounts = _context.Customer_Balance.FirstOrDefault(m => m.Loanno == loanRepays.LoanNo);

                        var CrAccno1 = accounts.CRaccno;
                        var bankcode = accounts.CRaccno;

                        //gltransactions
                        var newrecords = new GL_Transaction();

                        newrecords.MemberNo = loanRepays.MemberNo;
                        newrecords.TransactionDate = loanRepays.DateReceived;
                        newrecords.Amount = Principal2;
                        newrecords.strDocnumber = loanRepays.ReceiptNo;
                        newrecords.CrAccNo = glaccoun;
                        newrecords.DrAccNo = bankcode;
                        newrecords.Remarks = "Loan Repayment" + loanRepays.MemberNo;
                        newrecords.CompanyCode = sacco;
                        newrecords.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords);
                        _context.SaveChanges();



                        //gl interest
                        var newrecords1 = new GL_Transaction();
                        newrecords1.MemberNo = loanRepays.MemberNo;
                        newrecords1.TransactionDate = loanRepays.DateReceived;
                        newrecords1.Amount = Principal2;
                        newrecords1.strDocnumber = loanRepays.ReceiptNo;
                        newrecords1.CrAccNo = interestacounNo;
                        newrecords1.DrAccNo = bankcode;
                        newrecords1.Remarks = "Interest Repayment" + loanRepays.MemberNo;
                        newrecords1.CompanyCode = sacco;
                        newrecords1.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords1.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords1);
                        _context.SaveChanges();


                        //gl penalty
                        var newrecords2 = new GL_Transaction();
                        newrecords2.MemberNo = loanRepays.MemberNo;
                        newrecords2.TransactionDate = loanRepays.DateReceived;
                        newrecords2.Amount = Principal2;
                        newrecords2.strDocnumber = loanRepays.ReceiptNo;
                        newrecords2.CrAccNo = penaltyacounNo;
                        newrecords2.DrAccNo = bankcode;
                        newrecords2.Remarks = "Penalty Repayment" + loanRepays.MemberNo;
                        newrecords2.CompanyCode = sacco;
                        newrecords2.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords2.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords2);
                        _context.SaveChanges();
                        await RefreshGuarantorsAsync(loanRepays.LoanNo);
                    }

                    else if (loanRepays.RepaymentMethod == "AMRT")
                    {
                        var loanbal = _context.loanAplications.Where(k => k.LoanNo == loanRepays.LoanNo).FirstOrDefault();
                        var loanbalance = loanbal.LoanBalance;
                        //decimal remainingLoanAmount = loanbalance;

                        var lastrepaydates = _context.loanAplications.FirstOrDefault(k => k.LoanNo == loanRepays.LoanNo);
                        var Datereceived = loanRepays.DateReceived;

                        
                        int repayperiod = loanRepays.RepaymentPeriod;

                        //loanapplied
                        var loanapplied = _context.loanAplications.Where(k => k.LoanNo == loanRepays.LoanNo).FirstOrDefault();
                        var AmountApproved = loanapplied.ApprovedAmount;

                        var lastRepayDate = lastrepaydates.LastRepayDate;
                        var dateReceived = loanRepays.DateReceived;
                        var totalDays = (dateReceived - lastRepayDate).TotalDays;

                        // Define 1 month as 45 days, and make sure minimum charge is for 1 month
                        int monthsElapsed = (int)Math.Ceiling(Math.Max(totalDays, 1) / 45.0);

                        decimal monthlyInterestRate = loanRepays.InterestRate / 12m / 100m;

                        Interest = (loanbalance ?? 0m) * monthlyInterestRate * monthsElapsed;



                        Principal1 = AmountApproved * (decimal)monthlyInterestRate / (1 - (decimal)Math.Pow((double)(1 + (decimal)monthlyInterestRate), -repayperiod)) ?? 0;
                        Interest = loanbalance * monthlyInterestRate ?? 0;
                        Principal1 = Principal1 - Interest;

                        var AmountRepay = loanRepays.Amount;

                        Principal2 = AmountRepay - Interest;



                        var loanbalances = loanbalance - Principal2;

                        var no_ofrepay = _context.REPAY.Where(k => k.LoanNo == loanRepays.LoanNo).Count();
                        var list = loanRepays.customerBalances.FirstOrDefault();


                        var repay = new RepayMentTable();
                        repay.LoanNo = loanRepays.LoanNo;
                        repay.MemberNo = loanRepays.MemberNo;
                        repay.Loancode = loanRepays.Loancode;
                        repay.Principal = Principal2;
                        repay.Interest = Interest;
                        repay.TotalRepayment = (Principal2 + Interest);
                        repay.CompanyCode = sacco;
                        repay.nextduedate = date.AddMonths(1);
                        repay.Penalty = 0;
                        repay.DateReceived = loanRepays.DateReceived;
                        repay.AuditID = auditid;
                        repay.ReceiptNo = loanRepays.ReceiptNo;
                        repay.PaymentNo = no_ofrepay + 1;
                        repay.Remarks = loanRepays.Remarks.Trim();
                        repay.LoanBalance = loanbalances ?? 0;
                        _context.REPAY.Add(repay);
                        var transaction = new ShareTransactionss
                        {
                            MemberNo = loanRepays.MemberNo,
                            Docnumber = loanRepays.ReceiptNo,
                            TransAmount = loanRepays.Amount,
                            TransDate = DateTime.Now,
                            TransDescription = loanRepays.Remarks.Trim(),
                        };
                        _context.ShareTransaction.Add(transaction);
                        var updateloanapplication = _context.loanAplications.FirstOrDefault(k => k.LoanNo == repay.LoanNo);
                        if (updateloanapplication != null)
                        {
                            _context.Entry(updateloanapplication).State = EntityState.Modified;
                            updateloanapplication.LoanBalance = loanbalances;
                            updateloanapplication.LastRepayDate = loanRepays.DateReceived;
                            _context.SaveChanges();
                        }

                        _context.SaveChanges();

                        var glaccoun = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strLoanAccount;
                        var interestacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strInterestAccount;
                        var penaltyacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strPenaltyAccount;


                        var accounts = _context.Customer_Balance.FirstOrDefault(m => m.Memberno == loanRepays.MemberNo);

                        var CrAccno1 = accounts.CRaccno;
                        var bankcode = accounts.CRaccno;

                        //gltransactions
                        var newrecords = new GL_Transaction();

                        newrecords.MemberNo = loanRepays.MemberNo;
                        newrecords.TransactionDate = loanRepays.DateReceived;
                        newrecords.Amount = Principal2;
                        newrecords.strDocnumber = loanRepays.ReceiptNo;
                        newrecords.CrAccNo = glaccoun;
                        newrecords.DrAccNo = bankcode;
                        newrecords.Remarks = "Loan Repayment" + loanRepays.MemberNo;
                        newrecords.CompanyCode = sacco;
                        newrecords.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords);
                        _context.SaveChanges();



                        //gl interest
                        var newrecords1 = new GL_Transaction();
                        newrecords1.MemberNo = loanRepays.MemberNo;
                        newrecords1.TransactionDate = loanRepays.DateReceived;
                        newrecords1.Amount = Principal2;
                        newrecords1.strDocnumber = loanRepays.ReceiptNo;
                        newrecords1.CrAccNo = interestacounNo;
                        newrecords1.DrAccNo = bankcode;
                        newrecords1.Remarks = "Interest Repayment" + loanRepays.MemberNo;
                        newrecords1.CompanyCode = sacco;
                        newrecords1.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords1.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords1);
                        _context.SaveChanges();


                        //gl penalty
                        var newrecords2 = new GL_Transaction();
                        newrecords2.MemberNo = loanRepays.MemberNo;
                        newrecords2.TransactionDate = loanRepays.DateReceived;
                        newrecords2.Amount = Principal2;
                        newrecords2.strDocnumber = loanRepays.ReceiptNo;
                        newrecords2.CrAccNo = penaltyacounNo;
                        newrecords2.DrAccNo = bankcode;
                        newrecords2.Remarks = "Penalty Repayment" + loanRepays.MemberNo;
                        newrecords2.CompanyCode = sacco;
                        newrecords2.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords2.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords2);
                        _context.SaveChanges();

                        await RefreshGuarantorsAsync(loanRepays.LoanNo);
                    }
                    else if (loanRepays.RepaymentMethod == "RBAL")
                    {
                        var loanbal = _context.loanAplications.Where(k => k.LoanNo == loanRepays.LoanNo).FirstOrDefault();
                        var loanbalance = loanbal.LoanBalance;
                        decimal remainingLoanAmount = loanbalance ?? 0;

                        

                        decimal monthlyInterestRate = (loanRepays.InterestRate / 12 / 100);
                        decimal nterestRate = (loanRepays.InterestRate / 12 / 100) * Convert.ToDecimal(loanbal.LoanBalance);
                        int repayperiod = loanRepays.RepaymentPeriod;

                        //loanapplied
                        var loanapplied = _context.loanAplications.Where(k => k.LoanNo == loanRepays.LoanNo).FirstOrDefault();
                        var AmountApproved = loanapplied.ApprovedAmount;

                        var lastrepaydates = _context.loanAplications.FirstOrDefault(k => k.LoanNo == loanRepays.LoanNo);
                        var Datereceived = loanRepays.DateReceived;

                        double totalDays = (loanRepays.DateReceived - lastrepaydates.LastRepayDate).TotalDays;

                        int monthsElapsed = (int)Math.Ceiling(Math.Max(totalDays, 1) / 45.0);

                        decimal monthlyInt = loanRepays.InterestRate / 12m / 100m;

                        decimal tempBalance = loanbalance ?? 0m;
                        decimal accruedInterest = 0m;
                        
                        for (int m = 1; m <= monthsElapsed; m++)
                        {
                            decimal interestForThisMonth = tempBalance * monthlyInt;
                            accruedInterest += interestForThisMonth;

                            if (m < monthsElapsed)
                                tempBalance += interestForThisMonth;
                        }

                        Interest = accruedInterest;          // send to REPAY table
                        Principal1 = (AmountApproved ?? 0m) / loanRepays.RepaymentPeriod;


                        var AmountRepay = loanRepays.Amount;

                        Principal2 = AmountRepay - Interest;

                        var loanbalances = (loanbalance - Principal2);

                        var no_ofrepay = _context.REPAY.Where(k => k.LoanNo == loanRepays.LoanNo).Count();
                        var list = loanRepays.customerBalances.FirstOrDefault();


                        var repay = new RepayMentTable();
                        repay.LoanNo = loanRepays.LoanNo;
                        repay.MemberNo = loanRepays.MemberNo;
                        repay.Loancode = loanRepays.Loancode;
                        repay.Principal = Principal2;
                        repay.Interest = Interest;
                        repay.TotalRepayment = (Principal2 + Interest);
                        repay.CompanyCode = sacco;
                        repay.nextduedate = date.AddMonths(1);
                        repay.Penalty = 0;
                        repay.DateReceived = loanRepays.DateReceived;
                        repay.AuditID = auditid;
                        repay.ReceiptNo = loanRepays.ReceiptNo;
                        repay.PaymentNo = no_ofrepay + 1;
                        repay.Remarks = loanRepays.Remarks.Trim();
                        repay.LoanBalance = loanbalances ?? 0;
                        _context.REPAY.Add(repay);
                        var transaction = new ShareTransactionss
                        {
                            MemberNo = loanRepays.MemberNo,
                            Docnumber = loanRepays.ReceiptNo,
                            TransAmount = loanRepays.Amount,
                            TransDate = DateTime.Now,
                            TransDescription = loanRepays.Remarks.Trim(),
                        };

                        _context.ShareTransaction.Add(transaction);
                        var updateloanapplication = _context.loanAplications.FirstOrDefault(k => k.LoanNo == repay.LoanNo);
                        if (updateloanapplication != null)
                        {
                            _context.Entry(updateloanapplication).State = EntityState.Modified;
                            updateloanapplication.LoanBalance = loanbalances;
                            updateloanapplication.LastRepayDate = loanRepays.DateReceived;
                            _context.SaveChanges();
                        }
                        _context.SaveChanges();
                        var glaccoun = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strLoanAccount;
                        var interestacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strInterestAccount;
                        var penaltyacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strPenaltyAccount;


                        var accounts = _context.Customer_Balance.FirstOrDefault(m => m.Loanno == loanRepays.LoanNo);

                        var CrAccno1 = accounts.CRaccno;
                        var bankcode = accounts.CRaccno;

                        //gltransactions
                        var newrecords = new GL_Transaction();

                        newrecords.MemberNo = loanRepays.MemberNo;
                        newrecords.TransactionDate = loanRepays.DateReceived;
                        newrecords.Amount = Principal2;
                        newrecords.strDocnumber = loanRepays.ReceiptNo;
                        newrecords.CrAccNo = glaccoun;
                        newrecords.DrAccNo = bankcode;
                        newrecords.Remarks = "Loan Repayment" + loanRepays.MemberNo;
                        newrecords.CompanyCode = sacco;
                        newrecords.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords);
                        _context.SaveChanges();

                        //gl interest
                        var newrecords1 = new GL_Transaction();
                        newrecords1.MemberNo = loanRepays.MemberNo;
                        newrecords1.TransactionDate = loanRepays.DateReceived;
                        newrecords1.Amount = Interest;
                        newrecords1.strDocnumber = loanRepays.ReceiptNo;
                        newrecords1.CrAccNo = interestacounNo;
                        newrecords1.DrAccNo = bankcode;
                        newrecords1.Remarks = "Interest Repayment" + loanRepays.MemberNo;
                        newrecords1.CompanyCode = sacco;
                        newrecords1.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords1.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords1);
                        _context.SaveChanges();

                        //gl penalty
                        var newrecords2 = new GL_Transaction();
                        newrecords2.MemberNo = loanRepays.MemberNo;
                        newrecords2.TransactionDate = loanRepays.DateReceived;
                        newrecords2.Amount = 0;
                        newrecords2.strDocnumber = loanRepays.ReceiptNo;
                        newrecords2.CrAccNo = penaltyacounNo;
                        newrecords2.DrAccNo = bankcode;
                        newrecords2.Remarks = "Penalty Repayment" + loanRepays.MemberNo;
                        newrecords2.CompanyCode = sacco;
                        newrecords2.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                        newrecords2.AuditID = auditid;
                        _context.GL_Transactions.Add(newrecords2);
                        _context.SaveChanges();
                        await RefreshGuarantorsAsync(loanRepays.LoanNo);
                    }
                }
                if (loanRepays.customerBalances != null)
                {
                    //const decimal minShareValue = 100;

                    foreach (var item in loanRepays.customerBalances)
                    {
                        var CrAccno1 = _context.shareTypes.FirstOrDefault(k => k.strSharecode.Contains(item.Transactioncode.ToLower()))?.GLaccno;
                        var bankcode = _context.banksSetups.FirstOrDefault(k => k.strBankName.Contains(item.DRaccno.ToUpper()))?.strBankAccno;

                        var newrecord = new Customer_Balance
                        {
                            Memberno = item.Memberno,
                            MemberName = item.MemberName,
                            TransDate = item.TransDate,
                            Transactioncode = item.Transactioncode,
                            Amount = item.Amount,
                            Bookbalance = 0,
                            Availablebalance = 0,
                            Accountno = item.Memberno,
                            Documentnumber = item.Documentnumber,
                            CRaccno = CrAccno1,
                            DRaccno = bankcode,
                            Transdescription = item.Transdescription.Trim(),
                            CompanyCode = sacco,
                            Transactionno = DateTime.Now + item.Memberno,
                            Loanno = "0",
                            Soure = "BOSA",
                            Auditid = auditid
                        };

                        _context.Customer_Balance.Add(newrecord);
                        var transaction = new ShareTransactionss
                        {
                            MemberNo = item.Memberno,
                            Docnumber = item.Documentnumber, // Set the generated receipt number here
                            TransAmount = item.Amount,
                            TransDate = DateTime.Now,
                            TransDescription = item.Transdescription.Trim(),
                        };

                        _context.ShareTransaction.Add(transaction);
                        if (item.Transactioncode == "R001")
                        {
                            var member = _context.MembersRegistrations.FirstOrDefault(m => m.strMemberNo == item.Memberno);
                            if (member != null)
                            {
                                var totalRegFeePaid = _context.Customer_Balance
                                    .Where(c => c.Memberno == item.Memberno && c.Transactioncode == "R001")
                                    .Sum(c => c.Amount);

                                if (totalRegFeePaid + item.Amount > 1500)
                                {
                                    Notify("Member { item.Memberno}cannot pay more than 1500 for registration.", notificationType: NotificationType.error);
                                }
                                if (totalRegFeePaid + item.Amount == 1500)
                                {
                                    member.HasPaidRegFee = true;
                                    _context.MembersRegistrations.Update(member);

                                }
                            }
                        }

                        _context.SaveChanges();
                    }

                    foreach (var item in loanRepays.customerBalances)
                    {

                        //var sharecode1 = _context.shareTypes.FirstOrDefault(k => k.strShareType.Contains(item.Transactioncode.ToLower())).strSharecode;

                        var CrAccno1 = _context.shareTypes.FirstOrDefault(k => k.strSharecode.Contains(item.Transactioncode.ToLower())).GLaccno;
                        var bankcode = _context.banksSetups.FirstOrDefault(k => k.strBankName.Contains(item.DRaccno.ToUpper())).strBankAccno;


                        var newrecords = new GL_Transaction();

                        newrecords.MemberNo = item.Memberno;
                        newrecords.TransactionDate = item.TransDate;
                        newrecords.Amount = item.Amount;
                        newrecords.strDocnumber = item.Documentnumber;
                        newrecords.CrAccNo = CrAccno1;
                        newrecords.DrAccNo = bankcode;
                        newrecords.Remarks = item.Transdescription.Trim() + item.Memberno;
                        newrecords.CompanyCode = sacco;
                        newrecords.TransactionNo = DateTime.Now + item.Memberno;
                        newrecords.AuditID = auditid;

                        _context.GL_Transactions.Add(newrecords);
                        _context.SaveChanges();
                        //const int minSharesRequired = 10;
                        //const decimal minTotalShareValue = minShareValue * minSharesRequired;


                        // var totalSaccoShares = _context.Customer_Balance
                        //.Where(c => c.Transdescription == "Savings And Contributions")
                        //.Sum(c => c.Amount);
                        // var shareCapitalCode = _context.shareTypes
                        //    .Where(s => s.strShareType == "Share Capital" && s.strCompany == sacco)
                        //    .Select(s => s.strSharecode)
                        //    .FirstOrDefault();

                        //var CrAccno2 = _context.shareTypes.FirstOrDefault(k => k.strSharecode.Contains(shareCapitalCode.ToLower()))?.GLaccno;

                        //var totalMemberShares = _context.Customer_Balance
                        //    .Where(c => c.Memberno == item.Memberno && c.Transdescription == "Savings And Contributions")
                        //    .Sum(c => c.Amount / minShareValue);
                        //var membersharess = totalMemberShares * 10;

                        //var ownershipLimit = totalSaccoShares * 0.20m;

                        //if (totalMemberShares < minSharesRequired || totalMemberShares > ownershipLimit)
                        //{
                        //}
                        //else
                        //{
                        //    var existingRecord = _context.Customer_Balance
                        //        .FirstOrDefault(c => c.Memberno == item.Memberno && c.Transdescription == "Share Capital");

                        //    if (existingRecord != null)
                        //    {
                        //        existingRecord.Amount = membersharess;
                        //        existingRecord.TransDate = item.TransDate;
                        //        existingRecord.Documentnumber = item.Documentnumber;

                        //        _context.Customer_Balance.Update(existingRecord);
                        //    }
                        //    else
                        //    {

                        //        var newRecord = new Customer_Balance
                        //        {
                        //            Memberno = item.Memberno,
                        //            MemberName = item.MemberName,
                        //            TransDate = item.TransDate,
                        //            Transactioncode = shareCapitalCode,
                        //            Amount = membersharess,
                        //            Bookbalance = 0,
                        //            Availablebalance = 0,
                        //            Accountno = item.Memberno,
                        //            Documentnumber = item.Documentnumber,
                        //            CRaccno = CrAccno2,
                        //            DRaccno = bankcode,
                        //            Transdescription = "Share Capital",
                        //            CompanyCode = sacco,
                        //            Transactionno = DateTime.Now + item.Memberno,
                        //            Loanno = "0",
                        //            Soure = "BOSA",
                        //            Auditid = auditid
                        //        };

                        //  _context.Customer_Balance.Add(newRecord);
                        var transaction = new ShareTransactionss
                        {
                            MemberNo = item.Memberno,
                            Docnumber = item.Documentnumber, // Set the generated receipt number here
                            TransAmount = item.Amount,
                            TransDate = DateTime.Now,
                            TransDescription = item.Transdescription.Trim(),
                        };

                        _context.ShareTransaction.Add(transaction);
                        //    }

                        //    _context.SaveChanges();

                        //    var newGLTransaction = new GL_Transaction
                        //    {
                        //        MemberNo = item.Memberno,
                        //        TransactionDate = item.TransDate,
                        //        Amount = membersharess,
                        //        strDocnumber = item.Documentnumber,
                        //        CrAccNo = CrAccno2,
                        //        DrAccNo = bankcode,
                        //        Remarks = "Share Capital " + item.Memberno,
                        //        CompanyCode = sacco,
                        //        TransactionNo = DateTime.Now + item.Memberno,
                        //        AuditID = auditid
                        //    };

                        //    _context.GL_Transactions.Add(newGLTransaction);
                        //    _context.SaveChanges();
                        //}
                        //result = "Success! Record Saved Successfully!";

                    }
                }
                result = "Success! Record Saved Successfully!";
                loanRepays.customerBalances.Clear();

                string generatedReceiptNo = await GenerateReceiptNo();
                return Json(new
                {
                    success = true,
                    message = result,
                    receiptNo = generatedReceiptNo
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while saving the record.",
                    error = ex.Message
                });
            }
        }
        // GET: ShareTransactions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.strMemberNo = _context.MembersRegistrations.ToList();
            ViewBag.strBankName = _context.banksSetups.ToList();
            ViewBag.strSharecode = _context.shareTypes.ToList();



            if (id == null)
            {
                return NotFound();
            }

            var shareTransaction = await _context.shareTransactions.FindAsync(id);
            if (shareTransaction == null)
            {
                return NotFound();
            }
            return View(shareTransaction);
        }

        // POST: ShareTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ShareTransaction shareTransaction)
        {
            ViewBag.strMemberNo = _context.MembersRegistrations.ToList();
            ViewBag.strBankName = _context.banksSetups.ToList();
            ViewBag.strSharecode = _context.shareTypes.ToList();


            if (id != shareTransaction.strId.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shareTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShareTransactionExists(shareTransaction.strId.ToString()))
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
            return View(shareTransaction);
        }

        // GET: ShareTransactions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shareTransaction = await _context.shareTransactions
                .FirstOrDefaultAsync(m => m.strId.ToString() == id);
            if (shareTransaction == null)
            {
                return NotFound();
            }

            return View(shareTransaction);
        }

        // POST: ShareTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var shareTransaction = await _context.shareTransactions.FindAsync(id);
            _context.shareTransactions.Remove(shareTransaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShareTransactionExists(string id)
        {
            return _context.shareTransactions.Any(e => e.strId.ToString() == id);
        }
    }
}
