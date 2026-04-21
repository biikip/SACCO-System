using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using static EASY_SACCO.Controllers.LoanTYpesController;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using static EASY_SACCO.Controllers.LoanAplicationsController;
using System.Net;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Syncfusion.EJ2.Linq;
using System.Security.Policy;
using static EASY_SACCO.Controllers.HomeController;
using EASY_SACCO.Utils;
using NToastNotify;
using DocumentFormat.OpenXml.Spreadsheet;
using Org.BouncyCastle.Utilities;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Mail;



namespace EASY_SACCO.Controllers
{
    public class LoanViewModel
    {
        public List<Loanguar> loanguars { get; set; }
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public string LoanNo { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal Deposit { get; set; }
        public decimal ShareCapital { get; set; }
        public decimal OutstandingLoan { get; set; }
        public decimal LoanBalance { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime AuditTime { get; set; }
        public int RepaymentPeriod { get; set; }
        public decimal InterestRate { get; set; }
        public string RepaymentMethod { get; set; }
        public string LoanCode { get; set; }
        public string AuditId { get; set; }
        public string CompanyCode { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime DateApproved { get; set; }
        public string Approver { get; set; }
        public int status { get; set; }
    }


    public class LoanAplicationsController : BaseController
    {

        private readonly IToastNotification _toastNotification;

        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public LoanAplicationsController(BOSA_DBContext context, IToastNotification toastNotification) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            _toastNotification = toastNotification;

        }

        public async Task<IActionResult> LoanSchedule(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var loanAplication = await _context.loanAplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanAplication != null)
            {
                ViewBag.loanamount = loanAplication.AppliedAmount;
                ViewBag.repayperiod = loanAplication.RepaymentPeriod;
                ViewBag.interestrate = loanAplication.InterestRate;
            }
            return View(loanAplication);
        }
        [HttpPost]
        public IActionResult Calculate1(LoanModel model)
        {


            List<LoanScheduleItem> schedule = new List<LoanScheduleItem>();
            double remainingLoanAmount = model.LoanAmount;
            double monthlyInterestRate = model.InterestRate / 12 / 100;
            int numberOfPayments = model.LoanTerm;

            for (int i = 0; i < numberOfPayments; i++)
            {
                double interest = remainingLoanAmount * monthlyInterestRate;
                double principal = 0;

                if (model.RepaymentMethod == "Amortized")
                {


                    principal = (model.LoanAmount * monthlyInterestRate) / (1 - Math.Pow(1 + monthlyInterestRate, -numberOfPayments));

                    principal = principal - interest;
                }
                else if (model.RepaymentMethod == "ReducingBalance")
                {
                    principal = model.LoanAmount / numberOfPayments;


                }
                else if (model.RepaymentMethod == "StraightLine")
                {
                    principal = model.LoanAmount / numberOfPayments;
                    interest = model.LoanAmount * monthlyInterestRate;
                }

                double totalPayment = interest + principal;
                remainingLoanAmount -= principal;

                DateTime dueDate = model.StartDate.AddMonths(i + 1);
                schedule.Add(new LoanScheduleItem
                {
                    DueDate = dueDate,
                    Principal = principal,
                    Interest = interest,
                    TotalPayment = totalPayment,
                    RemainingBalance = remainingLoanAmount
                });
            }

            return Json(schedule);
        }

        // GET: LoanAplications
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            General general = new General();

            // Get loans only for existing members
            general.loanAplications = await _context.loanAplications
           .Where(loan => _context.MembersRegistrations.Any(member => member.strMemberNo == loan.MemberNo))
           .OrderBy(k => k.ApplicationDate.Date)
           .Select(loan => new LoanAplication
           {
               Id = loan.Id,
               ApplicationDate = loan.ApplicationDate,
               MemberNo = loan.MemberNo,
               AppliedAmount = loan.AppliedAmount,
               CompanyCode = loan.CompanyCode,
               status = loan.status,
           })
           .ToListAsync();

            general.co_Operatives = await _context.co_Operatives.ToListAsync();
            general.loanguars = await _context.Loanguars.ToListAsync();
            general.loanTYpes = await _context.loanTYpes.ToListAsync();

            return View(general);
        }
        // GET: LoanAplications
        public async Task<IActionResult> AppraisalLoans()
        {
            utilities.SetUpPrivileges(this);
            General general = new General();

            general.loanAplications = await _context.loanAplications
           .OrderBy(k => k.DateApproved.Date) // Group by application date
           .Select(group => new LoanAplication
           {
               Id = group.Id,
               ApplicationDate = group.DateApproved,
               MemberNo = group.MemberNo,
               AppliedAmount = group.AppliedAmount,
               CompanyCode = group.CompanyCode,
               LoanCode = group.LoanCode,
               status = group.status,
           }).Where(k => k.status == 2).ToListAsync();
            general.co_Operatives = await _context.co_Operatives.ToListAsync();
            general.membersRegistrations = await _context.MembersRegistrations.ToListAsync();
            general.loanguars = await _context.Loanguars.ToListAsync();
            general.loanTYpes = await _context.loanTYpes.ToListAsync();
            return View(general);
        }
        // GET: LoanAplications
        public async Task<IActionResult> EndorsementList()
        {
            utilities.SetUpPrivileges(this);
            ViewBag.Banks = new SelectList(_context.banksSetups, "strBankName", "strBankName").ToList();
            General general = new General();
            general.loanAplications = await _context.loanAplications
                       .OrderBy(k => k.DateApproved.Date) // Group by application date
                       .Select(group => new LoanAplication
                       {
                           Id = group.Id,
                           ApplicationDate = group.DateApproved,
                           MemberNo = group.MemberNo,
                           AppliedAmount = group.AppliedAmount,
                           ApprovedAmount = group.ApprovedAmount,
                           CompanyCode = group.CompanyCode,
                           LoanCode = group.LoanCode,
                           status = group.status,


                       }).Where(k => k.status == 3).ToListAsync();
            general.co_Operatives = await _context.co_Operatives.ToListAsync();
            general.membersRegistrations = await _context.MembersRegistrations.ToListAsync();
            general.loanguars = await _context.Loanguars.ToListAsync();
            general.Charges = await _context.Charges.ToListAsync();
            general.Customer_Balances = await _context.Customer_Balance.ToListAsync();
            general.loanTYpes = await _context.loanTYpes.ToListAsync();
            return View(general);
        }
        // GET: LoanAplications
        public async Task<IActionResult> DisbursedLoan()
        {
            utilities.SetUpPrivileges(this);
            General general = new General();

            general.loanAplications = await _context.loanAplications
                 .OrderBy(k => k.DateApproved.Date) // Group by application date
                 .Select(group => new LoanAplication
                 {
                     Id = group.Id,
                     DisburseDate = group.DisburseDate,
                     MemberNo = group.MemberNo,
                     DisburseAmount = group.DisburseAmount,
                     AppliedAmount = group.AppliedAmount,
                     LoanCode = group.LoanCode,
                     ApprovedAmount = group.ApprovedAmount,
                     TotalCharges = group.TotalCharges,
                     NetPay = group.NetPay,
                     CompanyCode = group.CompanyCode,
                     status = group.status,


                 }).Where(k => k.status == 4).ToListAsync();

            general.co_Operatives = await _context.co_Operatives.ToListAsync();
            general.membersRegistrations = await _context.MembersRegistrations.ToListAsync();
            general.loanguars = await _context.Loanguars.ToListAsync();
            general.Charges = await _context.Charges.ToListAsync();
            general.Customer_Balances = await _context.Customer_Balance.ToListAsync();
            general.loanTYpes = await _context.loanTYpes.ToListAsync();
            return View(general);
        }
        public async Task<IActionResult> LoanAppraisalReport(int? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }
            var loanAplication = await _context.loanAplications
                .FirstOrDefaultAsync(m => m.Id == id);
            //no of guarantor
            var guarantor = _context.Loanguars.Where(k => k.LoanNo == loanAplication.LoanNo).Select(k => k.LoanNo).Count();
            //member name
            var name = await _context.MembersRegistrations.FirstOrDefaultAsync(k => k.strMemberNo == loanAplication.MemberNo);
            //list of guarantor
            var listguarantor = _context.Loanguars.Where(k => k.LoanNo == loanAplication.LoanNo).ToList();
            //amount of guarantor
            var amountguarantor = _context.Loanguars.Where(k => k.LoanNo == loanAplication.LoanNo).Sum(k => k.AmountGuaranteed);
            //charges
            var amountchargedprocessing = _context.Charges.Where(k => k.LoanCode == loanAplication.LoanCode && k.ProductCode.Contains("Processing")).Sum(k => k.Amount);
            //charges
            var amountchargedInsuarance = _context.Charges.Where(k => k.LoanCode == loanAplication.LoanCode && k.ProductCode.Contains("Insuarance")).Sum(k => k.Amount);

