using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class Co_operative
    {
        [Key]
        
        public int strId { get; set; }
        [Required]
        [DisplayName("County")]
        public string strCountyId { get; set; }
        [Required]
        [DisplayName("Sub-County")]
        public string strSubCountId { get; set; }
        [Required]
        [DisplayName("Ward")]
        public string strWardId { get; set; }
        [Required]
        [DisplayName("Village")]
        public string strVillageId { get; set; }
        [Required]
        [DisplayName("Company Code")]
        public string strCompanyCode { get; set; }
        [Required]
        [DisplayName("Telephone No")]
        public string strTelephoneNo { get; set; }
        [Required]
        [DisplayName("Number Of Memebers")]
        public string strNumberOfMemebrs { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email Address")]
        public string strEmail { get; set; }
        [Required]
        [DisplayName("Company Name")]
        public string strCompanyName { get; set; }
        [Required]
        [DisplayName("Contact Person")]
        public string strContactperson { get; set; }
        [Required]
        [DisplayName("Business status")]
        public string strBusinessSatus { get; set; }
        [Required]
        [DisplayName("Postal Address")]
        public string strPostalAddress { get; set; }
    }
}
