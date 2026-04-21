using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EASY_SACCO.Models
{
    public class NormalBalance
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int strId { get; set; }
        [Required]
        [DisplayName("Normal Balance")]
        public string strNormalBal { get; set; }
    }
}
