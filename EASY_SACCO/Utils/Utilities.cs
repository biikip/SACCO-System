
using EASY_SACCO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using EASY_SACCO.Models;
using EASY_SACCO.Context;

namespace EASY_SACCO.Utils
{
    public class Utilities
    {
        private BOSA_DBContext _context;
        public Utilities(BOSA_DBContext context)
        {
            _context = context;
        }
        public void SetUpPrivileges(Controller controller)
        {
            var loggedInUser = controller.HttpContext.Session.GetString("UserName");
            var group = controller.HttpContext.Session.GetString("UserGroup") ?? "";
            var sacco = controller.HttpContext.Session.GetString("CompanyCode") ?? "";

            // Ensure group & sacco are not null
            if (string.IsNullOrEmpty(group) || string.IsNullOrEmpty(sacco))
            {
                controller.ViewBag.Error = "Invalid session data";
                return; // Exit if session data is incomplete
            }

            IQueryable<User_Group> usergroupslist = _context.user_Groups;

            var usergroup = usergroupslist
                .FirstOrDefault(u => u.strGroupName.ToUpper() == group.ToUpper()
                                  && u.CompanyCode.ToUpper() == sacco.ToUpper());


            if (usergroup == null)
            {
                controller.ViewBag.Error = "User group not found";
                return; // Exit if no matching group is found
            }

            var getuser = _context.system_Users.FirstOrDefault(x => x.strUserName == loggedInUser);

            if (getuser == null)
            {
                controller.ViewBag.Error = "User not found";
                return; // Exit if user is not found
            }

            // Assign user details
            controller.ViewBag.User = getuser.strFirstName;
            controller.ViewBag.group = getuser.strUserType;
            controller.ViewBag.Userid = getuser.strId;
            controller.ViewBag.loggedInUser = getuser.strUserName;
            controller.ViewBag.sacco = sacco;
            controller.ViewBag.sacconame = sacco.Length > 11 ? sacco.Substring(0, 11) + ".." : sacco;

            // Assign roles only if usergroup is valid
            controller.ViewBag.setuproles = usergroup?.Setup ?? false;
            controller.ViewBag.recordsrole = usergroup?.Records ?? false;
            controller.ViewBag.transactionsRole = usergroup?.Transactions ?? false;
            controller.ViewBag.creditmanagementrole = usergroup?.CreditManagement ?? false;
            controller.ViewBag.accountsrole = usergroup?.Accounting ?? false;
            controller.ViewBag.reportsRole = usergroup?.Reports ?? false;
            controller.ViewBag.enquiriesrole = usergroup?.Enquiries ?? false;
            controller.ViewBag.SystemUsersrole = usergroup?.SystemUsers ?? false;
            controller.ViewBag.LoanTypesrole = usergroup?.LoanTypes ?? false;
            controller.ViewBag.ShareTypesrole = usergroup?.ShareTypes ?? false;
            controller.ViewBag.CigRegistrationrole = usergroup?.CigRegistration ?? false;
            controller.ViewBag.BankSetuprole = usergroup?.BankSetup ?? false;
            controller.ViewBag.ChargesSetuprole = usergroup?.ChargesSetup ?? false;
            controller.ViewBag.MembershipRegistrationrole = usergroup?.MembershipRegistration ?? false;
            controller.ViewBag.SaccoRegistrationrole = usergroup?.SaccoRegistration ?? false;
            controller.ViewBag.RecruitmentAgentsrole = usergroup?.RecruitmentAgents ?? false;
            controller.ViewBag.Beneficiariesrole = usergroup?.Beneficiaries ?? false;
            controller.ViewBag.MemberWithdrawalrole = usergroup?.MemberWithdrawal ?? false;
            controller.ViewBag.Receiptingrole = usergroup?.Receipting ?? false;
            controller.ViewBag.JournalPostingrole = usergroup?.JournalPosting ?? false;
            controller.ViewBag.LoanApplicationsrole = usergroup?.LoanApplications ?? false;
            controller.ViewBag.Appraisalsrole = usergroup?.Appraisals ?? false;
            controller.ViewBag.Endorsementsrole = usergroup?.Endorsements ?? false;
            controller.ViewBag.LoanSchedulerole = usergroup?.LoanSchedule ?? false;
            controller.ViewBag.ReceiptPostingrole = usergroup?.ReceiptPosting ?? false;
            controller.ViewBag.ReprintMemberReceiptrole = usergroup?.ReprintMemberReceipt ?? false;
            controller.ViewBag.TransfersAndOffsettingsrole = usergroup?.TransfersAndOffsettings ?? false;
            controller.ViewBag.ShareTransferrole = usergroup?.ShareTransfer ?? false;
            controller.ViewBag.ShareToLoanOffsettingrole = usergroup?.ShareToLoanOffsetting ?? false;
            controller.ViewBag.DividendsProcessingrole = usergroup?.DividendsProcessing ?? false;
            controller.ViewBag.DividendsPaymentrole = usergroup?.DividendsPayment ?? false;
            controller.ViewBag.TransactionsManagementrole = usergroup?.TransactionsManagement ?? false;
            controller.ViewBag.AccountSetuprole = usergroup?.AccountSetup ?? false;
            controller.ViewBag.GLInquiryrole = usergroup?.GLInquiry ?? false;
            controller.ViewBag.BookOfAccountsrole = usergroup?.BookOfAccounts ?? false;
            controller.ViewBag.SharesInquiryrole = usergroup?.SharesInquiry ?? false;
            controller.ViewBag.LoansInquiryrole = usergroup?.LoansInquiry ?? false;
            controller.ViewBag.MembersPerSaccorole = usergroup?.MembersPerSacco ?? false;
            controller.ViewBag.CigsPerSaccorole = usergroup?.CigsPerSacco ?? false;
            controller.ViewBag.MembersPerCountyrole = usergroup?.MembersPerCounty ?? false;
            controller.ViewBag.ProjectAndInvestmentManagementrole = usergroup?.ProjectAndInvestmentManagement ?? false;
            controller.ViewBag.ProjectAndInvestmentsrole = usergroup?.ProjectAndInvestments ?? false;
            controller.ViewBag.FundDrivesrole = usergroup?.FundDrives ?? false;
            controller.ViewBag.SubscribeToProjectrole = usergroup?.SubscribeToProject ?? false;
        }


