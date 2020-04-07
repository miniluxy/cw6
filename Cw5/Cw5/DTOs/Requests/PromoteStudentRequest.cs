using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.DTOs.Requests
{
    public class PromoteStudentRequest
    {
        [Required(ErrorMessage = "Semester not specified")]
        public int Semester { get; set; }
        [Required(ErrorMessage = "Studies not specified")]
        [MaxLength(100)]
        public String Studies { get; set; }
    }
}