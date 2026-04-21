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

namespace EASY_SACCO.Controllers
{
    public class Co_operativeController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public Co_operativeController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        // GET: Co_operative
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.user_Groups = _context.user_Groups.ToList();
            general.system_Users = _context.system_Users.ToList();
            general.locations = _context.Location.ToList();
            general.co_Operatives = _context.co_Operatives.ToList();
            general.CIG_Croups = _context.CIG_Croups.ToList();
            return View(general);
        }

        // GET: Co_operative/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var co_operative = await _context.co_Operatives
                .FirstOrDefaultAsync(m => m.strId == id);
            if (co_operative == null)
            {
                return NotFound();
            }

            return View(co_operative);
        }

        // GET: Co_operative/Create
        public async Task<IActionResult> Create()
        {
            utilities.SetUpPrivileges(this);

            Random r = new Random();
            var x = r.Next(55000000, 59000000);
            var y = r.Next(10, 99);
             var finalcode = "SACCO" + x + "-0" + y;
            var ifexist = await _context.co_Operatives.FirstOrDefaultAsync(k => k.strCompanyCode == finalcode);
            if (ifexist != null)
            {
               
                 finalcode = "SACCO" + x + "-0" + y;
            }
            else
            {
                 x = r.Next(55000000, 59000000);
                 y = r.Next(10, 99);
                finalcode = "SACCO" + x + "-0" + y;
            }
           
            ViewBag.Codee = finalcode;
       var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            ViewBag.strCounty = strCountry;
            ViewBag.strSubCounty = strSub_Counties;
            ViewBag.strWord = strWards;
            return View();
        }

        // POST: Co_operative/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strCountyId,strSubCountId,strWardId,strVillageId,strCompanyCode,strTelephoneNo,strNumberOfMemebrs,strEmail,strCompanyName,strContactperson,strBusinessSatus,strPostalAddress")] Co_operative co_operative)
        {
            utilities.SetUpPrivileges(this);

            Random r = new Random();
            var x = r.Next(55000000, 59000000);
            var y = r.Next(10, 99);
            ViewBag.Codee = "SACCO" + x + "-0" + y;

            var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            ViewBag.strCounty = strCountry;
            ViewBag.strSubCounty = strSub_Counties;
            ViewBag.strWord = strWards;

            if (ModelState.IsValid)
            {
                _context.Add(co_operative);
                await _context.SaveChangesAsync();
                Notify("Records Saved Successfully", notificationType: NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            return View(co_operative);
        }

        // GET: Co_operative/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);

            var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            ViewBag.strCounty = strCountry;
            ViewBag.strSubCounty = strSub_Counties;
            ViewBag.strWord = strWards;
            if (id == null)
            {
                return NotFound();
            }

            var co_operative = await _context.co_Operatives.FindAsync(id);
            if (co_operative == null)
            {
                return NotFound();
            }
            return View(co_operative);
        }

        // POST: Co_operative/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("strId,strCountyId,strSubCountId,strWardId,strVillageId,strCompanyCode,strTelephoneNo,strNumberOfMemebrs,strEmail,strCompanyName,strContactperson,strBusinessSatus,strPostalAddress")] Co_operative co_operative)
        {
            utilities.SetUpPrivileges(this);

            var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            ViewBag.strCounty = strCountry;
            ViewBag.strSubCounty = strSub_Counties;
            ViewBag.strWord = strWards;
            if (id != co_operative.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(co_operative);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Co_operativeExists(co_operative.strId))
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
            return View(co_operative);
        }

        // GET: Co_operative/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var co_operative = await _context.co_Operatives
                .FirstOrDefaultAsync(m => m.strId == id);
            if (co_operative == null)
            {
                return NotFound();
            }

            return View(co_operative);
        }

        // POST: Co_operative/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            utilities.SetUpPrivileges(this);

            var co_operative = await _context.co_Operatives.FindAsync(id);
            _context.co_Operatives.Remove(co_operative);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Co_operativeExists(int? id)
        {
            utilities.SetUpPrivileges(this);

            return _context.co_Operatives.Any(e => e.strId == id);
        }
        public class PdfData
        {
            public List<string> Headers { get; set; }
            public List<List<string>> Body { get; set; }
        }
        [HttpPost]
        //public IActionResult GeneratePDF([FromBody] PdfData pdfData)
        //{
        //    try
        //    {
        //        // Create a MemoryStream to store the PDF content
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            // Create a PdfWriter that writes to the MemoryStream
        //            using (iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(stream))
        //            {
        //                // Create a PdfDocument using the writer
        //                using (iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(writer))
        //                {
        //                    // Create a Document to add content
        //                    using (iText.Layout.Document document = new iText.Layout.Document(pdf))
        //                    {
        //                        // Set margins
        //                        document.SetMargins(36, 36, 72, 36); // Left, right, top, bottom margins

        //                        // Add table with headers and body data
        //                        iText.Layout.Element.Table table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(pdfData.Headers.Count)).UseAllAvailableWidth();

        //                        // Add header cells
        //                        foreach (var header in pdfData.Headers)
        //                        {
        //                            table.AddHeaderCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(header)).SetBold());
        //                        }

        //                        // Add body data
        //                        foreach (var row in pdfData.Body)
        //                        {
        //                            foreach (var cell in row)
        //                            {
        //                                table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(cell)));
        //                            }
        //                        }

        //                        document.Add(table);
        //                    }
        //                }
        //            }

        //            // Return the PDF content as a byte array
        //            var fileName = $"SACCOS_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";

        //            var fileUrl = $"~/exports/{fileName}"; // Store the file in your desired location
        //            var fileContent = stream.ToArray();
        //            System.IO.File.WriteAllBytes(Path.Combine("wwwroot", "exports", fileName), fileContent);

        //            // Return the file content with Content-Disposition header to open in a new browser tab
        //            // return File(fileContent, "application/pdf", fileName);
        //            return Json(new { fileUrl = Url.Content(fileUrl) });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error generating PDF: {ex.Message}");
        //    }
        //}
        [HttpPost]
        public IActionResult ExportToExcel([FromBody] string[][] tableData)
        {
            byte[] fileContents;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");
                // Set background color for header row
                var headerCells = worksheet.Cells["A1:" + GetExcelColumnName(tableData[0].Length) + "1"];
                var fill = headerCells.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(System.Drawing.Color.GreenYellow);
                for (int i = 0; i < tableData.Length; i++)
                {
                    for (int j = 0; j < tableData[i].Length; j++)
                    {
                        worksheet.Cells[i + 1, j + 1].Value = tableData[i][j].Trim();
                    }
                }

                fileContents = package.GetAsByteArray();
            }

            var fileName = $"SACCOS_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            var fileUrl = $"~/exports/{fileName}"; // Store the file in your desired location

            System.IO.File.WriteAllBytes(Path.Combine("wwwroot", "exports", fileName), fileContents);

            return Json(new { fileUrl = Url.Content(fileUrl) });
        }
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }
}
