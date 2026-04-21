using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class MembersRegistration
    {
        [Key]
        public int strId { get; set; }
        [Required]

        [DisplayName("Company Code")]
        public string? strCompanyCode { get; set; }

        [DisplayName("Member No")]
        public string? strMemberNo { get; set; }

        [DisplayName("Phone No")]
        [RegularExpression(@"^\+?(254|0)?(7|1)([0-9]{8})$", ErrorMessage = "Invalid phone number")]
        public string? strPhoneNo { get; set; }
        public string? MemberType { get; set; }

        [DisplayName("Marital Status")]
        public string? strMaritalstatus { get; set; }

        [DisplayName("Reg Date")]
        public DateTime strRegDate { get; set; }

        [DisplayName("SurName")]
        public string? strSurName { get; set; }

        [DisplayName("Gender")]
        public string? strGender { get; set; }

        [DisplayName("D.O.B")]
        // [MinimumAge(18, ErrorMessage = "Member must be at least 18 years old.")]
        public DateTime? strDOB { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email Address")]
        public string? strEmail { get; set; }
        public string? KraPin { get; set; }
        public string? ScannedIdPath { get; set; }
        public string? GroupCertPath { get; set; }
        public string? PassportPhotoPath { get; set; }

        [DisplayName("Status")]
        public int? strActiveStatus { get; set; }

        [DisplayName("Address")]

        public string? strAddress { get; set; }

        [DisplayName("CIG/Group")]
        public string? strCIGGroupId { get; set; }

        [DisplayName("County")]
        public string? strCountyId { get; set; }
        public string? GroupName { get; set; }
        public string? ChairmanName { get; set; }
        public string? TreasurerName { get; set; }
        public string? SecretaryName { get; set; }
        public string? ChairmanIDNO { get; set; }
        public string? TreasurerIDNO { get; set; }
        public string? SecretaryIDNO { get; set; }
        public string? ChairmanPhoneNo { get; set; }
        public string? TreasurerPhoneNo { get; set; }
        public string? SecretaryPhoneNo { get; set; }

        [DisplayName("Sub-County")]
        public string? strSubCountId { get; set; }

        [DisplayName("Ward")]
        public string? strWardId { get; set; }

        [DisplayName("Other Name")]
        public string? strOtherName { get; set; }

        [DisplayName("Id NO")]
        public string? strIdNo { get; set; }
        public string? strFullName { get; set; }

        [DisplayName("Village")]
        public string? strVillageId { get; set; }
        public string? RegistrationToken { get; set; }
        public string? photo { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        [NotMapped]
        public IFormFile? PassportPhotoFile { get; set; }
        [NotMapped]
        public IFormFile? GroupCertFile { get; set; }
        public bool IsApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? RejectionReason { get; set; }
        public bool HasPaidRegFee { get; set; }
        public string? AuditId { get; set; } = string.Empty;
        public DateTime? AuditTime { get; set; }
        public MembersRegistration()
        {
            AuditTime = DateTime.Now;

        }
    }
}