        //public string GenerateExcelGridSupReg(ISheet sheet, string scodes, string loggedInUser, string branch)
        //{
        //    try { 
        //    StringBuilder sb = new StringBuilder();
        //    IRow headerRow = sheet.GetRow(0); //Get Header Row
        //    int cellCount = headerRow.LastCellNum;
        //    sb.Append("<table class='table table-bordered'><tr>");
        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
        //        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
        //        sb.Append("<th>" + cell.ToString() + "</th>");
        //    }
        //    sb.AppendLine("</tr>");
        //    sb.Append("<tr>");
        //    var existingData = _context.ExcelDumpSupReg.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == scodes).ToList();
        //    if(existingData.Any())
        //        _context.ExcelDumpSupReg.RemoveRange(existingData);

        //    decimal totalQnty = 0;
        //    var excelDumps = new List<ExcelDumpSupReg>();
        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //    {
        //        IRow row = sheet.GetRow(i);
        //        if (row == null) continue;
        //        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
        //        for (int j = row.FirstCellNum; j < cellCount; j++)
        //        {
        //            if (row.GetCell(j) != null)
        //                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
        //        }
        //        // Reg_date, SNo, Names, PhoneNo, IdNo, DOB, Acc_Number, Bank_code, Bank_Branch,
        //        // Gender, PaymentMode, Village, LOCATION, WARD, SUB_COUNTY, COUNTY, LoggedInUser, Branch, SaccoCode
        //        decimal qnty = 0;
        //        decimal.TryParse(row.GetCell(2).ToString(), out qnty);
        //        totalQnty += qnty;
        //        String dateString = (row.GetCell(0)).ToString();
        //        DateTime transDate = DateTime.Parse(dateString);

