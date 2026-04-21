using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class ProgrationProcessingOptions
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShareType { get; set; }
    }
}
