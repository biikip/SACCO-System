using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using static iText.Kernel.Pdf.Colorspace.PdfShading;

namespace EASY_SACCO.Controllers
{
    public class HomeController : BaseController
    {
        public static string Username = "";
        private readonly BOSA_DBContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        private readonly ILogger<HomeController> _logger;


        public HomeController(BOSA_DBContext context, INotyfService notyf, ILogger<HomeController> logger)
            : base(context)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
            _logger = logger;
        }
        public IActionResult Index()
        {
            DateTime currentDatetime = DateTime.Now;
            DateOnly currentDate = DateOnly.FromDateTime(currentDatetime);
            TimeOnly currentTime = TimeOnly.FromDateTime(currentDatetime);

            var loggedInUser = HttpContext.Session.GetString("UserName");
            if (!string.IsNullOrEmpty(loggedInUser))
            {
                utilities.SetUpPrivileges(this);
            }
            return View();
        }

        public IActionResult Save()
        {
            try
            {
                //try save data into database
                Notify("Data saved successfully");
            }
            catch (Exception)
            {

            }

            return RedirectToAction(nameof(Dashboard));
        }


        public IActionResult Delete()
        {
            try
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                Notify("Could not delete data!", notificationType: NotificationType.error);
            }

            return RedirectToAction(nameof(Dashboard));
        }
        public class LoanModel
        {
            public double LoanAmount { get; set; }
            public double InterestRate { get; set; }
            public int LoanTerm { get; set; }
            public string RepaymentMethod { get; set; }
            public DateTime StartDate { get; set; }
        }
        public class LoanScheduleItem
        {
            public DateTime DueDate { get; set; }
            public double Principal { get; set; }
            public double Interest { get; set; }
            public double TotalPayment { get; set; }
            public double RemainingBalance { get; set; }
        }
        public IActionResult Calculate()
        {
            return View();
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
        public IActionResult MembersReports(string strCompanyName)
        {
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();
            general.locations = _context.Location.ToList();
            if (strCompanyName != null)
            {
                general.membersRegistrations = _context.MembersRegistrations.Where(k => k.strCompanyCode == strCompanyName).ToList();
            }
            else
            {
                general.membersRegistrations = _context.MembersRegistrations.ToList();

            }

            general.CIG_Croups = _context.CIG_Croups.ToList();
            general.co_Operatives = _context.co_Operatives.ToList();
            general.stations = _context.stations.ToList();
            general.locations = _context.Location.ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            return View(general);
        }
        public IActionResult MembersReportsPerSaccos()
        {
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();
            general.locations = _context.Location.ToList();

            general.membersRegistrations = _context.MembersRegistrations.ToList();



            general.CIG_Croups = _context.CIG_Croups.ToList();
            general.co_Operatives = _context.co_Operatives.ToList();
            general.stations = _context.stations.ToList();
            general.locations = _context.Location.ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            return View(general);
        }
        public IActionResult MembersReportsPerCounty(string countyId)
        {
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();
            if (countyId != null)
            {
                general.membersRegistrations = _context.MembersRegistrations.Where(k => k.strCountyId == countyId).ToList();

            }
            else
            {
                general.membersRegistrations = _context.MembersRegistrations.ToList();

            }




            general.CIG_Croups = _context.CIG_Croups.ToList();
            general.co_Operatives = _context.co_Operatives.ToList();
            general.stations = _context.stations.ToList();
            general.locations = _context.Location.ToList();
            ViewBag.strCompanyId = _context.Location.ToList();
            return View(general);
        }

        public IActionResult CIGsGroupReports(string strCompanyId)
        {
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();
            general.locations = _context.Location.ToList();
            if (strCompanyId != null)
            {
                general.CIG_Croups = _context.CIG_Croups.Where(k => k.strCompanyCode == strCompanyId).ToList();
            }
            else
            {
                general.CIG_Croups = _context.CIG_Croups.ToList();

            }


            general.co_Operatives = _context.co_Operatives.ToList();
            general.stations = _context.stations.ToList();
            general.locations = _context.Location.ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            return View(general);
        }
        public IActionResult CIGsGroupReportsPerSacco()
        {
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();
            general.locations = _context.Location.ToList();

            //general.CIG_Croups = _context.CIG_Croups.Where(k => k.strCompanyCode == strCompanyId).ToList();

            general.CIG_Croups = _context.CIG_Croups.ToList();




            general.co_Operatives = _context.co_Operatives.ToList();
            general.stations = _context.stations.ToList();
            general.locations = _context.Location.ToList();
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            return View(general);
        }
        public IActionResult SharesTransactionReportsPerMember()
        {
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();



            general.shareTransactions = _context.shareTransactions.ToList();
            general.ShareTypes = _context.shareTypes.ToList();

            general.banksSetups = _context.banksSetups.ToList();
            general.membersRegistrations = _context.MembersRegistrations.ToList();



            ViewBag.strMemberId = _context.MembersRegistrations.ToList();
            return View(general);
        }
        public IActionResult SharesTransactionReportsPerSharecode(string SharecodeId)
        {
            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();

            if (SharecodeId != null)
            {
                general.shareTransactions = _context.shareTransactions.Where(k => k.strShareCode == SharecodeId).ToList();
            }
            else
            {
                general.shareTransactions = _context.shareTransactions.ToList();

            }



            general.ShareTypes = _context.shareTypes.ToList();

            general.banksSetups = _context.banksSetups.ToList();
            general.membersRegistrations = _context.MembersRegistrations.ToList();



            ViewBag.strSharecodeId = _context.shareTypes.ToList();
            return View(general);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            utilities.SetUpPrivileges(this);
            ViewBag.total = 85;
            ViewBag.total1 = 50;
            return View();
        }
        public IActionResult LoginIPS()
        {
            return View(_context.loginIPs.ToList());
        }

        [HttpPost]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult GetMonthlyRegistrations()
        {
            var monthlyData = _context.MembersRegistrations
                .GroupBy(m => new { Year = m.strRegDate.Year, Month = m.strRegDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    MonthNumber = g.Key.Month, // Keep numeric month for ordering
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"), // Convert to month name
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.MonthNumber) // Order correctly
                .Select(g => new
                {
                    g.Year,
                    g.Month,
                    g.Count
                }) // Remove MonthNumber from response
                .ToList();

            return Json(monthlyData);
        }
        public IActionResult RegistrationFeeDetails()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var membersWithFee = (from m in _context.MembersRegistrations
                                  join t in _context.Customer_Balance
                                      on m.strMemberNo equals t.Memberno
                                  where t.Transdescription.ToLower().Contains("registration") && m.strCompanyCode == sacco
                                  select new RegistrationFeeViewModel
                                  {
                                      MemberNo = m.strMemberNo,
                                      SurName = m.strSurName,
                                      OtherName = m.strOtherName,
                                      PhoneNo = m.strPhoneNo,
                                      IdNo = m.strIdNo,
                                      Email = m.strEmail,
                                      Amount = t.Amount,
                                      TransDate = t.TransDate
                                  }).OrderBy(k => k.TransDate).ToList();

            ViewBag.TotalAmount = membersWithFee.Sum(x => x.Amount);
            return View(membersWithFee);
        }
        public IActionResult SavingDetails()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var membersWithFee = (from m in _context.MembersRegistrations
                                  join t in _context.Customer_Balance
                                      on m.strMemberNo equals t.Memberno
                                  where t.Transdescription.ToLower().Contains("saving") && m.strCompanyCode == sacco
                                  select new RegistrationFeeViewModel
                                  {
                                      MemberNo = m.strMemberNo,
                                      SurName = m.strSurName,
                                      OtherName = m.strOtherName,
                                      PhoneNo = m.strPhoneNo,
                                      IdNo = m.strIdNo,
                                      Email = m.strEmail,
                                      Amount = t.Amount,
                                      TransDate = t.TransDate
                                  }).OrderBy(k => k.TransDate).ToList();

            ViewBag.TotalAmount = membersWithFee.Sum(x => x.Amount);
            return View(membersWithFee);
        }
        public IActionResult TotalShareCapitalDetails()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var membersWithFee = (from m in _context.MembersRegistrations
                                  join t in _context.Customer_Balance
                                      on m.strMemberNo equals t.Memberno
                                  where t.Transdescription.ToLower().Contains("capital") && m.strCompanyCode == sacco
                                  select new RegistrationFeeViewModel
                                  {
                                      MemberNo = m.strMemberNo,
                                      SurName = m.strSurName,
                                      OtherName = m.strOtherName,
                                      PhoneNo = m.strPhoneNo,
                                      IdNo = m.strIdNo,
                                      Email = m.strEmail,
                                      Amount = t.Amount,
                                      TransDate = t.TransDate
                                  }).OrderBy(k => k.TransDate).ToList();

            ViewBag.TotalAmount = membersWithFee.Sum(x => x.Amount);
            return View(membersWithFee);
        }
        public IActionResult TotalMemberDetails()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var totalMembers = _context.MembersRegistrations
                .Where(m => m.strCompanyCode == sacco)
                .OrderBy(m => m.strRegDate)
                .Select(m => new MembersRegistration
                {
                    strMemberNo = m.strMemberNo,
                    strSurName = m.strSurName,
                    strOtherName = m.strOtherName,
                    strPhoneNo = m.strPhoneNo,
                    strIdNo = m.strIdNo,
                    strEmail = m.strEmail,
                    IsApproved = m.IsApproved,
                    HasPaidRegFee = m.HasPaidRegFee,
                    strRegDate = m.strRegDate
                })
                .ToList();

            ViewBag.TotalMembers = totalMembers.Count;
            return View(totalMembers);
        }
        public IActionResult TotalLoansTaken()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var TotalsLoansTakenn = (from m in _context.MembersRegistrations
                                  join t in _context.loanAplications
                                      on m.strMemberNo equals t.MemberNo
                                  where t.status==4 && m.strCompanyCode == sacco
                                  select new LoansAppViewModel
                                  {
                                      MemberNo = m.strMemberNo,
                                      SurName = m.strSurName,
                                      OtherName = m.strOtherName,
                                      PhoneNo = m.strPhoneNo,
                                      IdNo = m.strIdNo,
                                      Email = m.strEmail,
                                      LoanAmount =Convert.ToDecimal(t.DisburseAmount),
                                      LoanBalance =Convert.ToDecimal(t.LoanBalance),
                                      ApplicationDate = t.ApplicationDate
                                  }).OrderBy(k => k.ApplicationDate).ToList();

            ViewBag.TotalAmount = TotalsLoansTakenn.Sum(x => x.LoanAmount);
            return View(TotalsLoansTakenn);
        }
        public IActionResult TotalLoanBalances()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var TotalLoanBalancess = (from m in _context.MembersRegistrations
                                     join t in _context.loanAplications
                                         on m.strMemberNo equals t.MemberNo
                                     where t.status == 4 && m.strCompanyCode == sacco && t.LoanBalance>0
                                     select new LoansAppViewModel
                                     {
                                         MemberNo = m.strMemberNo,
                                         SurName = m.strSurName,
                                         OtherName = m.strOtherName,
                                         PhoneNo = m.strPhoneNo,
                                         IdNo = m.strIdNo,
                                         Email = m.strEmail,
                                         LoanAmount = Convert.ToDecimal(t.DisburseAmount),
                                         LoanBalance = Convert.ToDecimal(t.LoanBalance),
                                         ApplicationDate = t.ApplicationDate
                                     }).OrderBy(k => k.ApplicationDate).ToList();

            ViewBag.TotalAmount = TotalLoanBalancess.Sum(x => x.LoanBalance);
            return View(TotalLoanBalancess);
        }
        public IActionResult TotalLoansPaid()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");

            var TotalLoansPaidd = (from m in _context.MembersRegistrations
                                      join t in _context.loanAplications
                                          on m.strMemberNo equals t.MemberNo
                                      where t.status == 4 && m.strCompanyCode == sacco && t.DisburseAmount>t.LoanBalance
                                      select new LoansAppViewModel
                                      {
                                          MemberNo = m.strMemberNo,
                                          SurName = m.strSurName,
                                          OtherName = m.strOtherName,
                                          PhoneNo = m.strPhoneNo,
                                          IdNo = m.strIdNo,
                                          Email = m.strEmail,
                                          LoanAmount = Convert.ToDecimal(t.DisburseAmount),
                                          LoanBalance = Convert.ToDecimal(t.LoanBalance),
                                          ApplicationDate = t.ApplicationDate
                                      }).OrderBy(k => k.ApplicationDate).ToList();

            ViewBag.TotalAmount = TotalLoansPaidd.Sum(x => x.LoanAmount - x.LoanBalance);
            return View(TotalLoansPaidd);
        }
        [HttpGet]
        public IActionResult DashBoard()
        {
            decimal Sharecapital = 0;
            decimal Deposit = 0;
            decimal RegFee = 0;
            decimal loanbal = 0;
            int outstandingloan = 0;
            // decimal TotalLoans = 0;
            var auditid = HttpContext.Session.GetString("UserID");
            var sacco = HttpContext.Session.GetString("CompanyCode");
            if (!string.IsNullOrEmpty(auditid))
            {
                utilities.SetUpPrivileges(this);
            }
            if (string.IsNullOrEmpty(sacco))
            {
                ViewBag.TotalMembers = 0;
            }
            else
            {
                int totalMembers = _context.MembersRegistrations
                    .Where(m => m.strCompanyCode == sacco)
                    .Count();

                // Join Customer_Balance with MembersRegistrations
                Deposit = (from cb in _context.Customer_Balance
                           join mr in _context.MembersRegistrations
                           on cb.Memberno equals mr.strMemberNo
                           where mr.strCompanyCode == sacco &&
                                (cb.Transactioncode.ToUpper().Contains("D") ||
                                 cb.Transdescription.ToUpper().Contains("DEPOSIT") ||
                                 cb.Transdescription.ToUpper().Contains("SAVING"))
                           select cb.Amount
                          ).Sum();

                // Share Capital
                Sharecapital = (from cb in _context.Customer_Balance
                                join mr in _context.MembersRegistrations
                                on cb.Memberno equals mr.strMemberNo
                                where mr.strCompanyCode == sacco &&
                                      (cb.Transactioncode.ToUpper().Contains("SC") ||
                                       cb.Transdescription.ToUpper().Contains("SHARE CAPITAL") ||
                                       cb.Transdescription.ToUpper().Contains("CAPITAL"))
                                select cb.Amount
                               ).Sum();

                // Reg Fees
                RegFee = (from cb in _context.Customer_Balance
                          join mr in _context.MembersRegistrations
                          on cb.Memberno equals mr.strMemberNo
                          where mr.strCompanyCode == sacco &&
                                (cb.Transactioncode.ToUpper().Contains("RF") ||
                                 cb.Transdescription.ToUpper().Contains("FEE") ||
                                 cb.Transdescription.ToUpper().Contains("FEES") ||
                                 cb.Transdescription.ToUpper().Contains("REGISTRATION"))
                          select cb.Amount
                         ).Sum();

                // TotalLoansTaken
                decimal TotalLoans = (from loan in _context.loanAplications
                                      join member in _context.MembersRegistrations
                                          on loan.MemberNo equals member.strMemberNo
                                      where loan.DisburseAmount != null
                                            && loan.status == 4
                                      select loan.DisburseAmount ?? 0).Sum();


                // TotalLoansBalances
                decimal TotalLoansBalances = (from loan in _context.loanAplications
                                              join member in _context.MembersRegistrations
                                                  on loan.MemberNo equals member.strMemberNo
                                              where loan.LoanBalance != null
                                                  && loan.status == 4
                                                  && loan.LoanBalance > 0
                                              select loan.LoanBalance ?? 0).Sum();


                // TotalLoansPaid
                decimal TotalLoansPaid = TotalLoans - TotalLoansBalances;

                int TotalLoanees = _context.loanAplications
                 .Where(l => l.DisburseAmount != null && l.DisburseAmount > 0)
                 .Select(l => l.MemberNo)
                 .Distinct()
                 .Count();

                string formattedDeposit = Deposit.ToString("N0");
                string formattedSharecapital = Sharecapital.ToString("N0");
                string formattedRegFee = RegFee.ToString("N0");

                ViewBag.Savings = formattedDeposit;
                ViewBag.Capital = formattedSharecapital;
                ViewBag.FEE = formattedRegFee;
                ViewBag.TotalMembers = totalMembers;
                ViewBag.TotalLoansTaken = TotalLoans.ToString("N0");
                ViewBag.TotalLoanBalances = TotalLoansBalances.ToString("N0");
                ViewBag.TotalLoansPaid = TotalLoansPaid.ToString("N0");
                ViewBag.TotalLoannees = TotalLoanees;


                var regFeess = _context.Customer_Balance
                    .Where(t => t.Transdescription.ToLower().Contains("registration") && t.CompanyCode == sacco)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                var savingss = _context.Customer_Balance
                    .Where(t => t.Transdescription.ToLower().Contains("saving") && t.CompanyCode == sacco)
                    .Sum(t => (decimal?)t.Amount) ?? 0;
                var shareCapitals = _context.Customer_Balance
                    .Where(t => t.Transdescription.ToLower().Contains("capital") && t.CompanyCode == sacco)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                var LoansTaken= _context.loanAplications
                    .Where(t => t.status==4 && t.CompanyCode == sacco)
                    .Sum(t => (decimal?)t.DisburseAmount) ?? 0;
                
                var LoansBalances = _context.loanAplications
                    .Where(t => t.status == 4 && t.CompanyCode == sacco)
                    .Sum(t => (decimal?)t.LoanBalance) ?? 0;


                ViewBag.Feess = regFeess;
                ViewBag.Savingss = savingss;
                ViewBag.Capitals = shareCapitals;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string userPassword, string CaptchaCode)
        {
            // Define the time window and max allowed attempts
            var fifteenMinutesAgo = DateTime.UtcNow.AddMinutes(-15);
            int maxFailedAttempts = 5;

            var recentFails = await _context.LoginAttempts
                .Where(x => x.Username == username && x.Status == "Failed" && x.AttemptedAt >= fifteenMinutesAgo)
                .CountAsync();

            if (recentFails >= maxFailedAttempts)
            {
                TempData["Err"] = "Too many failed login attempts. Please try again later or contact support.";

                _logger.LogWarning($"Suspicious login activity: {recentFails} failed attempts for {username}");

                return Redirect("~/Home/Index");
            }
            var sessionCode = HttpContext.Session.GetString("CaptchaCode");
            if (sessionCode != CaptchaCode)
            {
                TempData["Error"] = "Invalid verification code.";
                return RedirectToAction("Login");
            }
            var ifExist = await _context.system_Users.FirstOrDefaultAsync(j => j.strUserName == username);
            if (ifExist != null && BCrypt.Net.BCrypt.Verify(userPassword, ifExist.strPassword))
            {
                var key = Encoding.ASCII.GetBytes("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                var JWToken = new JwtSecurityToken(
                    issuer: "",
                    audience: "",
                    claims: GetUserClaims(ifExist),
                    notBefore: new DateTimeOffset(DateTime.UtcNow.AddHours(3)).DateTime,
                    expires: new DateTimeOffset(DateTime.UtcNow.AddHours(3).AddDays(1)).DateTime,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );
                var JWTtoken = new JwtSecurityTokenHandler().WriteToken(JWToken);
                if (JWTtoken != null)
                {
                    var companyname = _context.co_Operatives.FirstOrDefault(k => k.strCompanyCode == ifExist.strCompanyId);
                    HttpContext.Session.SetString("CompanyName", companyname.strCompanyName);
                    HttpContext.Session.SetString("CompanyCode", companyname.strCompanyCode);
                    HttpContext.Session.SetString("UserID", ifExist.strFirstName);
                    HttpContext.Session.SetString("UserGroup", ifExist.strUserType);
                    HttpContext.Session.SetString("UserName", ifExist.strUserName);
                    HttpContext.Session.SetString("UserType", ifExist.strUserType);
                    if (ifExist.strUserType.ToUpper().Contains("MEMBER"))
                    {
                        HttpContext.Session.SetString("MemberNo", ifExist.MemberNo);
                    }
                    HttpContext.Session.SetString("JWToken", JWTtoken);
                    string hostName = Dns.GetHostName();
                    string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                    // ViewBag.MYIP = myIP;
                    //  return Json(User);
                    Username = ifExist.strFirstName;
                    // Guid guid = Guid.NewGuid();
                    // var newrecord = new LoginIPS
                    // {
                    //     strDate = DateTime.UtcNow.AddHours(3),
                    //     strId =Convert.ToInt32(guid),
                    //     strMachineIP = myIP,
                    //     strMachineName = hostName,
                    //     strName = ifExist.strFirstName,
                    //     strUserId = ifExist.strId.ToString(),

                    // };
                    // _context.Add(newrecord);
                    //await _context.SaveChangesAsync();

                    var session1 = new Session1
                    {
                        CompanyCode = companyname.strCompanyCode,
                        session_ID = username, // Generate a session ID
                        dDate = DateTime.Now,
                        dtime = DateTime.Now.TimeOfDay
                    };
                    await LogAudit("Login", $"User {username} successfully logged in");
                    _context.Session1.Add(session1);
                    await _context.SaveChangesAsync();
                    if (ifExist.strUserType.ToUpper().Contains("MEMBER"))
                    {
                        return Redirect("~/ShareTransaction/Shares_loansEnquiry");
                    }
                    else
                    {
                        return Redirect("~/Home/Dashboard");
                    }
                }
            }
            else
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

                var log = new LoginAttempt
                {
                    Username = username,
                    AttemptedAt = DateTime.UtcNow,
                    IPAddress = ip,
                    Status = "Failed"
                };
                await LogAudit("FailedLogin", $"Failed login attempt with username: {username}");
                _context.LoginAttempts.Add(log);
                await _context.SaveChangesAsync();

                _logger.LogWarning($"Failed login attempt for username: {username} at {DateTime.UtcNow}");
                TempData["Err"] = "Invalid User credentials";

                return Redirect("~/Home/Index");
            }
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GenerateCaptcha()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Avoid confusing chars like I, O, 1, 0
            var rand = new Random();
            string code = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rand.Next(s.Length)]).ToArray());

            HttpContext.Session.SetString("CaptchaCode", code);

            // Create CAPTCHA image
            using var bitmap = new Bitmap(150, 60);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            // Draw more visible zig-zag lines
            for (int i = 0; i < 15; i++)
            {
                int x1 = rand.Next(bitmap.Width);
                int y1 = rand.Next(bitmap.Height);
                int x2 = rand.Next(bitmap.Width);
                int y2 = rand.Next(bitmap.Height);
                var lineColor = Color.FromArgb(rand.Next(50, 150), rand.Next(50, 150), rand.Next(50, 150));
                using var pen = new Pen(lineColor, 2);
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }

            // Add strong noise dots
            for (int i = 0; i < 300; i++)
            {
                int x = rand.Next(bitmap.Width);
                int y = rand.Next(bitmap.Height);
                bitmap.SetPixel(x, y, Color.DarkGray);
            }

            // Draw CAPTCHA text
            var textColor = Color.FromArgb(rand.Next(0, 200), rand.Next(0, 100), rand.Next(0, 200));
            using var font = new Font("Arial", 28, FontStyle.Bold | FontStyle.Italic);
            using var brush = new SolidBrush(textColor);
            float xText = rand.Next(10, 30);
            float yText = rand.Next(5, 15);
            graphics.DrawString(code, font, brush, new PointF(xText, yText));

            // Output as PNG
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }


        private IEnumerable<Claim> GetUserClaims(System_Users user)
        {
            List<Claim> claims = new List<Claim>();
            Claim _claim;
            _claim = new Claim(ClaimTypes.Name, user.strId.ToString());
            claims.Add(_claim);
            _claim = new Claim(ClaimTypes.Role, user.strUserType);
            claims.Add(_claim);

            if (user.strUserType != "")
            {
                _claim = new Claim(user.strUserType, user.strUserType);
                claims.Add(_claim);
            }
            return claims.AsEnumerable<Claim>();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
