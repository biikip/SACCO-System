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
    public class AgentsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public AgentsController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            General general = new General();
            general.Agents = _context.Agents.ToList();
            general.stations = _context.stations.ToList();
            return View(general);
        }

        // GET: Agents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            utilities.SetUpPrivileges(this);

            var agents = await _context.Agents
                .FirstOrDefaultAsync(m => m.strId == id);
            if (agents == null)
            {
                return NotFound();
            }

            return View(agents);
        }

        // GET: Agents/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strGender = _context.Gender.ToList();
            ViewBag.strStation = _context.stations.ToList();

            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strFullName,strGender,strOccupation,strLandLineNo,strAddress,strstaffNo,strIDNO,strStation,strMobileno,strTown")] Agents agents)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strGender = _context.Gender.ToList();
            ViewBag.strStation = _context.stations.ToList();
            if (ModelState.IsValid)
            {
                _context.Add(agents);
                await _context.SaveChangesAsync();
                Notify("Records Saved Successfully", notificationType: NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            return View(agents);
        }

        // GET: Agents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strGender = _context.Gender.ToList();
            ViewBag.strStation = _context.stations.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var agents = await _context.Agents.FindAsync(id);
            if (agents == null)
            {
                return NotFound();
            }
            return View(agents);
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("strId,strFullName,strGender,strOccupation,strLandLineNo,strAddress,strstaffNo,strIDNO,strStation,strMobileno,strTown")] Agents agents)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strGender = _context.Gender.ToList();
            ViewBag.strStation = _context.stations.ToList();
            if (id != agents.strId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agents);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgentsExists(agents.strId))
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
            return View(agents);
        }

        // GET: Agents/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var agents = await _context.Agents
                .FirstOrDefaultAsync(m => m.strId == id);
            if (agents == null)
            {
                return NotFound();
            }

            return View(agents);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            utilities.SetUpPrivileges(this);

            var agents = await _context.Agents.FindAsync(id);
            _context.Agents.Remove(agents);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgentsExists(int id)
        {
            utilities.SetUpPrivileges(this);

            return _context.Agents.Any(e => e.strId == id);
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
        //            var fileName = $"RecruitmentAgents_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
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

            var fileName = $"RecruitmentAgents_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
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