        //        var DobdateString = row.GetCell(5).ToString();
        //        DateTime DobtransDate = DateTime.Parse(DobdateString);

        //        excelDumps.Add(new ExcelDumpSupReg
        //        {
        //            LoggedInUser = loggedInUser,
        //            SaccoCode = scodes,
        //            Branch = branch,
        //            Reg_date = transDate,
        //            SNo = row.GetCell(1)?.ToString() ?? "",
        //            Names = row.GetCell(2)?.ToString() ?? "",
        //            PhoneNo = row.GetCell(3)?.ToString() ?? "",
        //            IdNo = row.GetCell(4)?.ToString() ?? "",
        //            DOB = DobtransDate,
        //            Acc_Number = row.GetCell(6)?.ToString() ?? "",
        //            Bank_code = row.GetCell(7)?.ToString() ?? "",
        //            Bank_Branch = row.GetCell(8)?.ToString() ?? "",
        //            Gender = row.GetCell(9)?.ToString() ?? "",
        //            PaymentMode = row.GetCell(10)?.ToString() ?? "",
        //            Village = row.GetCell(11)?.ToString() ?? "",
        //            LOCATION = row.GetCell(12)?.ToString() ?? "",
        //            WARD = row.GetCell(13)?.ToString() ?? "",
        //            SUB_COUNTY = row.GetCell(14)?.ToString() ?? "",
        //            COUNTY = row.GetCell(15)?.ToString() ?? "",
        //            CIGName = row.GetCell(16)?.ToString() ?? "",
        //        });

        //        sb.AppendLine("</tr>");
        //    }

        //    if (excelDumps.Any())
        //    {
        //        _context.ExcelDumpSupReg.AddRange(excelDumps);
        //        _context.SaveChanges();
        //    }


        //    sb.Append("<tr>");
        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        if(j == 0)
        //            sb.Append("<td>Total</td>");
        //        else if(j == 2)
        //            sb.Append("<td>" + totalQnty + "</td>");
        //        else
        //            sb.Append("<td></td>");
        //    }

        //    sb.AppendLine("</tr>");
        //    sb.Append("</table>");

        //    return sb.ToString();
        //    }
        //    catch (Exception e)
        //    {
        //        return ("");
        //    }
        //}
        //public string GenerateExcelGrid(ISheet sheet, string sacco, string loggedInUser, string branch)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    IRow headerRow = sheet.GetRow(0); //Get Header Row
        //    int cellCount = headerRow.LastCellNum;
        //    sb.Append("<table class='table table-bordered'><tr>");
        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
        //        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
        //        sb.Append("<th>" + cell.ToString() + "</th>");
        //    }
        //    sb.AppendLine("</tr>");
        //    sb.Append("<tr>");
        //    var existingData = _context.ExcelDump.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
        //    if (existingData.Any())
        //        _context.ExcelDump.RemoveRange(existingData);

        //    decimal totalQnty = 0;
        //    var excelDumps = new List<ExcelDump>();
        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //    {
        //        IRow row = sheet.GetRow(i);
        //        if (row == null) continue;
        //        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
        //        for (int j = row.FirstCellNum; j < cellCount; j++)
        //        {
        //            if (row.GetCell(j) != null)
        //                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
        //        }

        //        decimal.TryParse(row.GetCell(2).ToString(), out decimal qnty);
        //        totalQnty += qnty;
        //        var dateString = row.GetCell(3).ToString();
        //        var transDate = DateTime.Parse(dateString);
        //        excelDumps.Add(new ExcelDump
        //        {
        //            LoggedInUser = loggedInUser,
        //            SaccoCode = sacco,
        //            Branch = branch,
        //            Sno = row.GetCell(0)?.ToString() ?? "",
        //            ProductType = row.GetCell(1)?.ToString() ?? "",
        //            Quantity = qnty,
        //            TransDate = transDate,
        //            TransCode = row.GetCell(4)?.ToString() ?? ""
        //        });

