using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class AccountSetupGL
    {
        [Key]
        public int strId { get; set; }
        [Required]
        [DisplayName("Accont No")]
        public string strAccountNo { get; set; }
        [Required]
        [DisplayName("Account Type")]
        public string strAccountType { get; set; }
        [Required]
        [DisplayName("Account Group")]
        public string strAccountGroup { get; set; }
        [ValidateNever]
        [DisplayName("Account SubGroup")]
        public string strAccountSubGroup { get; set; }
        [Required]
        [DisplayName("Currency")]
        public string strCurrency { get; set; }
        [Required]
        [DisplayName("Account Category")]
        public string strAccountCategory { get; set; }
        [Required]
        [DisplayName("Account Name")]
        public string strAccountName { get; set; }
        [ValidateNever]
        [DisplayName("Account SubCategory")]
        public string strAccountSubcategory { get; set; }
        [Required]
        [DisplayName("Normal Balance")]
        public string strNormalBal { get; set; }
        [ValidateNever]
        [DisplayName("Account Typename")]
        public string strAccounttypename { get; set; }
        [Required]
        [DisplayName("Balance As At")]
        public DateTime strBalance { get; set; }
        [DisplayName("Opening Balance")]
        public string strOpeningBal { get; set; }
        [DisplayName("Company Code")]
        public string strCompanyCode { get; set; }
        [Required]
        [DisplayName("Is Suspense Account")]
        public bool strIssuspenseAccount { get; set; }
        [Required]
        [DisplayName("Is Retained Earning Account")]
        public bool strIsRetainedEarningAccount { get; set; }

    }
}
