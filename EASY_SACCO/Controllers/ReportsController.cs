using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using Microsoft.AspNetCore.Hosting;
using FastReport.Web;
using System.IO;
using System.Data;


namespace EASY_SACCO.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly BOSA_DBContext _context;

        public ReportsController(IWebHostEnvironment hostingEnvironment, BOSA_DBContext context) : base(context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public IActionResult MemberList()
        {
            var webReport = new WebReport();
            webReport.Report.Dictionary.Connections.Clear();

            // Use hosting env to resolve path correctly
            var reportPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Reports", "MemberList.frx");
            webReport.Report.Load(reportPath);

            var members = _context.MembersRegistrations
                .Select(m => new
                {
                    m.strMemberNo,
                    m.strPhoneNo,
                    m.strRegDate,
                    m.strSurName,
                    m.strOtherName,
                    m.strGender,
                    m.strIdNo,
                    m.strDOB,
                    m.strEmail,
                    m.IsApproved,
                    m.KraPin
                }).ToList();

            DataTable table = new DataTable("MembersRegistrations");
            table.Columns.Add("strMemberNo", typeof(string));
            table.Columns.Add("strPhoneNo", typeof(string));
            table.Columns.Add("strRegDate", typeof(DateTime));
            table.Columns.Add("strSurName", typeof(string));
            table.Columns.Add("strOtherName", typeof(string));
            table.Columns.Add("strGender", typeof(string));
            table.Columns.Add("strIdNo", typeof(string));
            table.Columns.Add("strDOB", typeof(DateTime));
            table.Columns.Add("strEmail", typeof(string));
            table.Columns.Add("IsApproved", typeof(string));
            table.Columns.Add("KraPin", typeof(string));

            foreach (var m in members)
            {
                table.Rows.Add(
                    m.strMemberNo,
                    m.strPhoneNo,
                    m.strRegDate,
                    m.strSurName,
                    m.strOtherName,
                    m.strGender,
                    m.strIdNo,
                    m.strDOB ?? (object)DBNull.Value,
                    m.strEmail,
                    m.IsApproved,
                    m.KraPin
                );
            }

            var ds = new DataSet();
            ds.Tables.Add(table);

            webReport.Report.RegisterData(ds, "Data");
            webReport.Report.GetDataSource("MembersRegistrations").Enabled = true;

            return View(webReport);
        }

        public IActionResult LoansIssued()
        {
            try
            {
                var webReport = new WebReport();
                webReport.Report.Dictionary.Connections.Clear();

                var reportPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Reports", "LoansIssued.frx");
                if (!System.IO.File.Exists(reportPath))
                {
                    throw new Exception("Report file not found!");
                }

                webReport.Report.Load(reportPath);

                var loanData = (from l in _context.loanAplications
                                join m in _context.MembersRegistrations on l.MemberNo equals m.strMemberNo
                                join c in _context.co_Operatives on m.strCompanyCode equals c.strCompanyCode
                                where l.status == 4
                                select new
                                {
                                    Names = m.strSurName + " " + m.strOtherName,
                                    c.strCompanyName,
                                    l.MemberNo,
                                    l.LoanNo,
                                    l.AppliedAmount,
                                    l.ApplicationDate,
                                    l.RepaymentPeriod,
                                    l.ApprovedAmount,
                                    l.DateApproved,
                                    l.DisburseAmount,
                                    l.DisburseDate
                                }).Take(500).ToList(); // Optional limit

                DataTable table = new DataTable("LoansIssued");
                table.Columns.Add("Names", typeof(string));
                table.Columns.Add("strCompanyName", typeof(string));
                table.Columns.Add("MemberNo", typeof(string));
                table.Columns.Add("LoanNo", typeof(string));
                table.Columns.Add("AppliedAmount", typeof(decimal));
                table.Columns.Add("ApplicationDate", typeof(DateTime));
                table.Columns.Add("RepaymentPeriod", typeof(int));
                table.Columns.Add("ApprovedAmount", typeof(decimal));
                table.Columns.Add("DateApproved", typeof(DateTime));
                table.Columns.Add("DisburseAmount", typeof(decimal));
                table.Columns.Add("DisburseDate", typeof(DateTime));

                foreach (var item in loanData)
                {
                    table.Rows.Add(
                        item.Names,
                        item.strCompanyName,
                        item.MemberNo,
                        item.LoanNo,
                        item.AppliedAmount,
                        item.ApplicationDate.Date,
                        item.RepaymentPeriod,
                        item.ApprovedAmount,
                        item.DateApproved.Date,
                        item.DisburseAmount,
                        item.DisburseDate.Date
                    );
                }
                var ds = new DataSet();
                ds.Tables.Add(table);

                webReport.Report.RegisterData(ds, "Table");
                webReport.Report.GetDataSource("LoansIssued").Enabled = true;

                return View(webReport);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message} - {ex.StackTrace}");
            }
        }       
        public IActionResult Loan_Balances()
        {
            try
            {
                var webReport = new WebReport();
                webReport.Report.Dictionary.Connections.Clear();

                var reportPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Reports", "Loan_Balances.frx");
                if (!System.IO.File.Exists(reportPath))
                {
                    throw new Exception("Report file not found!");
                }

                webReport.Report.Load(reportPath);
                decimal amtpaid = 0;
                var loanData = (from l in _context.loanAplications
                                join m in _context.MembersRegistrations on l.MemberNo equals m.strMemberNo
                                join c in _context.co_Operatives on m.strCompanyCode equals c.strCompanyCode
                                where l.status == 4 && l.LoanBalance>=0
                                select new
                                { 
                                    Names = m.strSurName + " " + m.strOtherName,
                                    c.strCompanyName,
                                    l.MemberNo,
                                    l.LoanNo,
                                    l.DisburseAmount,
                                    l.LoanBalance
                                }).Take(500).ToList(); // Optional limit

                DataTable table = new DataTable("Loan_Balances");
                table.Columns.Add("Names", typeof(string));
                table.Columns.Add("strCompanyName", typeof(string));
                table.Columns.Add("MemberNo", typeof(string));
                table.Columns.Add("LoanNo", typeof(string));
                table.Columns.Add("LoanBalance", typeof(decimal));
                table.Columns.Add("DisburseAmount", typeof(decimal));

                foreach (var item in loanData)
                {
                    table.Rows.Add(
                        item.Names,
                        item.strCompanyName,
                        item.MemberNo,
                        item.LoanNo,
                        item.LoanBalance,
                        item.DisburseAmount
                    );
                }
                var ds = new DataSet();
                ds.Tables.Add(table);

                webReport.Report.RegisterData(ds, "Table");
                webReport.Report.GetDataSource("Loan_Balances").Enabled = true;

                return View(webReport);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message} - {ex.StackTrace}");
            }
        }

        public IActionResult ExportReport(string format)
        {
            //var webReport = new WebReport();
            //var reportPath = Path.Combine(_hostingEnvironment.WebRootPath, "reports", "MembershipReport.frx");
            //webReport.Report.Load(reportPath);

            //var members = _context.MembersRegistrations.ToList();
            //webReport.Report.RegisterData(members, "Members");

            //webReport.Prepare();

            //FastReport.Export.Base.ExportBase export;

            //if (format == "pdf")
            //{
            //    export = new FastReport.Export.PdfSimple.PDFSimpleExport();
            //}
            //else if (format == "excel")
            //{
            //    export = new FastReport.Export.OoXML.Excel2007Export();
            //}
            //else
            //{
            //    return BadRequest("Unsupported format");
            //}

            //var stream = new MemoryStream();
            //webReport.Report.Export(export, stream);
            //stream.Position = 0;

            //string contentType = (format == "pdf") ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //string fileName = (format == "pdf") ? "MembershipReport.pdf" : "MembershipReport.xlsx";

            return View();
        }
    }
}