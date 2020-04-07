using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.DTOs.Responses
{
    public class PromoteStudentResponse
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public String Study { get; set; }
        public DateTime StartDate { get; set; }
    }
}