            ViewBag.loanno = loanAplication.LoanNo;
            ViewBag.applicationdate = loanAplication.ApplicationDate;
            ViewBag.loantype = loanAplication.LoanCode;
            ViewBag.memberno = loanAplication.MemberNo;
            ViewBag.membername = name.strOtherName + " " + name.strSurName;
            ViewBag.appliedamount = loanAplication.AppliedAmount;
            ViewBag.recommeded = loanAplication.RecommendedAmount;
            ViewBag.approved = loanAplication.ApprovedAmount;
            ViewBag.installment = loanAplication.RepaymentPeriod;
            ViewBag.noofguarantor = guarantor;
            ViewBag.repayamount = loanAplication.TotalAmount;
            ViewBag.listofguarantor = listguarantor;
            ViewBag.amountguarantor = amountguarantor;
            ViewBag.chargedprocessing = amountchargedprocessing;
            ViewBag.chargedinsuarnce = amountchargedInsuarance;
            //Deposits Analysis
            //Deposit
            var totaldeposit = _context.Customer_Balance.Where(k =>
            (k.Transactioncode.ToLower().Contains("d") ||
            k.Transdescription.ToLower().Contains("deposit") ||
            k.Transdescription.ToLower().Contains("saving")) &&
            k.Memberno == loanAplication.MemberNo).Sum(k => k.Amount);
            var roundoffDeposit = Math.Round(totaldeposit, 2);

            //matured share
            var sharecodes = _context.Customer_Balance.FirstOrDefault(k =>
            (k.Transactioncode.ToLower().Contains("d") ||
            k.Transdescription.ToLower().Contains("deposit") ||
            k.Transdescription.ToLower().Contains("saving")));
            var loantoshareratio = _context.shareTypes.FirstOrDefault(k => k.strSharecode == sharecodes.Transactioncode).strloanstoshareratio;
            var totalDepositMultiplier = (roundoffDeposit * Convert.ToDecimal(loantoshareratio));
            var loanbalance = 0;
            ViewBag.Deposits = roundoffDeposit;
            ViewBag.depositsmultiplier = totalDepositMultiplier;
            ViewBag.loanbalance = loanbalance;
            ViewBag.qualifyingamount = (totalDepositMultiplier - loanbalance);


            return View(loanAplication);
        }
        public IActionResult Appraisal(int? id)
        {
            utilities.SetUpPrivileges(this);
            var auditid = HttpContext.Session.GetString("UserID");
            var loanAplication = _context.loanAplications
              .FirstOrDefault(m => m.Id == id);
            // Deposit
            var totaldeposit = _context.Customer_Balance
                .Where(k =>
                    (k.Transactioncode.ToLower().Contains("d") ||
                     k.Transdescription.ToLower().Contains("deposit") ||
                     k.Transdescription.ToLower().Contains("saving"))
                     && k.Memberno == loanAplication.MemberNo)
                .Sum(k => k.Amount);
            var roundoffDeposit = Math.Round(totaldeposit, 2);

            // Share Capital
            var totalsharecapital = _context.Customer_Balance
                .Where(k =>
                    (k.Transactioncode.ToLower().Contains("sc") ||
                     k.Transdescription.ToLower().Contains("share capital") ||
                     k.Transdescription.ToLower().Contains("capital"))
                     && k.Memberno == loanAplication.MemberNo)
                .Sum(k => k.Amount);
            var roundoffsharecapital = Math.Round(totalsharecapital, 2);


            var requiresGuarantors = _context.loanTYpes
                .Where(k => k.strloancode == loanAplication.LoanCode)
                .Select(k => k.strRequiresGuarantors)
                .FirstOrDefault();
            decimal guaranteedAmount = 0;

            if (requiresGuarantors == true)
            {
                guaranteedAmount = _context.Loanguars
                   .Where(k => k.LoanNo == loanAplication.LoanNo)
                   .Sum(k => k.AmountGuaranteed);
            }
            else
            {
                guaranteedAmount = Convert.ToDecimal(loanAplication.AppliedAmount);
            }

            if (loanAplication != null)
            {
                ViewBag.id = id;
                ViewBag.memberno = loanAplication.MemberNo;
                ViewBag.deposit = roundoffDeposit;
                ViewBag.sharecapital = roundoffsharecapital;
                ViewBag.outstandingloan = 0;
                ViewBag.loanbal = 0;
                ViewBag.loancode = loanAplication.LoanCode;
                ViewBag.loanNo = loanAplication.LoanNo;
                ViewBag.loanamount = loanAplication.AppliedAmount;
                ViewBag.applicationdate = loanAplication.ApplicationDate.ToShortDateString();
                ViewBag.repayperiod = loanAplication.RepaymentPeriod;
                ViewBag.interestrate = loanAplication.InterestRate;
                ViewBag.repaymethod = loanAplication.RepaymentMethod;
                ViewBag.amountguarantee = guaranteedAmount;

            }
            return View(loanAplication);
        }


