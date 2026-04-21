using EASY_SACCO.Context;
using EASY_SACCO.Models;
using EASY_SACCO.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Controllers
{
    public class Charges_Set_UpController : BaseController
    {
        private readonly BOSA_DBContext _context;
        private Utilities utilities;

        public Charges_Set_UpController(BOSA_DBContext context) : base(context)
        {
            _context = context;
            utilities = new Utilities(context);

        }
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);

            return View(await _context.Charges_Setups.ToListAsync());
        }
 
        public IActionResult SetUp()
        {
            utilities.SetUpPrivileges(this);
            ViewBag.gl = new SelectList(_context.accountSetupGLs, "strAccountNo", "strAccountName").ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SetUp(Charges_Setup charges_Setup)
        {
            utilities.SetUpPrivileges(this);

            if (charges_Setup != null)
            {
                _context.Add(charges_Setup);
                await _context.SaveChangesAsync();    

            }
            return View(charges_Setup);
        }
    }
}
