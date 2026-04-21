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

namespace EASY_SACCO.Controllers
{
    public class LoanRepayViewModel2
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

  
    public class ShareTransactionController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        public ShareTransactionController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            
        }

        // GET: ShareTransactions
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            return View(await _context.Customer_Balance.ToListAsync());
        }
        public  IActionResult Shares_loansEnquiry()
        {
            var MemberNo = HttpContext.Session.GetString("MemberNo");
            ViewBag.MemberNo = MemberNo;
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
                var memberno = HttpContext.Session.GetString("MemberNo");
                if(memberno!= null) 
                {

                    return View(await _context.Customer_Balance.Where(k=>k.Memberno==memberno && k.Transactioncode!="Loan").ToListAsync());
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
            var memberno = HttpContext.Session.GetString("MemberNo");
            var Loanno = "";
            var Loancode = "";
            int Repayperiod =0;
            var Repaymemthod = "";
            decimal InterestRate = 0;

            loans = _context.loanAplications.Where(k => k.status == 4 && k.MemberNo == memberno).FirstOrDefault();

            Random r = new Random();
            DateTime dateTime = DateTime.Now;
            var date = dateTime.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
            var y = r.Next(10, 99);
            var finalcode = "RCP" + date + "-0" + y;
            var ifexist = _context.shareTransactions.FirstOrDefaultAsync(k => k.strDocnumber == finalcode);
            if (ifexist != null)
            {

                finalcode = "RCP" + date + "-0" + y;
            }
            else
            {

                y = r.Next(10, 99);
                finalcode = "RCP" + date + "-0" + y;
            }

            var Receiptno = finalcode;




                if (loans != null)
                {
                Loanno = loans.LoanNo;
                Loancode= loans.LoanCode;
                Repayperiod = loans.RepaymentPeriod ?? 0;
                Repaymemthod = loans.RepaymentMethod;
                InterestRate = loans.InterestRate;


                }

            return Json(new { Loanno , Loancode, Repayperiod, Repaymemthod, InterestRate, Receiptno });
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
            memberno = HttpContext.Session.GetString("MemberNo");
            var loans = _context.loanAplications.Where(k => k.MemberNo == memberno).FirstOrDefault();
            var loannos = "";
            var loancodes = "";
            var repaymethods = "";
            decimal interestrates =0;
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
        public JsonResult getActiveloans([FromBody] LoanAplication loanAplication, string filter)
        {

            var sacco = HttpContext.Session.GetString("CompanyCode");
            filter = HttpContext.Session.GetString("MemberNo");
            var loans = _context.loanAplications.Where(k => k.MemberNo == filter).FirstOrDefault();
            var result = "Erro";
            if (loans != null)
            {
                var schedule = new List<LoanScheduleItem>();
                double remainingLoanAmount = Convert.ToDouble(loans.AppliedAmount);
                double monthlyInterestRate = Convert.ToDouble(loans.InterestRate / 12 / 100);
                int numberOfPayments = loans.RepaymentPeriod ?? 0;
                double principal = 0;
                DateTime dueDate = loans.LastRepayDate;

                for (int i = 0; i < numberOfPayments; i++)
                {
                    double interest = remainingLoanAmount * monthlyInterestRate;

                    var memberNos = loans.MemberNo;


                    if (loans.RepaymentMethod == "AMRT")
                    {


                        principal = (Convert.ToDouble(loans.LoanBalance) * monthlyInterestRate) / (1 - Math.Pow(1 + monthlyInterestRate, -numberOfPayments));

                        principal = principal - interest;
                    }
                    else if (loans.RepaymentMethod == "RBAL")
                    {
                        principal = (Convert.ToDouble(loans.LoanBalance) / numberOfPayments);


                    }
                    else if (loans.RepaymentMethod == "STL")
                    {
                        principal = (Convert.ToDouble(loans.LoanBalance) / numberOfPayments);
                        interest = (Convert.ToDouble(loans.LoanBalance) * monthlyInterestRate);
                    }

                    double totalPayment = interest + principal;
                    remainingLoanAmount -= principal;

                 //   DateTime dueDate = loans.LastRepayDate.AddMonths(i + 1);

                    if (dueDate == loans.LastRepayDate)
                    {
                        dueDate = loans.LastRepayDate.AddMonths(i + 1); // Move to the next month for the due date
                    }
                    else
                    {
                        dueDate = loans.LastRepayDate; // Set the initial due date
                    }
                  

                    if (loans.LastRepayDate != dueDate)
                    {

                        schedule.Add(new LoanScheduleItem
                        {
                            DueDate = dueDate,
                            Principal = principal,
                            Interest = interest,
                            TotalPayment = totalPayment,
                            RemainingBalance = remainingLoanAmount,
                            Memberno = memberNos
                        });

                        break;
                    }

                }
                // Sort the schedule by DueDate
                var schedules = schedule.GroupBy(item => item.DueDate).Select(group => new
                {
                    DueDate = group.First().DueDate.ToShortDateString(),
                    Principal = group.First().Principal,
                    Interest = group.First().Interest,
                    TotalPayment = group.First().TotalPayment,
                    RemainingBalance = group.First().RemainingBalance,
                    Memberno = group.First().Memberno,
                }).ToList();
                

                return Json(schedules);

            }
            return Json(result);
          
        }

        [HttpPost]
        public JsonResult getmemberstatement([FromBody] Customer_Balance customer_Balance, string filter)
        {

            var MemberNo = HttpContext.Session.GetString("MemberNo");
            var sacco = HttpContext.Session.GetString("CompanyCode");
            var loans=  _context.loanAplications.Where(k => k.status ==4 && k.MemberNo==MemberNo).ToList();
            var members = _context.MembersRegistrations.Where(i => i.strMemberNo==MemberNo).ToList();
            var Names = "";
            decimal Sharecapital = 0;
            decimal Deposit = 0;
            decimal loanbal = 0;
            int outstandingloan = 0;
            //shares
            var individualTotals = _context.Customer_Balance
             .Where(x => x.Transactioncode != "Loan"&&x.Memberno==MemberNo)
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
                    loans = _context.loanAplications.Where(k => k.status == 4&&k.MemberNo==filter).ToList();
                    loanbal = _context.loanAplications.Where(k => k.MemberNo == filter && k.status == 4).Sum(k => k.LoanBalance) ?? 0;
                    outstandingloan = _context.loanAplications.Where(k => k.MemberNo == filter && k.LoanBalance > 0 && k.status == 4).Count();
                    members = _context.MembersRegistrations.Where(i => i.strCompanyCode.ToUpper().Equals(sacco.ToUpper()) && i.strMemberNo == filter).ToList();
                    Names = members.FirstOrDefault().strSurName + " " + members.FirstOrDefault().strOtherName;
                    
                    individualTotals = individualTotals.Where(i => i.MemberNo==filter).ToList();
                    }
                   
        
            }
            individualTotals = individualTotals.OrderByDescending(k => k.MemberNo).Take(15).ToList();

            
            return Json(new { Sharecapital , Deposit, Names, individualTotals , loans,
                loanbal,
                outstandingloan
            });
        }


        [HttpPost]
        public JsonResult getmembers([FromBody] MembersRegistration membersRegistration, string filter, string condition, int? memberId,string membernoss,string memberno1)
        {

            var sacco = HttpContext.Session.GetString("CompanyCode");
            filter= HttpContext.Session.GetString("MemberNo");
            var members = _context.MembersRegistrations.Where(i => i.strCompanyCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var names = "";
            var memberno ="";
            if (memberId != null)
            {
                var memberss = _context.MembersRegistrations.Find(memberId);
                names = memberss.strOtherName + "  " + memberss.strSurName;
                memberno = memberss.strMemberNo;

            }
            if (memberno1!= null)
            {
                var memberss = _context.MembersRegistrations.Where(K=>K.strMemberNo==memberno1).FirstOrDefault();
                names = memberss.strOtherName + "  " + memberss.strSurName;
                
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
                        members = members.Where(i => i.strPhoneNo.ToUpper().Contains(filter.ToUpper())).ToList();
                    }

                }
            }

            members = members.OrderByDescending(i => i.strMemberNo).Take(5).ToList();
            return Json(new { members , names , memberno } );
        }
        
       
        [HttpPost]
        public JsonResult getsharestransaction([FromBody] ShareTransaction shareTransaction)
        {
            

            var sacco = HttpContext.Session.GetString("CompanyCode");

            var transactions = _context.Customer_Balance.Where(i => i.CompanyCode.ToUpper().Equals(sacco.ToUpper())&&i.Transactioncode!="Loan").ToList();
            if (shareTransaction != null)
            {
                if (shareTransaction.StartDate != null && shareTransaction.EndDate != null)
                {

                    transactions = transactions.Where(i => (i.TransDate >= shareTransaction.StartDate && i.TransDate <= shareTransaction.EndDate)).ToList();

                }

            }


            else
            {
                transactions = transactions.OrderByDescending(i => i.TransDate.ToShortDateString()).Take(10).ToList();

            }
            return Json(transactions);
        }


        public class Member
        {
            public string MemberNo { get; set; }
            public string MemberName { get; set; }
        }

        // GET: ShareTransactions/Create
        public IActionResult Create(int? id)
        {
            utilities.SetUpPrivileges(this);
            if (id != null)
            {

                var membersRegistration = _context.MembersRegistrations.Find(id);
                if (membersRegistration != null)
                {
                    ViewBag.name = membersRegistration.strOtherName + "  " + membersRegistration.strSurName;
                    ViewBag.memberno = membersRegistration.strMemberNo;



                }
            }
            ViewBag.sharecode = new SelectList(_context.shareTypes, "strSharecode", "strShareType").ToList();
            ViewBag.strBankName = _context.banksSetups.ToList();
            ViewBag.strSharecode = _context.shareTypes.ToList();


            Random r = new Random();
            DateTime dateTime = DateTime.Now;
            var date =dateTime.ToString().Replace("/", "").Replace(" ","").Replace(":","");
            var y = r.Next(10, 99);
            var finalcode = "RCP" + date + "-0" + y;
            var ifexist = _context.shareTransactions.FirstOrDefaultAsync(k => k.strDocnumber == finalcode);
            if (ifexist != null)
            {

                finalcode = "RCP" + date + "-0" + y;
            }
            else
            {
                
                y = r.Next(10, 99);
                finalcode = "RCP" + date + "-0" + y;
            }

            ViewBag.Code = finalcode;
            return View();
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



            Random r = new Random();
            var x = r.Next(55000000, 59000000);
            var y = r.Next(10, 99);
            var finalcode = "RCP" + x + "-0" + y;
            var ifexist = await _context.shareTransactions.FirstOrDefaultAsync(k => k.strDocnumber == finalcode);
            if (ifexist != null)
            {

                finalcode = "RCP" + x + "-0" + y;
            }
            else
            {
                x = r.Next(55000000, 59000000);
                y = r.Next(10, 99);
                finalcode = "RCP" + x + "-0" + y;
            }

            ViewBag.Code = finalcode;
            if (ModelState.IsValid)
            {
                _context.Add(shareTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shareTransaction);
        }
        [HttpPost]
        public JsonResult SaveOrder2([FromBody] LoanRepayViewModel2 loanRepays)
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
                    var lastrepaydates = _context.loanAplications.FirstOrDefault(k => k.MemberNo == loanRepays.MemberNo);
                    var Datereceived = loanRepays.DateReceived;

                    var Totaldays = Datereceived.Subtract(lastrepaydates.LastRepayDate).TotalDays;
                    var loanapplied = _context.loanAplications.Where(k => k.MemberNo == loanRepays.MemberNo).FirstOrDefault();
                    var AmountApproved = loanapplied.ApprovedAmount;

                    decimal monthlyInterestRate = (loanRepays.InterestRate / 12 / 100);

                    if (Totaldays >= 0 && Totaldays <= 31)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0;
                    }
                    else if (Totaldays >= 32 && Totaldays <= 63)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 2;
                    }
                    else if (Totaldays >= 64 && Totaldays <= 95)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 3;
                    }

                    else if (Totaldays >= 96 && Totaldays <= 127)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 4;
                    }
                    else if (Totaldays >= 128 && Totaldays <= 159)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 5;
                    }
                    else if (Totaldays >= 160 && Totaldays <= 191)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 6;
                    }
                    else if (Totaldays >= 192 && Totaldays <= 223)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 7;
                    }
                    else if (Totaldays >= 224 && Totaldays <= 255)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 8;
                    }
                    else if (Totaldays >= 256 && Totaldays <= 287)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 9;
                    }
                    else if (Totaldays >= 288 && Totaldays <= 319)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 10;
                    }
                    else if (Totaldays >= 320 && Totaldays <= 351)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 11;
                    }
                    else if (Totaldays >= 352 && Totaldays <= 383)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 12;
                    }
                    else if (Totaldays >= 384 && Totaldays <= 415)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 13;
                    }
                    else if (Totaldays >= 416 && Totaldays <= 447)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 14;
                    }
                    else if (Totaldays >= 448 && Totaldays <= 479)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 15;
                    }
                    else if (Totaldays >= 480 && Totaldays <= 511)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 16;
                    }

                    else if (Totaldays >= 512 && Totaldays <= 543)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 17;
                    }
                    else if (Totaldays >= 544 && Totaldays <= 575)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 18;
                    }
                    else if (Totaldays >= 576 && Totaldays <= 607)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 19;
                    }
                    else if (Totaldays >= 608 && Totaldays <= 639)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 20;
                    }
                    else if (Totaldays >= 640 && Totaldays <= 671)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 21;
                    }
                    else if (Totaldays >= 672 && Totaldays <= 703)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 22;
                    }
                    else if (Totaldays >= 704 && Totaldays <= 735)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 23;
                    }
                    else if (Totaldays >= 736 && Totaldays <= 767)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 24;
                    }
                    else if (Totaldays >= 768 && Totaldays <= 799)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 25;
                    }
                    else if (Totaldays >= 800 && Totaldays <= 831)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 26;
                    }
                    else if (Totaldays >= 832 && Totaldays <= 863)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 27;
                    }
                    else if (Totaldays >= 894 && Totaldays <= 925)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 28;
                    }

                    else if (Totaldays >= 926 && Totaldays <= 957)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 29;
                    }
                    else if (Totaldays >= 958 && Totaldays <= 989)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 30;
                    }
                    else if (Totaldays >= 990 && Totaldays <= 1021)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 31;
                    }
                    else if (Totaldays >= 1022 && Totaldays <= 1053)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 32;
                    }
                    else if (Totaldays >= 1054 && Totaldays <= 1085)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 33;
                    }
                    else if (Totaldays >= 1086 && Totaldays <= 1117)
                    {
                        Interest = (AmountApproved * monthlyInterestRate) ?? 0 * 34;
                    }


                    var loanbal = _context.loanAplications.Where(k => k.MemberNo == loanRepays.MemberNo).FirstOrDefault();
                    var loanbalance = loanbal.LoanBalance;



                    Principal1 = (AmountApproved / loanRepays.RepaymentPeriod) ?? 0;


                    var AmountRepay = loanRepays.Amount;

                    Principal2 = AmountRepay - Interest;



                    var loanbalances = (loanbalance - Principal2);


                    var norepay = _context.REPAY.Where(k => k.MemberNo == loanRepays.MemberNo).Count();
                    var list = loanRepays.customerBalances.FirstOrDefault();


                    var repay = new RepayMentTable();
                    repay.LoanNo = list.Loanno;
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

                    var updateloanapplication = _context.loanAplications.FirstOrDefault(k => k.MemberNo == repay.MemberNo);
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
                    var bankcode = accounts.DRaccno;

                    //gltransactions
                    var newrecords = new GL_Transaction();

                    newrecords.MemberNo = loanRepays.MemberNo;
                    newrecords.TransactionDate = loanRepays.DateReceived;
                    newrecords.Amount = Principal2;
                    newrecords.strDocnumber = loanRepays.ReceiptNo;
                    newrecords.CrAccNo = bankcode;
                    newrecords.DrAccNo = glaccoun;
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
                    newrecords1.CrAccNo = bankcode;
                    newrecords1.DrAccNo = interestacounNo;
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
                    newrecords2.CrAccNo = bankcode;
                    newrecords2.DrAccNo = penaltyacounNo;
                    newrecords2.Remarks = "Penalty Repayment" + loanRepays.MemberNo;
                    newrecords2.CompanyCode = sacco;
                    newrecords2.TransactionNo = DateTime.Now + loanRepays.MemberNo;
                    newrecords2.AuditID = auditid;
                    _context.GL_Transactions.Add(newrecords2);
                    _context.SaveChanges();
                }

                else if (loanRepays.RepaymentMethod == "AMRT")
                {
                    var loanbal = _context.loanAplications.Where(k => k.MemberNo == loanRepays.MemberNo).FirstOrDefault();
                    var loanbalance = loanbal.LoanBalance;
                    //decimal remainingLoanAmount = loanbalance;

                    var lastrepaydates = _context.loanAplications.FirstOrDefault(k => k.MemberNo == loanRepays.MemberNo);
                    var Datereceived = loanRepays.DateReceived;

                    var Totaldays = Datereceived.Subtract(lastrepaydates.LastRepayDate).TotalDays;

                    decimal monthlyInterestRate = (loanRepays.InterestRate / 12 / 100);
                    int repayperiod = loanRepays.RepaymentPeriod;

                    //loanapplied
                    var loanapplied = _context.loanAplications.Where(k => k.MemberNo == loanRepays.MemberNo).FirstOrDefault();
                    var AmountApproved = loanapplied.ApprovedAmount;



                    if (Totaldays >= 0 && Totaldays <= 31)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0;
                    }
                    else if (Totaldays >= 32 && Totaldays <= 63)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 2;
                    }
                    else if (Totaldays >= 64 && Totaldays <= 95)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 3;
                    }

                    else if (Totaldays >= 96 && Totaldays <= 127)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 4;
                    }
                    else if (Totaldays >= 128 && Totaldays <= 159)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 5;
                    }
                    else if (Totaldays >= 160 && Totaldays <= 191)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 6;
                    }
                    else if (Totaldays >= 192 && Totaldays <= 223)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 7;
                    }
                    else if (Totaldays >= 224 && Totaldays <= 255)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 8;
                    }
                    else if (Totaldays >= 256 && Totaldays <= 287)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 9;
                    }
                    else if (Totaldays >= 288 && Totaldays <= 319)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 10;
                    }
                    else if (Totaldays >= 320 && Totaldays <= 351)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 11;
                    }
                    else if (Totaldays >= 352 && Totaldays <= 383)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 12;
                    }
                    else if (Totaldays >= 384 && Totaldays <= 415)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 13;
                    }
                    else if (Totaldays >= 416 && Totaldays <= 447)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 14;
                    }
                    else if (Totaldays >= 448 && Totaldays <= 479)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 15;
                    }
                    else if (Totaldays >= 480 && Totaldays <= 511)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 16;
                    }

                    else if (Totaldays >= 512 && Totaldays <= 543)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 17;
                    }
                    else if (Totaldays >= 544 && Totaldays <= 575)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 18;
                    }
                    else if (Totaldays >= 576 && Totaldays <= 607)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 19;
                    }
                    else if (Totaldays >= 608 && Totaldays <= 639)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 20;
                    }
                    else if (Totaldays >= 640 && Totaldays <= 671)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 21;
                    }
                    else if (Totaldays >= 672 && Totaldays <= 703)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 22;
                    }
                    else if (Totaldays >= 704 && Totaldays <= 735)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 23;
                    }
                    else if (Totaldays >= 736 && Totaldays <= 767)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 24;
                    }
                    else if (Totaldays >= 768 && Totaldays <= 799)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 25;
                    }
                    else if (Totaldays >= 800 && Totaldays <= 831)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 26;
                    }
                    else if (Totaldays >= 832 && Totaldays <= 863)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 27;
                    }
                    else if (Totaldays >= 894 && Totaldays <= 925)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 28;
                    }

                    else if (Totaldays >= 926 && Totaldays <= 957)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 29;
                    }
                    else if (Totaldays >= 958 && Totaldays <= 989)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 30;
                    }
                    else if (Totaldays >= 990 && Totaldays <= 1021)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 31;
                    }
                    else if (Totaldays >= 1022 && Totaldays <= 1053)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 32;
                    }
                    else if (Totaldays >= 1054 && Totaldays <= 1085)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 33;
                    }
                    else if (Totaldays >= 1086 && Totaldays <= 1117)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 34;
                    }


                    Principal1 = AmountApproved * (decimal)monthlyInterestRate / (1 - (decimal)Math.Pow((double)(1 + (decimal)monthlyInterestRate), -repayperiod)) ?? 0;
                    Interest = loanbalance * monthlyInterestRate ?? 0;
                    Principal1 = Principal1 - Interest;

                    var AmountRepay = loanRepays.Amount;

                    Principal2 = AmountRepay - Interest;



                    var loanbalances = loanbalance - Principal2;

                    var no_ofrepay = _context.REPAY.Where(k => k.MemberNo == loanRepays.MemberNo).Count();
                    var list = loanRepays.customerBalances.FirstOrDefault();


                    var repay = new RepayMentTable();
                    repay.LoanNo = list.Loanno;
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

                    var updateloanapplication = _context.loanAplications.FirstOrDefault(k => k.MemberNo == repay.MemberNo);
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
                    var bankcode = accounts.DRaccno;

                    //gltransactions
                    var newrecords = new GL_Transaction();

                    newrecords.MemberNo = loanRepays.MemberNo;
                    newrecords.TransactionDate = loanRepays.DateReceived;
                    newrecords.Amount = Principal2;
                    newrecords.strDocnumber = loanRepays.ReceiptNo;
                    newrecords.CrAccNo = glaccoun;
                    newrecords.DrAccNo = bankcode ;
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


                }
                else if (loanRepays.RepaymentMethod == "RBAL")
                {
                    var loanbal = _context.loanAplications.Where(k => k.MemberNo == loanRepays.MemberNo).FirstOrDefault();
                    var loanbalance = loanbal.LoanBalance;
                    decimal remainingLoanAmount = loanbalance ?? 0;

                    var lastrepaydates = _context.loanAplications.FirstOrDefault(k => k.MemberNo == loanRepays.MemberNo);
                    var Datereceived = loanRepays.DateReceived;

                    var Totaldays = Datereceived.Subtract(lastrepaydates.LastRepayDate).TotalDays;

                    decimal monthlyInterestRate = (loanRepays.InterestRate / 12 / 100);
                    int repayperiod = loanRepays.RepaymentPeriod;

                    //loanapplied
                    var loanapplied = _context.loanAplications.Where(k => k.MemberNo == loanRepays.MemberNo).FirstOrDefault();
                    var AmountApproved = loanapplied.ApprovedAmount;



                    if (Totaldays >= 0 && Totaldays <= 31)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0;
                    }
                    else if (Totaldays >= 32 && Totaldays <= 63)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0;
                    }
                    else if (Totaldays >= 64 && Totaldays <= 95)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 3;
                    }

                    else if (Totaldays >= 96 && Totaldays <= 127)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 4;
                    }
                    else if (Totaldays >= 128 && Totaldays <= 159)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 5;
                    }
                    else if (Totaldays >= 160 && Totaldays <= 191)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 6;
                    }
                    else if (Totaldays >= 192 && Totaldays <= 223)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 7;
                    }
                    else if (Totaldays >= 224 && Totaldays <= 255)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 8;
                    }
                    else if (Totaldays >= 256 && Totaldays <= 287)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 9;
                    }
                    else if (Totaldays >= 288 && Totaldays <= 319)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 10;
                    }
                    else if (Totaldays >= 320 && Totaldays <= 351)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 11;
                    }
                    else if (Totaldays >= 352 && Totaldays <= 383)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 12;
                    }
                    else if (Totaldays >= 384 && Totaldays <= 415)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 13;
                    }
                    else if (Totaldays >= 416 && Totaldays <= 447)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 14;
                    }
                    else if (Totaldays >= 448 && Totaldays <= 479)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 15;
                    }
                    else if (Totaldays >= 480 && Totaldays <= 511)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 16;
                    }

                    else if (Totaldays >= 512 && Totaldays <= 543)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 17;
                    }
                    else if (Totaldays >= 544 && Totaldays <= 575)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 18;
                    }
                    else if (Totaldays >= 576 && Totaldays <= 607)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 19;
                    }
                    else if (Totaldays >= 608 && Totaldays <= 639)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 20;
                    }
                    else if (Totaldays >= 640 && Totaldays <= 671)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 21;
                    }
                    else if (Totaldays >= 672 && Totaldays <= 703)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 22;
                    }
                    else if (Totaldays >= 704 && Totaldays <= 735)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 23;
                    }
                    else if (Totaldays >= 736 && Totaldays <= 767)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 24;
                    }
                    else if (Totaldays >= 768 && Totaldays <= 799)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 25;
                    }
                    else if (Totaldays >= 800 && Totaldays <= 831)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 26;
                    }
                    else if (Totaldays >= 832 && Totaldays <= 863)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 27;
                    }
                    else if (Totaldays >= 894 && Totaldays <= 925)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 28;
                    }

                    else if (Totaldays >= 926 && Totaldays <= 957)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 29;
                    }
                    else if (Totaldays >= 958 && Totaldays <= 989)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 30;
                    }
                    else if (Totaldays >= 990 && Totaldays <= 1021)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 31;
                    }
                    else if (Totaldays >= 1022 && Totaldays <= 1053)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 32;
                    }
                    else if (Totaldays >= 1054 && Totaldays <= 1085)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 33;
                    }
                    else if (Totaldays >= 1086 && Totaldays <= 1117)
                    {
                        Interest = (loanbalance * monthlyInterestRate) ?? 0 * 34;
                    }






                    Principal1 = (AmountApproved/loanRepays.RepaymentPeriod ?? 0);

                    var AmountRepay = loanRepays.Amount;

                    Principal2 = AmountRepay - Interest;



                    var loanbalances = (loanbalance - Principal2);

                    var no_ofrepay = _context.REPAY.Where(k => k.MemberNo == loanRepays.MemberNo).Count();
                    var list = loanRepays.customerBalances.FirstOrDefault();


                    var repay = new RepayMentTable();
                    repay.LoanNo = loanRepays.MemberNo;
                    repay.MemberNo = list.Loanno;
                    repay.Loancode = loanRepays.Loancode;
                    repay.Principal = Principal1;
                    repay.Interest = Interest;
                    repay.TotalRepayment = (Principal1 + Interest);
                    repay.CompanyCode = sacco;
                    repay.nextduedate = date.AddMonths(1);
                    repay.Penalty = 0;
                    repay.DateReceived = date;
                    repay.AuditID = auditid;
                    repay.ReceiptNo = loanRepays.ReceiptNo;
                    repay.PaymentNo = no_ofrepay + 1;
                    repay.Remarks = loanRepays.Remarks.Trim();
                    repay.LoanBalance = loanbalances ?? 0;
                    _context.REPAY.Add(repay);

                    var updateloanapplication = _context.loanAplications.FirstOrDefault(k => k.MemberNo == repay.MemberNo);
                    if (updateloanapplication != null)
                    {
                        _context.Entry(updateloanapplication).State = EntityState.Modified;
                        updateloanapplication.LoanBalance = loanbalances;
                        _context.SaveChanges();
                    }

                    _context.SaveChanges();

                    var glaccoun = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strLoanAccount;
                    var interestacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strInterestAccount;
                    var penaltyacounNo = _context.loanTYpes.Where(k => k.strloancode == loanRepays.Loancode).FirstOrDefault().strPenaltyAccount;


                    var accounts = _context.Customer_Balance.FirstOrDefault(m => m.Memberno == loanRepays.MemberNo);

                    var CrAccno1 = accounts.CRaccno;
                    var bankcode = accounts.DRaccno;

                    //gltransactions
                    var newrecords = new GL_Transaction();

                    newrecords.MemberNo = loanRepays.MemberNo;
                    newrecords.TransactionDate = loanRepays.DateReceived;
                    newrecords.Amount = Principal2;
                    newrecords.strDocnumber = loanRepays.ReceiptNo;
                    newrecords.CrAccNo = bankcode;
                    newrecords.DrAccNo = glaccoun;
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
                }

            }
            if (loanRepays.customerBalances != null)
            { 

            foreach (var item in loanRepays.customerBalances)
            {

                //var sharecode1 = _context.shareTypes.FirstOrDefault(k => k.strShareType.Contains(item.Transactioncode.ToLower())).strSharecode;

                var CrAccno1 = _context.shareTypes.FirstOrDefault(k => k.strSharecode.Contains(item.Transactioncode.ToLower())).GLaccno;
                var bankcode = _context.banksSetups.FirstOrDefault(k => k.strBankName.Contains(item.DRaccno.ToUpper())).strBankAccno;

                var newrecord = new Customer_Balance();

                newrecord.Memberno = item.Memberno;
                newrecord.MemberName = item.MemberName;
                newrecord.TransDate = item.TransDate;
                newrecord.Transactioncode = item.Transactioncode;
                newrecord.Amount = item.Amount;
                newrecord.Bookbalance = 0;
                newrecord.Availablebalance = 0;
                newrecord.Accountno = item.Memberno;
                newrecord.TransDate = item.TransDate;
                newrecord.Documentnumber = item.Documentnumber;

                newrecord.CRaccno = CrAccno1;
                newrecord.DRaccno = bankcode;
                newrecord.Transdescription = item.Transdescription.Trim();
                newrecord.CompanyCode = sacco;
                newrecord.Transactionno = DateTime.Now + item.Memberno;
                newrecord.Loanno = "0";
                newrecord.Soure = "BOSA";
                newrecord.CompanyCode = sacco;
                newrecord.Auditid = auditid;


                _context.Customer_Balance.Add(newrecord);
                _context.SaveChanges();
                //result = "Success! Record Is Complete!";

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
                result = "Success! Record Saved Successfully!";

            }
            }
            loanRepays.customerBalances.Clear();
            return Json(result);
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