        // GET: LoanAplications/Appraisal/5
        [HttpPost]
        public async Task<IActionResult> Appraisal(int? id, decimal RecommendedAmount, DateTime DateApproved)
        {
            utilities.SetUpPrivileges(this);
            var auditid = HttpContext.Session.GetString("UserID");

            if (id == null)
            {
                return NotFound();
            }

            var loanAplication = await _context.loanAplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanAplication != null)
            {
                _context.Entry(loanAplication).State = EntityState.Modified;
                loanAplication.status = 2;
                loanAplication.Approver = auditid;
                loanAplication.DateApproved = DateApproved;
                loanAplication.RecommendedAmount = RecommendedAmount;
                loanAplication.ApprovedAmount = RecommendedAmount;
                loanAplication.PrincipalRepayment = 0;
                loanAplication.InterestRepayment = 0;
                loanAplication.TotalAmount = 0;
                Notify("Loan appraised Successfully!");
                _context.SaveChanges();

            }
            return RedirectToAction("Index");


        }

        public IActionResult Endorsement(int? id)
        {
            utilities.SetUpPrivileges(this);

            var loanAplication = _context.loanAplications
                .FirstOrDefault(m => m.Id == id);

            //Deposit
            var totaldeposit = _context.Customer_Balance.Where(k =>
            (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving")) && k.Memberno == loanAplication.MemberNo).Sum(k => k.Amount);
            var roundoffDeposit = Math.Round(totaldeposit, 2);

            //matured share
            var sharecodes = _context.Customer_Balance.FirstOrDefault(k => (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving")));
            var loantoshareratio = _context.shareTypes.FirstOrDefault(k => k.strSharecode == sharecodes.Transactioncode).strloanstoshareratio;
            var totalDeposit = (roundoffDeposit * Convert.ToDecimal(loantoshareratio));


            var totalsharecapital = _context.Customer_Balance.Where(k =>
            (k.Transactioncode.ToLower().Contains("sc") ||
            k.Transdescription.ToLower().Contains("share capital") ||
            k.Transdescription.ToLower().Contains("capital")) &&
            k.Memberno == loanAplication.MemberNo).Sum(k => k.Amount);
            var roundoffsharecapital = Math.Round(totalsharecapital, 2);
            var requiresGuarantors = _context.loanTYpes
                 .Where(k => k.strloancode == loanAplication.LoanCode)
                 .Select(k => k.strRequiresGuarantors)
                 .FirstOrDefault();
            decimal guaranteedAmount = 0;

            if (requiresGuarantors == true)
            {
                guaranteedAmount = _context.Loanguars
                   .Where(k => k.LoanNo == loanAplication.LoanNo)
                   .Sum(k => k.AmountGuaranteed);
            }
            else
            {
                guaranteedAmount = Convert.ToDecimal(loanAplication.AppliedAmount);
            }

            if (loanAplication != null)
            {
                ViewBag.id = id;
                ViewBag.memberno = loanAplication.MemberNo;
                ViewBag.deposit = totalDeposit;
                ViewBag.sharecapital = roundoffsharecapital;
                ViewBag.outstandingloan = 0.00;
                ViewBag.loanbal = 0.00;
                ViewBag.loancode = loanAplication.LoanCode;
                ViewBag.loanno = loanAplication.LoanNo;
                ViewBag.appliedamount = loanAplication.AppliedAmount;
                ViewBag.applicationdate = loanAplication.ApplicationDate;
                ViewBag.repayperiod = loanAplication.RepaymentPeriod;
                ViewBag.interest = loanAplication.InterestRate;
                ViewBag.repaymethod = loanAplication.RepaymentMethod;
                ViewBag.guaranteeloan = guaranteedAmount;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Endorsement(int? id, string MinuteNumber, decimal RecommendedAmount, decimal ApprovedAmount, decimal PrincipalRepayment, decimal InterestRepayment, decimal TotalAmount, DateTime DateApproved)
        {
            utilities.SetUpPrivileges(this);
            var auditid = HttpContext.Session.GetString("UserID");

            if (id == null)
            {
                return NotFound();
            }

            var loanAplication = await _context.loanAplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanAplication != null)
            {
                _context.Entry(loanAplication).State = EntityState.Modified;
                loanAplication.status = 3;
                loanAplication.Approver = auditid;
                loanAplication.DateApproved = DateApproved;
                loanAplication.ApprovedAmount = ApprovedAmount;
                loanAplication.RecommendedAmount = RecommendedAmount;
                loanAplication.MinuteNumber = MinuteNumber;
                loanAplication.PrincipalRepayment = PrincipalRepayment;
                loanAplication.InterestRepayment = InterestRepayment;
                loanAplication.TotalAmount = TotalAmount;
                loanAplication.LoanBalance = ApprovedAmount;

                Notify("Loan Approved Successfully!");
                _context.SaveChanges();

            }
            return RedirectToAction("Index");


        }
        public IActionResult Disburse(int? id)
        {
            utilities.SetUpPrivileges(this);
            ViewBag.Banks = new SelectList(_context.banksSetups, "strBankName", "strBankName").ToList();

            var loanAplication = _context.loanAplications
                .FirstOrDefault(m => m.Id == id);
            var name = _context.MembersRegistrations.FirstOrDefault(k => k.strMemberNo == loanAplication.MemberNo);
            var Docnumber1 = _context.Customer_Balance.FirstOrDefault(k => k.Memberno == loanAplication.MemberNo).Documentnumber;
            var totalcharges = _context.Charges.Where(k => k.LoanCode == loanAplication.LoanCode).Sum(k => k.Amount);
            var Lischarges = _context.Charges.Where(k => k.LoanCode == loanAplication.LoanCode).ToList();
            if (loanAplication != null)
            {
                ViewBag.id = id;
                ViewBag.memberno = loanAplication.MemberNo;
                ViewBag.name = name.strOtherName + " " + name.strSurName;
                ViewBag.docno = Docnumber1;
                ViewBag.approvedamount = loanAplication.ApprovedAmount;
                ViewBag.totalcharges = totalcharges;
                ViewBag.netpay = (loanAplication.ApprovedAmount - totalcharges);
                ViewBag.listcharges = Lischarges;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Disburse(List<Charges> charges, int? id, string Reason, string BankAccount, string MemberNo, string Transactioncode, string Name, string Docno, decimal TotalCharges, decimal NetPay, decimal DisburseAmount, string PaymentMode, decimal ApprovedAmount, DateTime DisburseDate, string ReferenceNumber)
        {
            utilities.SetUpPrivileges(this);

            var auditid = HttpContext.Session.GetString("UserID");
            var sacco = HttpContext.Session.GetString("CompanyCode");
            if (id == null)
            {
                return NotFound();
            }

            var loanAplication = await _context.loanAplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanAplication != null)
            {
                _context.Entry(loanAplication).State = EntityState.Modified;
                loanAplication.status = 4;
                loanAplication.Approver = auditid;
                loanAplication.NetPay = NetPay;
                loanAplication.TotalCharges = TotalCharges;
                loanAplication.DisburseAmount = DisburseAmount;
                loanAplication.PaymentMode = PaymentMode;
                loanAplication.DisburseDate = DisburseDate;
                loanAplication.BankAccount = BankAccount;
                loanAplication.ReferenceNumber = ReferenceNumber;

                Notify("Loan Disbursed Successfully!");
                _context.SaveChanges();

            }
            var glaccoun = _context.loanTYpes.Where(k => k.strloancode == loanAplication.LoanCode).FirstOrDefault().strLoanAccount;
            var accounts = _context.Customer_Balance.FirstOrDefault(m => m.Memberno == loanAplication.MemberNo);

            var CrAccno1 = glaccoun;
            var bankcode = accounts.DRaccno;


            var newrecord = new Customer_Balance();

            newrecord.Memberno = MemberNo;
            newrecord.MemberName = Name;
            newrecord.TransDate = DisburseDate;
            newrecord.Transactioncode = "Loan";
            newrecord.Amount = DisburseAmount;
            newrecord.Bookbalance = 0;
            newrecord.Availablebalance = 0;
            newrecord.Accountno = MemberNo;
            newrecord.Documentnumber = Docno;

            newrecord.CRaccno = bankcode;
            newrecord.DRaccno = CrAccno1;
            newrecord.Transdescription = "Disbursement";
            newrecord.CompanyCode = sacco;
            newrecord.Transactionno = DateTime.Now + MemberNo;
            newrecord.Loanno = loanAplication.LoanNo;
            newrecord.Soure = "BOSA";
            newrecord.CompanyCode = sacco;
            newrecord.Auditid = auditid;


            _context.Customer_Balance.Add(newrecord);
            _context.SaveChanges();

            //gltransactions
            var newrecords = new GL_Transaction();

            newrecords.MemberNo = MemberNo;
            newrecords.TransactionDate = DisburseDate;
            newrecords.Amount = DisburseAmount;
            newrecords.strDocnumber = Docno;
            newrecords.CrAccNo = bankcode;
            newrecords.DrAccNo = glaccoun;
            newrecords.Remarks = "Loan Disbursement-" + MemberNo;
            newrecords.CompanyCode = sacco;
            newrecords.TransactionNo = DateTime.Now + MemberNo;
            newrecords.AuditID = auditid;
            _context.GL_Transactions.Add(newrecords);
            _context.SaveChanges();
            //gltransactionscharges

            foreach (var charge in charges)
            {
                var newrecords1 = new GL_Transaction();

                newrecords1.MemberNo = MemberNo;
                newrecords1.TransactionDate = DisburseDate;
                newrecords1.Amount = charge.Amount;
                newrecords1.strDocnumber = Docno;
                newrecords1.CrAccNo = charge.GLAccount;
                newrecords1.DrAccNo = CrAccno1;
                newrecords1.Remarks = charge.ProductCode + MemberNo;
                newrecords1.CompanyCode = sacco;
                newrecords1.TransactionNo = DateTime.Now + MemberNo;
                newrecords1.AuditID = auditid;
                _context.GL_Transactions.Add(newrecords1);
                Notify("Loan Disbursed Successfully!");
                _context.SaveChanges();
            }
            var guarantor = _context.MembersRegistrations
    .Where(k => k.strMemberNo == MemberNo.Trim())
    .FirstOrDefault();

            if (guarantor != null && !string.IsNullOrEmpty(guarantor.strEmail))
            {
                string guarantorEmail = guarantor.strEmail;
                string Names = guarantor.strSurName + ' ' + guarantor.strOtherName;
                // Find Requesting Member's Name
                var requestingMember = _context.MembersRegistrations
                                        .Where(k => k.strMemberNo == MemberNo.Trim())
                                        .FirstOrDefault();

                if (requestingMember != null)
                {
                    string requestName = requestingMember.strSurName + " " + requestingMember.strOtherName;

                    string subject = "Loan Request Approval";
                    string body = $@"
                             <p>Dear {requestName},</p>
                             <p>You Loan of Amount Ksh{DisburseAmount} has been Approved.</p>
                             <p>Regards,<br>Sacco Team</p>";
                    SendEmail(guarantorEmail, subject, body);
                }
            }
            return RedirectToAction("Index");


        }
        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("notifications@cargen.com", "lwqhgbyqbucukzei"), // Use secure credentials
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("notifications@cargen.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email Sending Failed: " + ex.Message);
            }
        }

        // GET: LoanAplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var loanAplication = await _context.loanAplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanAplication == null)
            {
                return NotFound();
            }

            return View(loanAplication);
        }

        public class Member
        {
            public string MemberNo { get; set; }
            public string MemberName { get; set; }
        }
        [HttpGet]
        public IActionResult CheckGuarantorRequirement(int loanTypeId)
        {
            var loanType = _context.loanTYpes.FirstOrDefault(x => x.Id == loanTypeId);

            if (loanType != null)
            {
                return Json(loanType.strRequiresGuarantors);
            }

            return Json(false);
        }
        [HttpPost]
        public IActionResult populateInterestrate(string LoanCode, string MemberNo)
        {
            if (string.IsNullOrEmpty(LoanCode) || string.IsNullOrEmpty(MemberNo))
            {
                return Json(new { error = "LoanCode or MemberNo is missing." });
            }

            var loancount = "";

            int loancodenoCount = _context.loanAplications
                .Where(k => k.MemberNo == MemberNo && k.LoanCode == LoanCode)
                .Count();

            if (loancodenoCount > 0)
            {
                loancount = LoanCode + MemberNo + "-" + (loancodenoCount + 1);
            }
            else
            {
                loancount = LoanCode + MemberNo;
            }

            var intrestrates = _context.loanTYpes.FirstOrDefault(k => k.strloancode == LoanCode);

            if (intrestrates == null)
            {
                return Json(new { error = "Loan type not found." });
            }
            bool requiresGuarantor = intrestrates.strRequiresGuarantors;
            var result = new
            {
                loancode = intrestrates.strloancode.Trim(),
                repaypriod = intrestrates.strRPeriod,
                interestrate = Math.Round(decimal.Parse(intrestrates.strInterestRate), 2),
                repaymethod = intrestrates.strRepaymentMethod?.Trim(),
                loancount = loancount,
                loantoshareratios = intrestrates.LoanToShareRatio,
                requiresGuarantor = requiresGuarantor
            };

            return Json(result);
        }
        [HttpPost]
        public IActionResult populaterecordsformemberno(string MemberNo)
        {
            //get months
            var Applicationdate = _context.MembersRegistrations.FirstOrDefault(k => k.strMemberNo == MemberNo);

            var date2 = DateTime.Now.Subtract(Applicationdate.strRegDate).TotalDays;
            var totaldays = date2;
            if (totaldays < 90)
            {
                return View();

            }

            var Names = _context.MembersRegistrations.FirstOrDefault(k => k.strMemberNo == MemberNo);
            var Nameofmember = Names.strOtherName + " " + Names.strSurName;
            //Deposit
            var totaldeposit = _context.Customer_Balance.Where(k => (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving")) && k.Memberno == MemberNo).Sum(k => k.Amount);
            var roundoffDeposit = Math.Round(totaldeposit, 2);

            //matured share
            var sharecodes = _context.Customer_Balance.FirstOrDefault(k => (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving")));
            var loantoshareratio = _context.shareTypes.FirstOrDefault(k => k.strSharecode == sharecodes.Transactioncode).strloanstoshareratio;
            var totalmaturedshare = (roundoffDeposit * Convert.ToDecimal(loantoshareratio));


            var totalsharecapital = _context.Customer_Balance.Where(k => (k.Transactioncode.ToLower().Contains("sc") || k.Transdescription.ToLower().Contains("share capital") || k.Transdescription.ToLower().Contains("capital")) && k.Memberno == MemberNo).Sum(k => k.Amount);
            var roundoffsharecapital = Math.Round(totalsharecapital, 2);




            var myResult = new
            {
                deposit = roundoffDeposit,
                capital = roundoffsharecapital,
                maturedshare = totalmaturedshare,
                names = Nameofmember


            };
            if (myResult != null)
            {

                var alldata = myResult.ToString().Replace("{", "").Replace("}", "");
                var finalldata = alldata.ToString().Replace("deposit", "")
                    .Replace("capital", "")
                    .Replace("maturedshare", "")
                    .Replace("names", "")
                    .Replace("=", "");


                return Json(finalldata);
            }



            return View();



        }
        [HttpPost]
        public IActionResult PopulateMemberdetails(int? Id)
        {
            utilities.SetUpPrivileges(this);
            if (Id != null)
            {

                var names = "";
                var membernos = "";
                decimal totaldeposit = 0;
                decimal totalsharecapital = 0;
                decimal maturedshares = 0;
                decimal maxloan = 0;
                decimal loanbal = 0;
                int outstandingloan = 0;


                var customerdetails = _context.MembersRegistrations.Find(Id);


                if (customerdetails != null)
                {

                    int maturity = _context.SYSPARAM.Select(k => k.maturity).FirstOrDefault();
                    string maturityStr = maturity.ToString();


                    DateTime regdate = _context.MembersRegistrations
                        .Where(k => k.strMemberNo == customerdetails.strMemberNo)
                        .Select(k => k.strRegDate)
                        .FirstOrDefault();


                    int monthsSinceJoining = ((DateTime.Now.Year - regdate.Year) * 12) + DateTime.Now.Month - regdate.Month;


                    //if (monthsSinceJoining < maturity)
                    //{
                    //    return Json(new { error = "The member has not matured yet to take a loan." });
                    //}


                    names = customerdetails.strSurName + " " + customerdetails.strOtherName;
                    membernos = customerdetails.strMemberNo;

                    //Deposit
                    totaldeposit = _context.Customer_Balance.Where(k => (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving")) && k.Memberno == customerdetails.strMemberNo).Sum(k => k.Amount);
                    var roundoffDeposit = Math.Round(totaldeposit, 2);

                    //share capital
                    totalsharecapital = _context.Customer_Balance.Where(k => (k.Transactioncode.ToLower().Contains("sc") || k.Transdescription.ToLower().Contains("share capital") || k.Transdescription.ToLower().Contains("capital")) && k.Memberno == customerdetails.strMemberNo).Sum(k => k.Amount);
                    var roundoffsharecapital = Math.Round(totalsharecapital, 2);

                    //max loan
                    var sharecodes = _context.Customer_Balance.FirstOrDefault(k => (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving")));
                    var loantoshareratio = _context.shareTypes.FirstOrDefault(k => k.strSharecode == sharecodes.Transactioncode).strloanstoshareratio;
                    var totalmaturedshare = (roundoffDeposit * Convert.ToDecimal(loantoshareratio));

                    loanbal = _context.loanAplications.Where(k => k.MemberNo == customerdetails.strMemberNo).Sum(k => k.LoanBalance) ?? 0;
                    outstandingloan = _context.loanAplications.Where(k => k.MemberNo == customerdetails.strMemberNo && k.LoanBalance > 0).Count();

                    totaldeposit = roundoffDeposit;
                    totalsharecapital = roundoffsharecapital;
                    maturedshares = roundoffDeposit;
                    maxloan = totalmaturedshare;

                }
                return Json(new
                {
                    names,
                    membernos,
                    totaldeposit,
                    totalsharecapital,
                    maturedshares,
                    maxloan,
                    loanbal,
                    outstandingloan


                });
            }
            return View();
        }
        [HttpPost]
        public IActionResult PopulateGuarantor(int? guarantorId)
        {
            utilities.SetUpPrivileges(this);

            if (guarantorId != null)
            {
                var names = "";
                var membernos = "";
                decimal totaldeposit = 0;
                decimal AmtAvailable = 0;

                var memberno = _context.Customer_Balance.Where(k => k.Id == guarantorId).FirstOrDefault();

                if (memberno != null)
                {
                    var guarantormemberno = memberno.Memberno;

                    var guaranteedAmount = _context.Loanguars
                        .Where(k => k.MemberNo == guarantormemberno)
                        .Sum(k => k.AmountGuaranteed);

                    var totalcurrentDeposit = _context.Customer_Balance
                        .Where(k => (k.Transactioncode.Contains("D") || k.Transdescription.Contains("DEPOSIT") || k.Transdescription.Contains("SAVING")) && k.Memberno == guarantormemberno)
                        .Sum(k => k.Amount);

                    totalcurrentDeposit = Math.Round(totalcurrentDeposit, 2);
                    names = memberno.MemberName;
                    membernos = memberno.Memberno;
                    totaldeposit = totalcurrentDeposit;
                    AmtAvailable = totaldeposit - guaranteedAmount;
                    if(AmtAvailable<0)
                    {
                        AmtAvailable = 0;
                    }
                }

                return Json(new
                {
                    names,
                    membernos,
                    totaldeposit,
                    AmtAvailable
                });
            }

            // when no guarantorId is provided
            return Json(new
            {
                names = "",
                membernos = "",
                totaldeposit = 0,
                AmtAvailable = 0
            });
        }

        [HttpPost]
        public IActionResult populateguarantordetails(string MemberNo)
        {
            if (MemberNo == null)
            {
                Notify("The Guarantor does not exist!", notificationType: NotificationType.error);
                return View("Create");
            }
            else
            {
                var names = "";
                var membernos = "";
                decimal totaldeposit = 0;

                var memberno = _context.Customer_Balance.Where(k => k.Memberno == MemberNo).FirstOrDefault();

                var guarantormemberno = memberno.Memberno;
                if (guarantormemberno != null)
                {

                    var totalcurrentDeposit = _context.Customer_Balance.Where(k => (k.Transactioncode.Contains("D") || k.Transdescription.Contains("DEPOSIT") || k.Transdescription.Contains("SAVING")) && k.Memberno == guarantormemberno).Sum(k => k.Amount);
                    totalcurrentDeposit = Math.Round(totalcurrentDeposit, 2);
                    names = memberno.MemberName;
                    membernos = memberno.Memberno;
                    totaldeposit = totalcurrentDeposit;

                }
                return Json(new
                {
                    names,
                    membernos,
                    totaldeposit

                });
            }

        }
        public class customer
        {
            public int customerid { get; set; }
            public string customername { get; set; }

        }

        [HttpPost]
        public IActionResult SaveOrder([FromBody] LoanViewModel loans)
        {
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");
            DateTime date = DateTime.UtcNow.AddHours(3);

            if (loans == null)
            {
                return Json(new { success = false, message = "Invalid loan data." });
            }

            var memberNoGuarantorCount = loans.loanguars?.Count() ?? 0;
            var maxLoanGuarantors = _context.SYSPARAM.FirstOrDefault()?.MaxGuarantors ?? 0;
            var guaranteedAmount = _context.Loanguars
                    .Where(k => k.LoanNo == loans.LoanNo)
                    .Sum(k => k.AmountGuaranteed);
            var requiresGuarantors = _context.loanTYpes
            .Where(k => k.strloancode == loans.LoanCode)
            .Select(k => k.strRequiresGuarantors)
            .FirstOrDefault();
            if (memberNoGuarantorCount > maxLoanGuarantors)
            {
                return Json(new { success = false, message = "The Loan has reached the maximum guarantorship!" });
            }
            if (requiresGuarantors == true)
            {
                if (memberNoGuarantorCount == 0)
                {
                    return Json(new { success = false, message = "The Loan Requires guarantors please add before you continue!" });
                }
            }
            var existingLoan = _context.loanAplications.FirstOrDefault(j => j.MemberNo == loans.MemberNo);
            var loanCount = _context.loanAplications.Count(j => j.MemberNo == loans.MemberNo);

            string loanNo;
            if (existingLoan != null)
            {
                loanNo = existingLoan.LoanCode + loans.MemberNo + "-" + (loanCount + 1);
            }
            else
            {
                loanNo = loans.LoanNo;
            }

            var loanApplication = new LoanAplication
            {
                Id = 0,
                MemberNo = loans.MemberNo,
                LoanNo = loans.LoanNo,
                LoanCode = loans.LoanCode,
                AppliedAmount = loans.AppliedAmount,
                Deposit = loans.Deposit,
                ShareCapital = loans.ShareCapital,
                OutstandingLoan = loans.OutstandingLoan,
                LoanBalance = loans.AppliedAmount,
                ApplicationDate = loans.ApplicationDate,
                AuditId = auditid,
                RepaymentMethod = loans.RepaymentMethod,
                RepaymentPeriod = loans.RepaymentPeriod,
                InterestRate = loans.InterestRate,
                LastRepayDate = loans.ApplicationDate,
                CompanyCode = sacco,
                Approver = "0",
                ApprovedAmount = 0,
                status = 1
            };

            _context.loanAplications.Add(loanApplication);

            // Add Guarantors if any
            if (loans.loanguars != null && loans.loanguars.Count > 0)
            {
                foreach (var item in loans.loanguars)
                {
                    var newGuarantor = new Loanguar
                    {
                        LoanNo = loans.LoanNo,
                        MemberNo = item.MemberNo,
                        AmountDeposit = item.AmountDeposit,
                        AmountGuaranteed = item.AmountGuaranteed,
                        FullNames = item.FullNames,
                        CompanyCode = sacco,
                        AuditId = auditid
                    };
                    _context.Loanguars.Add(newGuarantor);
                }
            }

            try
            {
                _context.SaveChanges();
                return Json(new { success = true, message = "Records Saved Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving loan: " + ex.Message });
            }
        }


        [HttpPost]
        public JsonResult getmembers([FromBody] MembersRegistration membersRegistration, string memberss, string membersconditionss, string memberrnos)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode");

            // Only fetch active and fully approved members
            var members = _context.MembersRegistrations
                .Where(i => i.strCompanyCode.ToUpper() == sacco.ToUpper() && i.IsApproved == true && i.HasPaidRegFee == true // Only active members
                )
                .ToList();

            var names = "";
            var membernos = "";
            decimal totaldeposit = 0;
            decimal totalsharecapital = 0;
            decimal maturedshares = 0;
            decimal maxloan = 0;
            decimal loanbal = 0;
            int outstandingloan = 0;

            if (memberrnos != null)
            {
                int maturity = _context.SYSPARAM.Select(k => k.maturity).FirstOrDefault();
                string maturityStr = maturity.ToString();


                DateTime regdate = _context.MembersRegistrations
                    .Where(k => k.strMemberNo == memberrnos)
                    .Select(k => k.strRegDate)
                    .FirstOrDefault();


                int monthsSinceJoining = ((DateTime.Now.Year - regdate.Year) * 12) + DateTime.Now.Month - regdate.Month;


                //if (monthsSinceJoining < maturity)
                //{
                //    return Json(new { error = "The member has not matured yet to take a loan." });
                //}
                var customerdetails = _context.MembersRegistrations
                    .FirstOrDefault(k =>
                        k.strMemberNo == memberrnos &&
                        k.IsApproved == true &&
                        k.HasPaidRegFee == true // Only if active
                    );

                if (customerdetails == null)
                {
                    //Notify("This member is not yet active or fully registered.", notificationType: NotificationType.error);
                    return Json(new { error = "This member is not yet active or fully registered." });
                }

                // If active member found
                names = customerdetails.strSurName + " " + customerdetails.strOtherName;
                membernos = customerdetails.strMemberNo;

                // Deposit
                totaldeposit = _context.Customer_Balance
                    .Where(k =>
                        (k.Transactioncode.Contains("D") || k.Transdescription.Contains("DEPOSIT") || k.Transdescription.Contains("SAVING")) &&
                        k.Memberno == customerdetails.strMemberNo
                    )
                    .Sum(k => k.Amount);

                var roundoffDeposit = Math.Round(totaldeposit, 2);

                // Share capital
                totalsharecapital = _context.Customer_Balance
                    .Where(k =>
                        (k.Transactioncode.ToLower().Contains("sc") || k.Transdescription.ToLower().Contains("share capital") || k.Transdescription.ToLower().Contains("capital")) &&
                        k.Memberno == customerdetails.strMemberNo
                    )
                    .Sum(k => k.Amount);

                var roundoffsharecapital = Math.Round(totalsharecapital, 2);

                // Max Loan based on matured shares
                var sharecodes = _context.Customer_Balance
                    .FirstOrDefault(k =>
                        (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving"))
                    );

                var loantoshareratio = _context.shareTypes
                    .FirstOrDefault(k => k.strSharecode == sharecodes.Transactioncode)
                    ?.strloanstoshareratio ?? "0";

                var totalmaturedshare = roundoffDeposit * Convert.ToDecimal(loantoshareratio);

                // Loan balances
                loanbal = _context.loanAplications
                    .Where(k => k.MemberNo == customerdetails.strMemberNo)
                    .Sum(k => k.LoanBalance) ?? 0;

                outstandingloan = _context.loanAplications
                    .Count(k => k.MemberNo == customerdetails.strMemberNo && k.LoanBalance > 0);

                totaldeposit = roundoffDeposit;
                totalsharecapital = roundoffsharecapital;
                maturedshares = roundoffDeposit;
                maxloan = totalmaturedshare;
            }

            // Search Filters
            if (!string.IsNullOrEmpty(memberss))
            {
                if (!string.IsNullOrEmpty(membersconditionss))
                {
                    if (membersconditionss == "MemberNo")
                    {
                        members = members.Where(i => i.strMemberNo.ToUpper().Contains(memberss.ToUpper())).ToList();
                    }
                    if (membersconditionss == "Name")
                    {
                        members = members.Where(i => i.strOtherName.ToUpper().Contains(memberss.ToUpper()) || i.strSurName.ToUpper().Contains(memberss.ToUpper())).ToList();
                    }
                    if (membersconditionss == "IdNo")
                    {
                        members = members.Where(i => i.strIdNo.ToUpper().Contains(memberss.ToUpper())).ToList();
                    }
                    if (membersconditionss == "PhoneNo")
                    {
                        members = members.Where(i => i.strPhoneNo.ToUpper().Contains(memberss.ToUpper())).ToList();
                    }
                }
            }

            // Latest 5 members only
            members = members.OrderByDescending(i => i.strMemberNo).Take(5).ToList();

            return Json(new
            {
                members,
                names,
                membernos,
                totaldeposit,
                totalsharecapital,
                maturedshares,
                maxloan,
                loanbal,
                outstandingloan
            });
        }

        [HttpPost]
        public JsonResult getguarantor([FromBody] Customer_Balance customer_Balance, string guarantorMemberno, string Guarantor, string guarantorscconditions)
        {

            var sacco = HttpContext.Session.GetString("CompanyCode");

            //shares
            var individualTotals = _context.Customer_Balance
            .GroupBy(k => k.Memberno)
            .Select(group => new
            {
                Id = group.First().Id,
                MemberNo = group.First().Memberno,
                Name = group.First().MemberName,
                TotalAmount = group.Sum(k => k.Amount),
            }).ToList();

            var names = "";
            var membernos = "";
            decimal totaldeposit = 0;
            decimal AmtAvailable = 0;
            if (guarantorMemberno != null)
            {

                var memberno = _context.Customer_Balance.Where(k => k.Memberno == guarantorMemberno).FirstOrDefault();

                var guarantormemberno = memberno.Memberno ?? "Unknown";
                var guaranteedAmount = _context.Loanguars
                .Where(k => k.MemberNo == guarantorMemberno)
                .Sum(k => k.AmountGuaranteed);
                if (guarantormemberno != null)
                {
                    var totalcurrentDeposit = _context.Customer_Balance.Where(k => (k.Transactioncode.ToLower().Contains("d") || k.Transdescription.ToLower().Contains("deposit") || k.Transdescription.ToLower().Contains("saving")) && k.Memberno == guarantormemberno).Sum(k => k.Amount);
                    totalcurrentDeposit = Math.Round(totalcurrentDeposit, 2);
                    names = memberno.MemberName;
                    membernos = memberno.Memberno;
                    totaldeposit = totalcurrentDeposit;

                    AmtAvailable = totaldeposit - guaranteedAmount;
                }
            }

            if (!string.IsNullOrEmpty(guarantorMemberno))
            {
                if (!string.IsNullOrEmpty(guarantorscconditions))
                {
                    if (guarantorscconditions == "MemberNo")
                    {
                        individualTotals = individualTotals.Where(k => k.MemberNo == guarantorMemberno).ToList();
                    }
                    if (guarantorscconditions == "Name")
                    {
                        individualTotals = individualTotals.Where(k => k.Name.Contains(guarantorMemberno.ToUpper())).ToList();
                    }
                }
            }

            individualTotals = individualTotals.OrderByDescending(k => k.MemberNo).Take(15).ToList();
            return Json(new
            {
                names,
                membernos,
                totaldeposit,
                individualTotals,
                AmtAvailable
            });


        }

        // GET: LoanAplications/Create
        public IActionResult Create(int? id)
        {
            utilities.SetUpPrivileges(this);
            if (id != null)
            {

                var membersRegistration = _context.MembersRegistrations.Find(id);
                //Deposit
                var totaldeposit = _context.Customer_Balance.Where(k => (k.Transactioncode.Contains("D") || k.Transdescription.Contains("DEPOSIT") || k.Transdescription.Contains("SAVING")) && k.Memberno == membersRegistration.strMemberNo).Sum(k => k.Amount);
                var roundoffDeposit = Math.Round(totaldeposit, 2);

                //matured share
                var sharecodes = _context.Customer_Balance.FirstOrDefault(k => (k.Transactioncode.Contains("D") || k.Transdescription.Contains("DEPOSIT") || k.Transdescription.Contains("SAVING")));
                var loantoshareratio = _context.shareTypes.FirstOrDefault(k => k.strSharecode == sharecodes.Transactioncode).strloanstoshareratio;
                var totalmaturedshare = (roundoffDeposit * Convert.ToDecimal(loantoshareratio));


                var totalsharecapital = _context.Customer_Balance.Where(k => (k.Transactioncode.Contains("SC") || k.Transdescription.Contains("share capital") || k.Transdescription.Contains("capital")) && k.Memberno == membersRegistration.strMemberNo).Sum(k => k.Amount);
                var roundoffsharecapital = Math.Round(totalsharecapital, 2);
                if (membersRegistration != null)
                {
                    ViewBag.name = membersRegistration.strOtherName + "  " + membersRegistration.strSurName;
                    ViewBag.memberno = (membersRegistration.strMemberNo).Trim();
                    ViewBag.deposit = roundoffDeposit;
                    ViewBag.capital = roundoffsharecapital;
                    ViewBag.maturedshares = roundoffDeposit;
                    ViewBag.maxloam = totalmaturedshare;


                }
            }




            ViewBag.totalshare = 0.00;
            ViewBag.totalshareCapital = 0.00;
            ViewBag.maxloan = 0.00;
            ViewBag.outstanding = 0.00;
            ViewBag.loanbal = 0.00;



            var loantypes = new SelectList(_context.loanTYpes, "strloancode", "strLoanType").ToList();
            ViewBag.loantypes = loantypes;



            Random r = new Random();
            var x = r.Next(55000000, 59000000);
            var y = r.Next(10, 99);
            var finalcode = x + "-0" + y;
            var ifexist = _context.shareTransactions.FirstOrDefaultAsync(k => k.strDocnumber == finalcode);
            if (ifexist != null)
            {

                finalcode = x + "-0" + y;
            }
            else
            {
                x = r.Next(55000000, 59000000);
                y = r.Next(10, 99);
                finalcode = x + "-0" + y;
            }

            ViewBag.Code = finalcode;

            return View();
        }

        // POST: LoanAplications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        public IActionResult Test()
        {
            double pow_ab = Math.Pow(6, -2);//base , power
            return Content(pow_ab.ToString());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanAplication loanAplication)
        {



            return View(loanAplication);
        }

        // GET: LoanAplications/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var loanAplication = await _context.loanAplications.FindAsync(id);

            if (loanAplication == null)
            {
                return NotFound();
            }


            return View(loanAplication);
        }

        // POST: LoanAplications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("strId,strLoanTypeId,strMemberId,strLoanNo,strAmount,strApplicationDate,strRepaymentPeriod,strInterestRate,strRepaymentMethod,strEndoresmentDate,strEndorsementStatus")] LoanAplication loanAplication)
        {
            if (id != loanAplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loanAplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanAplicationExists(loanAplication.Id))
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
            return View(loanAplication);
        }

        // GET: LoanAplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var loanAplication = await _context.loanAplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanAplication == null)
            {
                return NotFound();
            }

            return View(loanAplication);
        }

        // POST: LoanAplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var loanAplication = await _context.loanAplications.FindAsync(id);
            _context.loanAplications.Remove(loanAplication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanAplicationExists(int? id)
        {
            return _context.loanAplications.Any(e => e.Id == id);
        }
    }
}
