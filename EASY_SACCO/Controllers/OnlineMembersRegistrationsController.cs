using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using DinkToPdf.Contracts;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Syncfusion.EJ2.Linq;
using iText.Layout.Properties;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace EASY_SACCO.Controllers
{
    public class OnlineMembersRegistrationsController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConverter _converter;
        public OnlineMembersRegistrationsController(BOSA_DBContext context, INotyfService notyf, IConverter converter, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
           // utilities = new Utilities(context);
            _notyf = notyf;
            _converter = converter;
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.hostingEnvironment = hostingEnvironment;

        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CompleteRegistration(string token)
        {
            //utilities.SetUpPrivileges(this);

            // Retrieve member data using the token
            var memberData = _context.MEMBERS.FirstOrDefault(m => m.RegistrationToken == token);
            var RegistrationTokenExists = _context.MembersRegistrations.Any(i => (i.RegistrationToken == token));
            if (memberData == null)
            {
                // Handle the case where the token is invalid or data is not found
                Notify("Invalid or expired registration link.", notificationType: NotificationType.error);
                return RedirectToAction("Error", "Home");
            }
            if (RegistrationTokenExists)
            {
                ViewBag.ShouldClose = true;
                return View();
            }
            // Create a view model and populate it with the retrieved data
            var model = new MembersRegistration
            {
                strSurName = memberData.SurName,
                strOtherName = memberData.OtherNames,
                strPhoneNo = memberData.PhoneNo,
                strIdNo = memberData.IDNO,
                strEmail = memberData.Email,
                RegistrationToken = token
            };

            // Populate ViewBag properties as needed
            ViewBag.strFullName = _context.Agents.ToList();
            ViewBag.strGender = _context.Gender.ToList();
            ViewBag.strMaritalstatus = _context.Marital_Status.ToList();
            ViewBag.strCIGGroupId = new SelectList(_context.CIG_Croups, "strName", "strName").ToList();
            var strCountry = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            var strSub_Counties = _context.Location.Select(i => new { i.strSubCounty, i.strSubCountyId, i.strCountyId }).Distinct().ToList();
            var strWards = _context.Location.Select(i => new { i.strWard, i.strWardId, i.strSubCountyId }).Distinct();
            ViewBag.strCounty = strCountry;
            ViewBag.strSubCounty = strSub_Counties;
            ViewBag.strWord = strWards;
            ViewBag.strCompanyCode = _context.co_Operatives.ToList();
            ViewBag.strActiveStatus = _context.Active_Statuses.ToList();

            return View(model);
        }

        // POST: MembersRegistrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CompleteRegistration(MembersRegistration membersRegistration)
        {
            try
            {
               // utilities.SetUpPrivileges(this);

                var sacco = HttpContext.Session.GetString("CompanyCode");
                var auditid = HttpContext.Session.GetString("UserID");

                DateTime date = DateTime.UtcNow.AddHours(3);
                membersRegistration.strRegDate = date;
                ViewBag.strGender = _context.Gender.ToList();
                ViewBag.strFullName = _context.Agents.ToList();
                ViewBag.strMaritalstatus = _context.Marital_Status.ToList();
                ViewBag.strCIGGroupId = new SelectList(_context.CIG_Croups, "strName", "strName").ToList();
                ViewBag.strCounty = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
                ViewBag.strActiveStatus = _context.Active_Statuses.ToList();

                DateTime dobb =Convert.ToDateTime(membersRegistration.strDOB);
                DateTime now = DateTime.Today;
                int age = now.Year - dobb.Year;

                if (age <= 18)
                {
                    Notify("Sorry, a member cannot be under 18 years!", notificationType: NotificationType.error);
                    return View();
                }

                if (membersRegistration != null)
                {
                    if (membersRegistration.MemberType == "Individual")
                    {
                        string wwwRootPath = hostingEnvironment.WebRootPath;
                        string filename = Path.GetFileNameWithoutExtension(membersRegistration.ImageFile.FileName);
                        string extension = Path.GetExtension(membersRegistration.ImageFile.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Images/", filename);
                        membersRegistration.ScannedIdPath = "https://localhost:44367/Images/" + filename;
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await membersRegistration.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    if (membersRegistration.MemberType != "Individual")
                    {
                        string wwwRootPathh = hostingEnvironment.WebRootPath;
                        string filenames = Path.GetFileNameWithoutExtension(membersRegistration.GroupCertFile.FileName);
                        string extensions = Path.GetExtension(membersRegistration.GroupCertFile.FileName);
                        filenames = filenames + DateTime.Now.ToString("yymmssfff") + extensions;
                        string pathh = Path.Combine(wwwRootPathh + "/Images/", filenames);
                        membersRegistration.GroupCertPath = "https://localhost:44367/Images/" + filenames;
                        using (var fileStream = new FileStream(pathh, FileMode.Create))
                        {
                            await membersRegistration.GroupCertFile.CopyToAsync(fileStream);
                        }
                    }

                    if (membersRegistration.MemberType == "Individual")
                    {
                        string wwwRootPathhh = hostingEnvironment.WebRootPath;
                        string filenamess = Path.GetFileNameWithoutExtension(membersRegistration.PassportPhotoFile.FileName);
                        string extensionss = Path.GetExtension(membersRegistration.PassportPhotoFile.FileName);
                        filenamess = filenamess + DateTime.Now.ToString("yymmssfff") + extensionss;
                        string pathhh = Path.Combine(wwwRootPathhh + "/Images/", filenamess);
                        membersRegistration.PassportPhotoPath = "https://localhost:44367/Images/" + filenamess;
                        using (var fileStream = new FileStream(pathhh, FileMode.Create))
                        {
                            await membersRegistration.PassportPhotoFile.CopyToAsync(fileStream);
                        }
                    }
                    var newrecords = new MembersRegistration
                    {
                        strId = 0,
                       // strMemberNo = membersRegistration.strMemberNo,
                        strSurName = membersRegistration.strSurName,
                        strOtherName = membersRegistration.strOtherName,
                        strCompanyCode = sacco,
                        strPhoneNo = membersRegistration.strPhoneNo,
                        strAddress = membersRegistration.strAddress,
                        strDOB = membersRegistration.strDOB,
                        AuditId = auditid,
                        strVillageId = membersRegistration.strVillageId,
                        strIdNo = membersRegistration.strIdNo,
                        strEmail = membersRegistration.strEmail,
                        strRegDate = membersRegistration.strRegDate,
                        strCIGGroupId = membersRegistration.strCIGGroupId,
                        strActiveStatus = membersRegistration.strActiveStatus,
                        strCountyId = membersRegistration.strCountyId,
                        strSubCountId = membersRegistration.strSubCountId,
                        strWardId = membersRegistration.strWardId,
                        strGender = membersRegistration.strGender,
                        strMaritalstatus = membersRegistration.strMaritalstatus,
                        strFullName = membersRegistration.strFullName,
                        ScannedIdPath = membersRegistration.ScannedIdPath,
                        GroupCertPath = membersRegistration.GroupCertPath,
                        PassportPhotoPath = membersRegistration.PassportPhotoPath,
                        ChairmanName = membersRegistration.ChairmanName,
                        ChairmanIDNO = membersRegistration.ChairmanIDNO,
                        ChairmanPhoneNo = membersRegistration.ChairmanPhoneNo,
                        TreasurerName = membersRegistration.TreasurerName,
                        TreasurerIDNO = membersRegistration.TreasurerIDNO,
                        TreasurerPhoneNo = membersRegistration.TreasurerPhoneNo,
                        SecretaryName = membersRegistration.SecretaryName,
                        SecretaryIDNO = membersRegistration.SecretaryIDNO,
                        SecretaryPhoneNo = membersRegistration.SecretaryPhoneNo,
                        MemberType = membersRegistration.MemberType,
                        GroupName = membersRegistration.GroupName,
                        KraPin = membersRegistration.KraPin,
                        RegistrationToken = membersRegistration.RegistrationToken
                    };

                    _context.MembersRegistrations.Add(newrecords);
                    //Random r = new Random();
                    //var x = r.Next(1, 99);
                    //var memberno = "0001" + x;

                    //ViewBag.memberno = memberno;
                    await _context.SaveChangesAsync();
                    Notify("Records Saved Successfully", notificationType: NotificationType.success);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // You can log the exception here if needed
                Notify("An error occurred: " + ex.Message, notificationType: NotificationType.error);
                return View();
            }
        }
        [HttpGet]
        public JsonResult CheckUnique(string field, string value)
        {
            bool exists = false;

            switch (field)
            {
                case "Phone":
                    exists = _context.MembersRegistrations.Any(m => m.strPhoneNo == value);
                    break;
                case "ID":
                    exists = _context.MembersRegistrations.Any(m => m.strIdNo == value);
                    break;
                case "Email":
                    exists = _context.MembersRegistrations.Any(m => m.strEmail == value);
                    break;
                case "DOB":
                    if (DateTime.TryParse(value, out DateTime dob))
                    {
                        var minDate = DateTime.Today.AddYears(-18);
                        exists = dob > minDate; 
                    }
                    else
                    {
                        exists = true; 
                    }
                    break;
            }

            return Json(!exists); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BasicMemberRegistration(MembersRegistration membersRegistration)
        {
            utilities.SetUpPrivileges(this);
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the highlighted errors.";
                return View(membersRegistration);
            }

            var token = Guid.NewGuid().ToString();
            var registrationLink = Url.Action("CompleteRegistration", "MembersRegistrations", new { token = token }, protocol: HttpContext.Request.Scheme);

            var subject = "Complete Your Membership Registration";
            var body = $"Please complete your registration by clicking on the link: <a href='{registrationLink}'>Register Here</a>";

            await SendEmailAsync(membersRegistration.strEmail, subject, body);

            TempData["SuccessMessage"] = "Registration link sent successfully!";
            return RedirectToAction("BasicMemberRegistration");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("notifications@cargen.com", "lwqhgbyqbucukzei"),
                EnableSsl = true,
            })
            {
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
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Email Sending Failed: " + ex.Message);
                    throw;
                }
            }
        }

        // GET: MembersRegistrations/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strGender = _context.Gender.ToList();
            ViewBag.strStation = _context.stations.ToList();
            ViewBag.strMemberType = _context.Member_Type.ToList();
            ViewBag.strMaritalstatus = _context.Marital_Status.ToList();
            ViewBag.strCIGGroupId = _context.CIG_Croups.ToList();
            ViewBag.strAgenId = _context.Agents.ToList();
            ViewBag.strCounty = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            ViewBag.strCompanyCode = _context.co_Operatives.ToList();
            ViewBag.strActiveStatus = _context.Active_Statuses.ToList();
            
            if (id == null)
            {
                return NotFound();
            }
            var membersRegistration = await _context.MembersRegistrations.FindAsync(id);
            ViewBag.Idno = membersRegistration.strIdNo;
            if (membersRegistration != null && membersRegistration.ScannedIdPath != null)
            {
                ViewBag.photo = membersRegistration.ScannedIdPath;
                ViewBag.GroupName = membersRegistration.GroupName;
                
            }
            else
            {
                ViewBag.photo = "default-photo.jpg"; 
            }
            if (membersRegistration == null)
            {
                return NotFound();
            }
            return View(membersRegistration);
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
        //                      //  iText.Layout.Element.Table table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(pdfData.Headers.Count)).UseAllAvailableWidth();

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
        //            var fileName = $"Members_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
        //            var fileUrl = $"~/exports/{fileName}"; // Store the file in your desired location
        //            var fileContent = stream.ToArray();
        //            System.IO.File.WriteAllBytes(Path.Combine("wwwroot", "exports", fileName), fileContent);

        //            // Return the file content with Content-Disposition header to open in a new browser tab
        //           // return File(fileContent, "application/pdf", fileName);
        //            return Json(new { fileUrl = Url.Content(fileUrl) });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error generating PDF: {ex.Message}");
        //    }
        //}
        [HttpPost]
        public IActionResult ExportToExcel([FromBody]  string[][] tableData)
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

            var fileName = $"Members_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
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
        // POST: MembersRegistrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, MembersRegistration membersRegistration, IFormFile? ImageFile)
        {
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString("CompanyCode");
            membersRegistration.strCompanyCode = sacco;
            ViewBag.strGender = _context.Gender.ToList();
            ViewBag.strStation = _context.stations.ToList();
            ViewBag.strMemberType = _context.Member_Type.ToList();
            ViewBag.strMaritalstatus = _context.Marital_Status.ToList();
            ViewBag.strCIGGroupId = _context.CIG_Croups.ToList();
            ViewBag.strAgenId = _context.Agents.ToList();
            ViewBag.strCounty = _context.Location.Select(i => new { i.strCounty, i.strCountyId }).Distinct();
            ViewBag.strCompanyCode = _context.co_Operatives.ToList();
            ViewBag.strActiveStatus = _context.Active_Statuses.ToList();

            if (id != membersRegistration.strId)
            {
                return NotFound();
            }

            // Handle image upload
            if (ImageFile != null)
            {
                string wwwRootPath = hostingEnvironment.WebRootPath;
                string filename = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string extension = Path.GetExtension(ImageFile.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Images/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // Save new image URL
                membersRegistration.ScannedIdPath = "https://localhost:44367/Images/" + filename;
            }
            else
            {
                // Keep existing image path from DB
                var existing = await _context.MembersRegistrations
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(m => m.strId == id);
                membersRegistration.ScannedIdPath = existing?.ScannedIdPath;
            }
            ViewBag.photo = membersRegistration.ScannedIdPath;
            
            ModelState.Remove("strCompanyCode");
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.WriteLine($"Key: {modelState.Key}, Error: {error.ErrorMessage}");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membersRegistration);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembersRegistrationExists(membersRegistration.strId))
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
            return View(membersRegistration);
        }


        // GET: MembersRegistrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var membersRegistration = await _context.MembersRegistrations
                .FirstOrDefaultAsync(m => m.strId == id);
            if (membersRegistration == null)
            {
                return NotFound();
            }

            return View(membersRegistration);
        }

        // POST: MembersRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            utilities.SetUpPrivileges(this);

            var membersRegistration = await _context.MembersRegistrations.FindAsync(id);
            _context.MembersRegistrations.Remove(membersRegistration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool MembersRegistrationExists(int? id)
        {
            utilities.SetUpPrivileges(this);

            return _context.MembersRegistrations.Any(e => e.strId == id);
        }
    }
}
