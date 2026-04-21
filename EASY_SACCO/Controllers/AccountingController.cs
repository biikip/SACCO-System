using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;using EASY_SACCO.Context;using EASY_SACCO.Models;using EASY_SACCO.Utils;using Microsoft.AspNetCore.Components;using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Mvc.Rendering;using Microsoft.EntityFrameworkCore;using OfficeOpenXml;
using Syncfusion.EJ2.Linq;using Syncfusion.EJ2.PivotView;using System;using System.Collections.Generic;using System.IO;using System.Linq;using System.Threading.Tasks;using static EASY_SACCO.ViewModels.AccountingVm;using static System.Net.WebRequestMethods;using DocumentFormat.OpenXml.Spreadsheet;namespace EASY_SACCO.Controllers{    public class AccountingController : BaseController    {        private readonly BOSA_DBContext _context;        private readonly INotyfService _notyf;        private Utilities utilities;        public class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }        public IEnumerable<AccountBooksOfAccounts> suppliersobj { get; set; }        public AccountingController(BOSA_DBContext context, INotyfService notyf) : base(context)        {            _context = context;            _notyf = notyf;            utilities = new Utilities(context);        }        private async Task<string> GenerateReceiptNo()
        {
            // Get the last receipt number that starts with "RCP-"
            var lastReceipt = await _context.ShareTransaction
                .Where(s => s.Docnumber.StartsWith("EXP-"))
                .OrderByDescending(s => s.Docnumber)
                .Select(s => s.Docnumber)
                .FirstOrDefaultAsync();

            int lastNumber = 0;

            if (!string.IsNullOrEmpty(lastReceipt))
            {
                // Extract the number part after the prefix "RCP-"
                var numberPart = lastReceipt.Replace("EXP-", "");
                int.TryParse(numberPart, out lastNumber);
            }

            int newNumber = lastNumber + 1;

            // Format with leading zeros to keep 5 digits
            string newReceipt = $"EXP-{newNumber.ToString("D5")}";

            return newReceipt;
        }        public async Task<IActionResult> JournalPosting()        {            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";            if (string.IsNullOrEmpty(loggedInUser))                return Redirect("~/");            utilities.SetUpPrivileges(this);            ViewBag.glAccounts = await _context.accountSetupGLs.Where(a => a.strCompanyCode == sacco).ToListAsync();
            //ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");

            return View();        }        [HttpPost]        public async Task<JsonResult> JournalPosting([FromBody] List<GL_Transaction> transactions)        {            try            {                var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";                var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";                utilities.SetUpPrivileges(this);                transactions.ForEach(t =>                {                    var getDr = _context.accountSetupGLs.FirstOrDefault(a => a.strAccountName == t.DrAccNo && a.strCompanyCode == sacco);                    var getCr = _context.accountSetupGLs.FirstOrDefault(a => a.strAccountName == t.CrAccNo && a.strCompanyCode == sacco);                    t.MemberNo = t.MemberNo ?? "0";                    t.Amount = t.Amount;                    t.CrAccNo = getCr.strAccountNo;                    t.DrAccNo = getDr.strAccountNo;                    t.TransactionDate = t.TransactionDate;                    t.AuditTime = DateTime.Now;                    t.AuditID = loggedInUser;                    t.Remarks = t?.Remarks ?? "";                    t.strDocnumber = t.strDocnumber ?? "";                    t.TransactionNo = $"{loggedInUser}{DateTime.Now}";                    t.CompanyCode = sacco;                });                await _context.GL_Transactions.AddRangeAsync(transactions);                await _context.SaveChangesAsync();
                //_notyf.Success("Journal posted successfully");
                return Json("");            }            catch (Exception e)            {                return Json("");            }        }        public async Task<IActionResult> JournalPostingExpenses()
        {
            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";
            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");

            utilities.SetUpPrivileges(this);

            // Load ONLY Expense Accounts for the dropdown
            ViewBag.expenseAccounts = await _context.accountSetupGLs
                .Where(a => a.strCompanyCode == sacco && a.strAccountGroup == "EXPENSES")
                .Select(a => new { a.strAccountNo, a.strAccountName })
                .ToListAsync();
            ViewBag.Code = await GenerateReceiptNo();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> JournalPostingExpenses([FromBody] GL_Transaction transaction)
        {
            try
            {
                var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";
                var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";
                utilities.SetUpPrivileges(this);

                // Get the Bank Account automatically
                var bankAccount = await _context.accountSetupGLs
                    .FirstOrDefaultAsync(a => a.strCompanyCode == sacco && a.strAccountName.Contains("Bank A/C"));

                if (bankAccount == null)
                    return Json(new { success = false, message = "Bank account not found!" });

                var expenseAccount = await _context.accountSetupGLs
                    .FirstOrDefaultAsync(a => a.strCompanyCode == sacco && a.strAccountName == transaction.DrAccNo);

                if (expenseAccount == null)
                    return Json(new { success = false, message = "Expense account not found!" });

                var newTransaction = new GL_Transaction
                {
                    TransactionDate = transaction.TransactionDate,
                    Amount = transaction.Amount,
                    DrAccNo = expenseAccount.strAccountNo,   // Debit Expense
                    CrAccNo = bankAccount.strAccountNo,      // Credit Bank
                    MemberNo = "0",
                    AuditID = loggedInUser,
                    AuditTime = DateTime.Now,
                    Remarks = transaction.Remarks ?? "",
                    strDocnumber = transaction.strDocnumber ?? "",
                    TransactionNo = $"{loggedInUser}{DateTime.Now:yyyyMMddHHmmssfff}",
                    CompanyCode = sacco
                };

                await _context.GL_Transactions.AddAsync(newTransaction);
                var transactionS = new ShareTransactionss
                {
                    MemberNo = transaction.MemberNo,
                    Docnumber = transaction.strDocnumber,
                    TransAmount = transaction.Amount,
                    TransDate = DateTime.Now,
                    TransDescription = transaction.Remarks.Trim(),
                };
                _context.ShareTransaction.Add(transactionS);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Journal posted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error posting journal." });
            }
        }        public async Task<IActionResult> GLInquiry()        {            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";            if (string.IsNullOrEmpty(loggedInUser))                return Redirect("~/");            utilities.SetUpPrivileges(this);            ViewBag.glAccounts = await _context.accountSetupGLs.Where(a => a.strCompanyCode == sacco).ToListAsync();            return View();        }        [HttpPost]
        public async Task<JsonResult> GLInquiry([FromBody] JournalFilter filter)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";
            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";
            utilities.SetUpPrivileges(this);

            var accno = await _context.accountSetupGLs.FirstOrDefaultAsync(v => v.strCompanyCode.Equals(sacco)
                && v.strAccountName == filter.AccNo);

            if (accno == null)
            {
                return Json(new { bookBalance = 0, journals = new List<JournalVm>() });
            }

            var accNo = accno.strAccountNo.Trim().ToUpper();
            var companyCode = sacco.ToUpper();

            var gltransactions = await _context.GL_Transactions
              .Where(t => t.CompanyCode == companyCode
               && t.TransactionDate >= filter.FromDate
               && t.TransactionDate <= filter.ToDate
               && (t.DrAccNo == accNo || t.CrAccNo == accNo))
               .ToListAsync();

            var journals = new List<JournalVm>();
            decimal bookBalance = 0;

            var isDebitNormal = accno.strNormalBal.Trim().ToLower() == "debit";

            // Detect if Bank or Cash account
            bool isBankAccount = accno.strAccountName.ToLower().Contains("bank a/c");

            // Prepare journals
            foreach (var txn in gltransactions)
            {
                bool isDebitTxn = txn.DrAccNo.Trim().ToUpper() == accNo;

                journals.Add(new JournalVm
                {
                    DocumentNo = isDebitTxn ? txn.TransactionNo : txn.strDocnumber,
                    TransDescript = txn.Remarks,
                    TransDate = txn.TransactionDate,
                    Dr = isDebitTxn ? txn.Amount : 0,
                    Cr = !isDebitTxn ? txn.Amount : 0,
                    MemberNo = txn.MemberNo
                });
            }

            // Order by date
            journals = journals.OrderBy(j => j.TransDate).ToList();

            // Calculate running balance properly
            decimal runningBalance = 0;
            foreach (var j in journals)
            {
                if (isBankAccount)
                {
                    // Bank Account logic (normal: deposit = increase)
                    runningBalance += j.Dr - j.Cr;
                }
                else
                {
                    // Other accounts: based on normal balance
                    runningBalance += isDebitNormal ? (j.Dr - j.Cr) : (j.Cr - j.Dr);
                }

                j.Bal = runningBalance;
            }

            // Calculate book balance (ending balance)
            bookBalance = runningBalance;

            return Json(new
            {
                bookBalance,
                journals
            });
        }
        public async Task<IActionResult> BooksOfAccounts()        {            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";            if (string.IsNullOrEmpty(loggedInUser))                return Redirect("~/");            utilities.SetUpPrivileges(this);            ViewBag.glAccounts = await _context.accountSetupGLs.Where(a => a.strCompanyCode == sacco).ToListAsync();
            //ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");

            return View();        }        [HttpPost]        [ValidateAntiForgeryToken]        public async Task<IActionResult> BooksOfAccounts(DateTime StartDate, DateTime EndDate)        {            try            {                var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";                var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";                var journalListings = new List<AccountBooksOfAccounts>();                utilities.SetUpPrivileges(this);                var gltransactions = await _context.GL_Transactions.Where(t => t.CompanyCode == sacco                && t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)                    .ToListAsync();                var AccTypeslist = _context.accountTypes.ToList();                var AccMainGrouplist = _context.accountGroups.ToList();                var AccGrouplist = _context.accountCategories.Where(v => v.Saccocode == sacco).ToList();                List<AccountSetupGL> glsetupsnotingls = await _context.accountSetupGLs.Where(g => g.strCompanyCode == sacco).ToListAsync();                glsetupsnotingls.ForEach(g =>                {                    decimal dr = 0;                    decimal cr = 0;                    dr = gltransactions.Where(b => b.DrAccNo == g.strAccountNo).Sum(c => c.Amount);                    cr = gltransactions.Where(b => b.CrAccNo == g.strAccountNo).Sum(c => c.Amount);                    int l1;                    int.TryParse(g.strAccountCategory, out l1);                    var getmno = AccGrouplist.FirstOrDefault(s => s.strId.ToString() == l1.ToString());                    if (dr > 0 || cr > 0)                    {                        _context.AccountBooksOfAccounts.Add(new AccountBooksOfAccounts                        {                            accno = g.strAccountNo,                            accname = g.strAccountName,                            amount = 0,
                            transtype = g.strNormalBal,
                            StartDate = StartDate,
                            EndDate = EndDate,                            auditid = loggedInUser,                            auditdatetime = DateTime.Now,
                            AccType = AccTypeslist.FirstOrDefault(f => f.strId.ToString() == g.strAccountType).strAccountType ?? "",                            GlAccMainGroup = AccMainGrouplist.FirstOrDefault(f => f.strId.ToString() == g.strAccountGroup).strAccoutGroup ?? "",                            AccGroup = getmno.strAcCategory,                            DR = dr,
                            CR = cr,
                            CompanyCode = sacco                        });                    }                });                await _context.SaveChangesAsync();                _notyf.Success("Generated successfully");                return View();            }            catch (Exception e)            {                _notyf.Error("Sorry, An Error Occured.");                return Json("");            }        }
        //public class TrialBalanceViewModel
        //{
        //    public string CompanyName { get; set; }
        //    public string CompanyAddress { get; set; }
        //    public string CompanyEmail { get; set; }
        //    public DateTime StartDate { get; set; }
        //    public DateTime EndDate { get; set; }
        //    public DateTime GeneratedDate { get; set; }
        //    public List<TrialBalanceItem> TrialBalanceItems { get; set; }
        //}

        //public class TrialBalanceItem
        //{
        //    public string AccNo { get; set; }
        //    public string AccName { get; set; }
        //    public string AccGroup { get; set; }
        //    public decimal Debit { get; set; }
        //    public decimal Credit { get; set; }
        //}

        //[HttpPost]
        //public IActionResult TrialBalance(DateTime StartDate, DateTime EndDate)
        //{
        //    var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";
        //    var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";

        //    utilities.SetUpPrivileges(this);

        //    // Step 1: Fetch all ledger accounts for the given company
        //    var accounts = _context.accountSetupGLs
        //        .Where(a => a.strCompanyCode == sacco)
        //        .Select(a => new
        //        {
        //            a.strAccountNo,
        //            a.strAccountName,
        //            a.strAccountGroup,
        //            a.strNormalBal // Credit or Debit
        //        }).ToList();

        //    // Step 2: Process each account to get Total DR and Total CR
        //    var trialBalanceList = new List<TrialBalanceItem>();

        //    foreach (var account in accounts)
        //    {
        //        var totalDR = _context.GL_Transactions
        //            .Where(t => t.CompanyCode == sacco && t.DrAccNo == account.strAccountNo &&
        //                        t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
        //            .Sum(t => (decimal?)t.Amount) ?? 0; // If null, default to 0

        //        var totalCR = _context.GL_Transactions
        //            .Where(t => t.CompanyCode == sacco && t.CrAccNo == account.strAccountNo &&
        //                        t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
        //            .Sum(t => (decimal?)t.Amount) ?? 0;

        //        decimal balance = 0;
        //        decimal debitAmount = 0;
        //        decimal creditAmount = 0;

        //        if (account.strNormalBal == "Credit")
        //        {
        //            balance = totalCR - totalDR;
        //        }
        //        else
        //        {
        //            balance = totalDR - totalCR;
        //        }

        //        // Ensure correct placement in Debit or Credit column
        //        if (balance >= 0)
        //        {
        //            debitAmount = (account.strNormalBal == "Debit") ? balance : 0;
        //            creditAmount = (account.strNormalBal == "Credit") ? balance : 0;
        //        }
        //        else
        //        {
        //            debitAmount = (account.strNormalBal == "Credit") ? Math.Abs(balance) : 0;
        //            creditAmount = (account.strNormalBal == "Debit") ? Math.Abs(balance) : 0;
        //        }

        //        // Only add accounts that have non-zero balances
        //        if (debitAmount != 0 || creditAmount != 0)
        //        {
        //            trialBalanceList.Add(new TrialBalanceItem
        //            {
        //                AccNo = account.strAccountNo,
        //                AccName = account.strAccountName,
        //                AccGroup = account.strAccountGroup,
        //                Debit = debitAmount,
        //                Credit = creditAmount
        //            });
        //        }
        //    }

        //    // Step 4: Debugging output
        //    Console.WriteLine("Computed Trial Balance:");
        //    foreach (var acc in trialBalanceList)
        //    {
        //        Console.WriteLine($"AccNo: {acc.AccNo}, Name: {acc.AccName}, Debit: {acc.Debit}, Credit: {acc.Credit}");
        //    }

        //    // Step 5: Generate Excel Report
        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Trial Balance");
        //        int currentRow = 1;

        //        // Fetch company details
        //        var company = _context.co_Operatives.FirstOrDefault(u => u.strCompanyCode == sacco);

        //        worksheet.Cell(currentRow++, 2).Value = company?.strCompanyName ?? "Company Name";
        //        worksheet.Cell(currentRow++, 2).Value = company?.strPostalAddress ?? "Company Address";
        //        worksheet.Cell(currentRow++, 2).Value = company?.strEmail ?? "Company Email";
        //        currentRow++;

        //        // Title
        //        worksheet.Cell(currentRow, 2).Value = "TRIAL BALANCE STATEMENT REPORT";
        //        worksheet.Cell(currentRow, 4).Value = EndDate.ToString("yyyy-MM-dd");
        //        worksheet.Cell(currentRow, 6).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        currentRow += 2;

        //        // Headers
        //        worksheet.Cell(currentRow, 1).Value = "AccNo";
        //        worksheet.Cell(currentRow, 2).Value = "Name";
        //        worksheet.Cell(currentRow, 3).Value = "Debit";
        //        worksheet.Cell(currentRow, 4).Value = "Credit";
        //        worksheet.Row(currentRow).Style.Font.Bold = true;
        //        currentRow++;

        //        // Insert Data
        //        decimal totalDebit = 0;
        //        decimal totalCredit = 0;

        //        foreach (var account in trialBalanceList)
        //        {
        //            worksheet.Cell(currentRow, 1).Value = account.AccNo;
        //            worksheet.Cell(currentRow, 2).Value = account.AccName;

        //            if (account.Debit > 0)
        //            {
        //                worksheet.Cell(currentRow, 3).Value = account.Debit;
        //                worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00"; // Debit
        //                worksheet.Cell(currentRow, 4).Value = ""; // No Credit
        //                totalDebit += account.Debit;
        //            }
        //            else
        //            {
        //                worksheet.Cell(currentRow, 3).Value = ""; // No Debit
        //                worksheet.Cell(currentRow, 4).Value = account.Credit;
        //                worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00"; // Credit
        //                totalCredit += account.Credit;
        //            }

        //            currentRow++;
        //        }

        //        // Total Amount Row
        //        worksheet.Cell(currentRow, 1).Value = "Total";
        //        worksheet.Cell(currentRow, 2).Value = "";
        //        worksheet.Cell(currentRow, 3).Value = totalDebit;
        //        worksheet.Cell(currentRow, 4).Value = totalCredit;
        //        worksheet.Row(currentRow).Style.Font.Bold = true;

        //        // Auto adjust column widths
        //        worksheet.Columns().AdjustToContents();

        //        // Return the Excel file
        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TrialBalance.xlsx");
        //        }
        //    }
        //}
        [HttpPost]
        public IActionResult TrialBalance(DateTime StartDate, DateTime EndDate)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";
            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";

            utilities.SetUpPrivileges(this);

            var accounts = _context.accountSetupGLs
                .Where(a => a.strCompanyCode == sacco)
                .Select(a => new
                {
                    a.strAccountNo,
                    a.strAccountName,
                    a.strAccountGroup,
                    a.strNormalBal
                }).ToList();

            var trialBalanceList = new List<TrialBalanceItem>();

            foreach (var account in accounts)
            {
                var totalDR = _context.GL_Transactions
                    .Where(t => t.CompanyCode == sacco && t.DrAccNo == account.strAccountNo &&
                                t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                var totalCR = _context.GL_Transactions
                    .Where(t => t.CompanyCode == sacco && t.CrAccNo == account.strAccountNo &&
                                t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                decimal balance = (account.strNormalBal == "Credit") ? (totalCR - totalDR) : (totalDR - totalCR);

                decimal debitAmount = balance >= 0 ? (account.strNormalBal == "Debit" ? balance : 0) : (account.strNormalBal == "Credit" ? Math.Abs(balance) : 0);
                decimal creditAmount = balance >= 0 ? (account.strNormalBal == "Credit" ? balance : 0) : (account.strNormalBal == "Debit" ? Math.Abs(balance) : 0);

                if (debitAmount != 0 || creditAmount != 0)
                {
                    trialBalanceList.Add(new TrialBalanceItem
                    {
                        AccNo = account.strAccountNo,
                        AccName = account.strAccountName,
                        AccGroup = account.strAccountGroup,
                        Debit = debitAmount,
                        Credit = creditAmount
                    });
                }
            }

            var company = _context.co_Operatives.FirstOrDefault(u => u.strCompanyCode == sacco);

            var reportModel = new TrialBalanceViewModel
            {
                CompanyName = company?.strCompanyName ?? "Company Name",
                CompanyAddress = company?.strPostalAddress ?? "Company Address",
                CompanyEmail = company?.strEmail ?? "Company Email",
                StartDate = StartDate,
                EndDate = EndDate,
                GeneratedDate = DateTime.Now,
                TrialBalanceItems = trialBalanceList
            };

            return View("TrialBalance", reportModel);
        }

        [HttpPost]
        public IActionResult IncomeStatement(DateTime StartDate, DateTime EndDate)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";

            utilities.SetUpPrivileges(this);

            var accounts = _context.accountSetupGLs
                .Where(a => a.strCompanyCode == sacco)
                .Select(a => new
                {
                    a.strAccountNo,
                    a.strAccountName,
                    a.strAccountGroup,
                    a.strNormalBal
                }).ToList();

            decimal totalRevenue = 0;
            decimal totalExpenses = 0;

            var incomeStatementItems = new List<IncomeStatementItem>();

            foreach (var account in accounts)
            {
                var totalDR = _context.GL_Transactions
                    .Where(t => t.CompanyCode == sacco && t.DrAccNo == account.strAccountNo &&
                                t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                var totalCR = _context.GL_Transactions
                    .Where(t => t.CompanyCode == sacco && t.CrAccNo == account.strAccountNo &&
                                t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                decimal balance = (account.strNormalBal == "Credit") ? (totalCR - totalDR) : (totalDR - totalCR);
                if (balance != 0)
                {
                    if (account.strAccountGroup == "INCOME")
                    {
                        totalRevenue += balance;
                        incomeStatementItems.Add(new IncomeStatementItem
                        {
                            AccountName = account.strAccountName,
                            Amount = balance,
                            Type = "INCOME"
                        });
                    }
                    else if (account.strAccountGroup == "EXPENSES")
                    {
                        totalExpenses += balance;
                        incomeStatementItems.Add(new IncomeStatementItem
                        {
                            AccountName = account.strAccountName,
                            Amount = balance,
                            Type = "Expense"
                        });
                    }
                }
            }

            decimal netIncome = totalRevenue - totalExpenses;

            var company = _context.co_Operatives.FirstOrDefault(u => u.strCompanyCode == sacco);

            var reportModel = new IncomeStatementViewModel
            {
                CompanyName = company?.strCompanyName ?? "Company Name",
                CompanyAddress = company?.strPostalAddress ?? "Company Address",
                CompanyEmail = company?.strEmail ?? "Company Email",
                StartDate = StartDate,
                EndDate = EndDate,
                GeneratedDate = DateTime.Now,
                IncomeStatementItems = incomeStatementItems,
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetIncome = netIncome
            };

            return View("IncomeStatement", reportModel);
        }

        [HttpPost]
        public IActionResult BalanceSheet(DateTime StartDate, DateTime EndDate)
        {
            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";
            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";

            utilities.SetUpPrivileges(this);

            // Fetch all accounts for the given company
            var accounts = _context.accountSetupGLs
                .Where(a => a.strCompanyCode == sacco)
                .Select(a => new
                {
                    a.strAccountNo,
                    a.strAccountName,
                    a.strAccountGroup,
                    a.strNormalBal // Credit or Debit
                }).ToList();

            var assetsList = new List<BalanceSheetItem>();
            var liabilitiesList = new List<BalanceSheetItem>();
            var equityList = new List<BalanceSheetItem>();

            foreach (var account in accounts)
            {
                var totalDR = _context.GL_Transactions
                    .Where(t => t.CompanyCode == sacco && t.DrAccNo == account.strAccountNo &&
                                t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                var totalCR = _context.GL_Transactions
                    .Where(t => t.CompanyCode == sacco && t.CrAccNo == account.strAccountNo &&
                                t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                decimal balance = (account.strNormalBal == "Credit") ? (totalCR - totalDR) : (totalDR - totalCR);
                if (balance != 0)
                {
                    var balanceSheetItem = new BalanceSheetItem
                    {
                        AccNo = account.strAccountNo,
                        AccName = account.strAccountName,
                        Balance = balance
                    };

                    // Categorize accounts into Assets, Liabilities, or Equity
                    if (account.strAccountGroup.Contains("ASSET"))
                    {
                        assetsList.Add(balanceSheetItem);
                    }
                    else if (account.strAccountGroup.Contains("LIABILITIES"))
                    {
                        liabilitiesList.Add(balanceSheetItem);
                    }
                    else if (account.strAccountGroup.Contains("EQUITY"))
                    {
                        equityList.Add(balanceSheetItem);
                    }
                }
            }

            // Calculate totals
            decimal totalAssets = assetsList.Sum(a => a.Balance);
            decimal totalLiabilities = liabilitiesList.Sum(l => l.Balance);
            decimal totalEquity = equityList.Sum(e => e.Balance);

            // Compute missing equity if no equity accounts exist
            if (totalEquity == 0)
            {
                totalEquity = totalAssets - totalLiabilities;
                equityList.Add(new BalanceSheetItem
                {
                    AccNo = "EQ-COMPUTED",
                    AccName = "Computed Equity",
                    Balance = totalEquity
                });
            }

            decimal totalLiabilitiesAndEquity = totalLiabilities + totalEquity;
            decimal netBalance = totalAssets - totalLiabilitiesAndEquity; // Should now be 0

            var company = _context.co_Operatives.FirstOrDefault(u => u.strCompanyCode == sacco);

            var reportModel = new BalanceSheetViewModel
            {
                CompanyName = company?.strCompanyName ?? "Company Name",
                CompanyAddress = company?.strPostalAddress ?? "Company Address",
                CompanyEmail = company?.strEmail ?? "Company Email",
                StartDate = StartDate,
                EndDate = EndDate,
                GeneratedDate = DateTime.Now,
                Assets = assetsList,
                Liabilities = liabilitiesList,
                Equity = equityList,
                TotalAssets = totalAssets,
                TotalLiabilities = totalLiabilities,
                TotalEquity = totalEquity,
                TotalLiabilitiesAndEquity = totalLiabilitiesAndEquity,
                NetBalance = netBalance
            };

            return View("BalanceSheet", reportModel);
        }


        public IActionResult JournalListing()        {            var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";            var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";            if (string.IsNullOrEmpty(loggedInUser))                return Redirect("~/");            utilities.SetUpPrivileges(this);            return View();        }        [HttpPost]        public async Task<JsonResult> JournalListing([FromBody] JournalFilter filter)        {            try            {                var sacco = HttpContext.Session.GetString("CompanyCode") ?? "";                var loggedInUser = HttpContext.Session.GetString("UserID") ?? "";                var journalListings = new List<JournalVm>();                var gltransactions = await _context.GL_Transactions.Where(t => t.CompanyCode == sacco                && t.TransactionDate >= filter.FromDate && t.TransactionDate <= filter.ToDate)                    .ToListAsync();                List<AccountSetupGL> glsetups = await _context.accountSetupGLs.Where(g => g.strCompanyCode == sacco).ToListAsync();                glsetups.ForEach(t =>                {                    t.strNormalBal = t?.strNormalBal ?? "";                    t.strOpeningBal = t?.strOpeningBal ?? "0";                    decimal dr = 0;                    decimal cr = 0;                    if (t.strNormalBal.ToUpper().Equals("DR") || t.strNormalBal.ToUpper() == "dr")                        dr = 0;                    if (t.strNormalBal.ToUpper().Equals("CR") || t.strNormalBal.ToUpper() == "cr")                        cr = 0;                    if (dr < 0)                    {                        cr = dr * -1;                        dr = 0;                    }                    if (cr < 0)                    {                        dr = cr * -1;                        cr = 0;                    }                    var debtorsAcc = gltransactions.FirstOrDefault(a => a.DrAccNo == t.strAccountNo);                    if (debtorsAcc != null)                    {                        var Amount = gltransactions.Where(a => a.DrAccNo == t.strAccountNo).Sum(n => n.Amount);                        if (Amount > 0)                        {                            journalListings.Add(new JournalVm                            {                                GlAcc = debtorsAcc.DrAccNo,                                TransDate = debtorsAcc.TransactionDate,                                AccName = t.strAccountName,                                AccCategory = t.strAccountCategory,                                Dr = Amount + dr + cr,                                DocumentNo = debtorsAcc.strDocnumber,                                Cr = 0,                                TransDescript = debtorsAcc.Remarks                            });                        }                    }                    var creditorsAcc = gltransactions.FirstOrDefault(a => a.CrAccNo == t.strAccountNo);                    if (creditorsAcc != null)                    {                        var Amount = gltransactions.Where(a => a.CrAccNo == t.strAccountNo).Sum(n => n.Amount);                        if (Amount > 0)                        {                            journalListings.Add(new JournalVm                            {                                GlAcc = creditorsAcc.CrAccNo,                                TransDate = creditorsAcc.TransactionDate,                                AccName = t.strAccountName,                                AccCategory = t.strAccountCategory,                                Cr = Amount + dr + cr,                                DocumentNo = creditorsAcc.strDocnumber,                                Dr = 0,                                TransDescript = creditorsAcc.Remarks                            });                        }                    }                });                var getdracc = gltransactions.Select(b => b.DrAccNo).Distinct().ToList();                var getcracc = gltransactions.Select(b => b.CrAccNo).Distinct().ToList();                List<AccountSetupGL> glsetupsnotingls = await _context.accountSetupGLs.Where(g => g.strCompanyCode == sacco && !getdracc.Contains(g.strAccountNo)                && !getcracc.Contains(g.strAccountNo)).ToListAsync();                glsetupsnotingls.ForEach(g =>                {                    g.strNormalBal = g?.strNormalBal ?? "";                    g.strOpeningBal = g?.strOpeningBal ?? "0";                    decimal dr = 0;                    decimal cr = 0;                    if (g.strNormalBal.ToUpper().Equals("DR") || g.strNormalBal.ToUpper() == "dr")                        dr = 0;                    if (g.strNormalBal.ToUpper().Equals("CR") || g.strNormalBal.ToUpper() == "cr")                        cr = 0;                    if (dr < 0)                    {                        cr = dr * -1;                        dr = 0;                    }                    if (cr < 0)                    {                        dr = cr * -1;                        cr = 0;                    }                    if (dr > 0 || cr > 0)                    {                        journalListings.Add(new JournalVm                        {                            GlAcc = g.strAccountNo,                            TransDate = filter.ToDate,                            AccName = g.strAccountName,                            AccCategory = g.strAccountCategory,                            Dr = dr,                            DocumentNo = "",                            Cr = cr,                            TransDescript = "Opening Bal"                        });                    }                });                var totalCr = journalListings.Sum(j => j.Cr);                var totalDr = journalListings.Sum(j => j.Dr);                journalListings.Add(new JournalVm                {                    GlAcc = "",                    AccCategory = "",                    AccName = "Total",                    Cr = totalCr,                    DocumentNo = "",                    Dr = totalDr,                    TransDescript = ""                });                return Json(journalListings);            }            catch (Exception e)            {                return Json("");            }        }    }}