        //        sb.AppendLine("</tr>");
        //    }

        //    if (excelDumps.Any())
        //    {
        //        _context.ExcelDump.AddRange(excelDumps);
        //        _context.SaveChanges();
        //    }


        //    sb.Append("<tr>");
        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        if (j == 0)
        //            sb.Append("<td>Total</td>");
        //        else if (j == 2)
        //            sb.Append("<td>" + totalQnty + "</td>");
        //        else
        //            sb.Append("<td></td>");
        //    }

        //    sb.AppendLine("</tr>");
        //    sb.Append("</table>");

        //    return sb.ToString();
        //}
        //public decimal? GetBalance(ProductIntakeVm productIntake)
        //{
        //    var latestIntake = _context.ProductIntake.Where(i => i.Sno == productIntake.Sno && i.SaccoCode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()))
        //            .OrderByDescending(i => i.Id).FirstOrDefault();
        //    if (latestIntake == null)
        //        latestIntake = new ProductIntake();
        //    latestIntake.Balance = latestIntake?.Balance ?? 0;
        //    productIntake.DR = productIntake?.DR ?? 0;
        //    productIntake.CR = productIntake?.CR ?? 0;
        //    var balance = latestIntake.Balance + productIntake.CR - productIntake.DR;
        //    return balance;
        //}

        //public string GetLocalIPAddress()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (var ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            return ip.ToString();
        //        }
        //    }
        //    throw new Exception("No network adapters with an IPv4 address in the system!");
        //}

        //public string GenerateDeductionsExcelGrid(ISheet sheet, string sacco, string loggedInUser, string branch)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    IRow headerRow = sheet.GetRow(0); //Get Header Row
        //    int cellCount = headerRow.LastCellNum;
        //    sb.Append("<table class='table table-bordered'><tr>");
        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
        //        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
        //        sb.Append("<th>" + cell.ToString() + "</th>");
        //    }
        //    sb.AppendLine("</tr>");
        //    sb.Append("<tr>");

        //    var existingData = _context.ExcelDeductionDump.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
        //    if (existingData.Any())
        //        _context.ExcelDeductionDump.RemoveRange(existingData);

        //    decimal totalAmount = 0;
        //    var excelDumps = new List<ExcelDeductionDump>();
        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //    {
        //        IRow row = sheet.GetRow(i);
        //        if (row == null) continue;
        //        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
        //        for (int j = row.FirstCellNum; j < cellCount; j++)
        //        {
        //            if (row.GetCell(j) != null)
        //                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
        //        }

        //        decimal.TryParse(row.GetCell(2).ToString(), out decimal amount);
        //        totalAmount += amount;
        //        var dateString = row.GetCell(3).ToString();
        //        var transDate = DateTime.Parse(dateString);
        //        excelDumps.Add(new ExcelDeductionDump
        //        {
        //            Sno = row.GetCell(0)?.ToString() ?? "",
        //            ProductType = row.GetCell(1)?.ToString() ?? "",
        //            Amount = amount,
        //            TransDate = transDate,
        //            LoggedInUser = loggedInUser,
        //            SaccoCode = sacco,
        //            Branch = branch
        //        });

        //        sb.AppendLine("</tr>");
        //    }

        //    if (excelDumps.Any())
        //    {
        //        _context.ExcelDeductionDump.AddRange(excelDumps);
        //        _context.SaveChanges();
        //    }


        //    sb.Append("<tr>");
        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        if (j == 0)
        //            sb.Append("<td>Total</td>");
        //        else if (j == 2)
        //            sb.Append("<td>" + totalAmount + "</td>");
        //        else
        //            sb.Append("<td></td>");
        //    }

        //    sb.AppendLine("</tr>");
        //    sb.Append("</table>");

        //    return sb.ToString();
        //}

    }
}
