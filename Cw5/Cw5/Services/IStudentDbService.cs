
using Cw5.DTOs.Requests;
using Cw5.DTOs.Responses;
using Cw5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.Services
{
    public interface IStudentsDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest req);
        PromoteStudentResponse PromoteStudent(PromoteStudentRequest req);
        bool IsStudentExists(String StudentIndexNumber);
    }
}