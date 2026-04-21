using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EASY_SACCO.Context;
using EASY_SACCO.Models;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Http;
using EASY_SACCO.Utils;

namespace EASY_SACCO.Controllers
{

   
    public class LoanTYpesController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public LoanTYpesController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }

        public class LoanTypeViewModel
        {
            public List<Charges> charges { get; set; }
            public int Id { get; set; }
            public string strLoanType { get; set; }
            public string strloancode { get; set; }

            public string strRPeriod { get; set; }


            public string CompanyCode { get; set; }

            public string strLoanAccount { get; set; }

            public string strInterestAccount { get; set; }

            public string strPenaltyAccount { get; set; }
            public decimal LoanToShareRatio { get; set; }           

            public string strRepaymentMethod { get; set; }

            public string strInterestRate { get; set; }

            public decimal strMaxloan { get; set; }


            public bool strAttractsPenalty { get; set; }

            public bool strCanitRefinance { get; set; }

            public bool strRequiresGuarantors { get; set; }

            public bool strGuaranteeownloan { get; set; }

            public string strRate { get; set; }

            public string strwhatToChange { get; set; }

            public string str { get; set; }
            public string AuditId { get; set; } 
            public decimal Amount { get; set; }
        }
        // GET: LoanTYpes
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            return View(await _context.loanTYpes.ToListAsync());
        }

        // GET: LoanTYpes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanTYpes = await _context.loanTYpes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanTYpes == null)
            {
                return NotFound();
            }

            return View(loanTYpes);
        }
        [HttpPost]
        public IActionResult populateChargcode(string Chargename)
        {
            utilities.SetUpPrivileges(this);

            if (Chargename != null)
            {


                var chargecode = _context.Charges_Setups.FirstOrDefault(k => k.ChargeName.Contains(Chargename));
                if (chargecode != null)
                {
                   
                    var myResult = new
                    {
                        code = (chargecode.ChargeCode).Trim(),
                        

                    };


                    var alldata = myResult.ToString().Replace("{", "").Trim().Replace("}", "").Trim();
                    var finalldata = alldata.ToString().Replace("code", "").Trim()
                        .Replace("=", "").Trim();


                    return Json(finalldata);
                }


            }

            return View();
        }

        [HttpPost]
        public JsonResult SaveOrder([FromBody] LoanTypeViewModel loanTypes)
        {
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString("CompanyCode");
            var auditid = HttpContext.Session.GetString("UserID");

            DateTime date = DateTime.UtcNow.AddHours(3);
            var loanacounNo = "";
            var interestacounNo = "";
            var penaltyacounNo = "";

            string result = "Error! Record Is Not Complete!";

            if (loanTypes != null)
            {
                loanacounNo = _context.accountSetupGLs
                    .Where(k => k.strAccountName.Contains(loanTypes.strLoanAccount))
                    .Select(k => k.strAccountNo)
                    .FirstOrDefault();

                interestacounNo = _context.accountSetupGLs
                    .Where(k => k.strAccountName.Contains(loanTypes.strInterestAccount))
                    .Select(k => k.strAccountNo)
                    .FirstOrDefault();

                penaltyacounNo = _context.accountSetupGLs
                    .Where(k => k.strAccountName.Contains(loanTypes.strPenaltyAccount))
                    .Select(k => k.strAccountNo)
                    .FirstOrDefault();

                var loantypess = new LoanTYpes
                {
                    Id = 0,
                    strLoanType = loanTypes.strLoanType,
                    strloancode = loanTypes.strloancode,
                    strRPeriod = Convert.ToInt32(loanTypes.strRPeriod),
                    CompanyCode = sacco,
                    strLoanAccount = loanacounNo,
                    strInterestAccount = interestacounNo,
                    strPenaltyAccount = penaltyacounNo,
                    strRepaymentMethod = loanTypes.strRepaymentMethod,
                    strInterestRate = loanTypes.strInterestRate,
                    AuditId = auditid,
                    strMaxloan = loanTypes.strMaxloan,
                    strAttractsPenalty = loanTypes.strAttractsPenalty,
                    strCanitRefinance = loanTypes.strCanitRefinance,
                    LoanToShareRatio = Convert.ToDecimal(loanTypes.LoanToShareRatio),
                    strGuaranteeownloan = loanTypes.strGuaranteeownloan,
                    strRequiresGuarantors = loanTypes.strRequiresGuarantors,
                    strRate = loanTypes.strRate,
                    strwhatToChange = loanTypes.strwhatToChange,
                    str = loanTypes.str,
                    Amount = loanTypes.Amount
                };

                _context.loanTYpes.Add(loantypess);
                _context.SaveChanges();

                Notify("Success! Record Is Complete!", notificationType: NotificationType.success);

                // Return an empty model to reset fields
                return Json(new
                {
                    success = true,
                    message = "Success! Record Is Complete!",
                    resetModel = new LoanTypeViewModel()
                });
            }

            return Json(new { success = false, message = result });
        }


        public class AccountInfo
        {
            public string AcountNo { get; set; }
            public string AcountDesc { get; set; }
        }


        // GET: LoanTYpes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            ViewBag.gl = new SelectList(_context.accountSetupGLs, "strAccountNo", "strAccountName").ToList();

            ViewBag.charges = new SelectList(_context.Charges_Setups, "ChargeName", "ChargeName").ToList();

            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            ViewBag.strRepaymentMethod = _context.RepaymentMethods.ToList();
            ViewBag.strRate = _context.Rates.ToList();
            ViewBag.strwhatToChange = _context.whatToChanges.ToList();
            List<AccountInfo> accountInfos = new List<AccountInfo>();
            accountInfos.Clear();
            var acccounts = _context.accountSetupGLs.ToList();
            if (acccounts != null)
            {
                foreach(var item in acccounts)
                {
                    var data = new AccountInfo
                    {
                        AcountDesc = item.strAccountName,
                        AcountNo = item.strAccountNo
                    };
                    accountInfos.Add(data);
                }
            }
           
            ViewBag.accountInfos = accountInfos;
            return View();
        }

        // POST: LoanTYpes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("strId,strloancode,strRPeriod,strMaxAmount,strNumsLan,strPriority,strCompanyId,strLoanAccount,strInterestAccount,strPenaltyAccount,strLoanType,strRepaymentMethod,strInterestRate,strMaxloan,strMDTI,strAttractsPenalty,strCanitRefinance,strRequiresGuarantors,strGuaranteeownloan,strRate,strwhatToChange,str,LoanToShareRatio")] LoanTYpes loanTYpes,Charges charges)
        {
            utilities.SetUpPrivileges(this);

            List<AccountInfo> accountInfos = new List<AccountInfo>();
            accountInfos.Clear();
            var acccounts = _context.accountSetupGLs.ToList();
            if (acccounts != null)
            {
                foreach (var item in acccounts)
                {
                    var data = new AccountInfo
                    {
                        AcountDesc = item.strAccountName,
                        AcountNo = item.strAccountNo
                    };
                    accountInfos.Add(data);
                }
            }

            ViewBag.accountInfos = accountInfos;
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            ViewBag.strRepaymentMethod = _context.RepaymentMethods.ToList();
            ViewBag.strRate = _context.Rates.ToList();
            ViewBag.strwhatToChange = _context.whatToChanges.ToList();

               
                var newdata = new LoanTYpes
                {

                    strloancode =loanTYpes.strloancode,
                    strRPeriod = loanTYpes.strRPeriod,
                  
                    CompanyCode = loanTYpes.CompanyCode,
                    strLoanAccount = loanTYpes.strLoanAccount,
                    strInterestAccount = loanTYpes.strInterestAccount,
                    strPenaltyAccount = loanTYpes.strPenaltyAccount,
                    strLoanType = loanTYpes.strLoanType,
                    strRepaymentMethod=loanTYpes.strRepaymentMethod,
                    strInterestRate=loanTYpes.strInterestRate,
                    strMaxloan= loanTYpes.strMaxloan,
                  
                    strAttractsPenalty= loanTYpes.strAttractsPenalty,
                    LoanToShareRatio = loanTYpes.LoanToShareRatio,

                    strCanitRefinance= loanTYpes.strCanitRefinance ,
                    strRequiresGuarantors=loanTYpes.strRequiresGuarantors,
                    strGuaranteeownloan=loanTYpes.strGuaranteeownloan,
                    strRate=loanTYpes.strRate,
                    strwhatToChange = loanTYpes.strwhatToChange ,
                    str = loanTYpes.str

                };
                _context.Add(newdata);
                await _context.SaveChangesAsync();               
                       
                        //var newassignedroads = new Charges
                        //{
                        //    strId = 0,
                        //    ProductCode = charges.ProductCode ,
                        //    ChargeCode = charges.ChargeCode,
                        //    Description = charges.Description,
                        //    Amount = charges.Amount,
                        //    Percentage = charges.Percentage,
                        //    GLAccount = charges.GLAccount,
                        //    Usepercentage = charges.Usepercentage,

                        //};
                        //_context.Add(newassignedroads);
                        //await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));           

         
        }
        // GET: LoanTYpes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            utilities.SetUpPrivileges(this);

            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            ViewBag.strRepaymentMethod = _context.RepaymentMethods.ToList();
            ViewBag.strRate = _context.Rates.ToList();
            ViewBag.strwhatToChange = _context.whatToChanges.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var loanTYpes = await _context.loanTYpes.FindAsync(id);
            if (loanTYpes == null)
            {
                return NotFound();
            }
            return View(loanTYpes);
        }

        // POST: LoanTYpes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, LoanTYpes loanTYpes)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString("CompanyCode");
            ViewBag.strCompanyId = _context.co_Operatives.ToList();
            ViewBag.strRepaymentMethod = _context.RepaymentMethods.ToList();
            ViewBag.strRate = _context.Rates.ToList();
            ViewBag.strwhatToChange = _context.whatToChanges.ToList();

            if (id == null || id != loanTYpes.Id)
            {
                return NotFound();
            }
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
                    loanTYpes.CompanyCode = sacco;
                    _context.Update(loanTYpes);
                    await _context.SaveChangesAsync();
                    Notify("Records Updated Successfully", notificationType: NotificationType.success);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanTYpesExists(loanTYpes.Id))
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

            return View(loanTYpes);
        }


        // GET: LoanTYpes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var loanTYpes = await _context.loanTYpes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanTYpes == null)
            {
                return NotFound();
            }

            return View(loanTYpes);
        }

        // POST: LoanTYpes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            utilities.SetUpPrivileges(this);

            if (id == null)
            {
                return NotFound();
            }

            var loanTypes = await _context.loanTYpes.FindAsync(id);

            if (loanTypes == null)
            {
                return NotFound();
            }
            bool loanTypesExist = _context.Customer_Balance
                .Any(k => k.Transactioncode == loanTypes.strloancode);
            if (loanTypesExist)
            {
                Notify("Sorry, the Loancode is already being used. It cannot be deleted!",
                       notificationType: NotificationType.error);
                return RedirectToAction(nameof(Index));
            }
            _context.loanTYpes.Remove(loanTypes);
            await _context.SaveChangesAsync();

            Notify("Loan Type deleted successfully!", notificationType: NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        private bool LoanTYpesExists(int? id)
        {
            utilities.SetUpPrivileges(this);

            return _context.loanTYpes.Any(e => e.Id == id);
        }
    }
}
