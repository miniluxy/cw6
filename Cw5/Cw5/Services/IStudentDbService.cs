using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw5.DTOs.Requests;
using Cw5.DTOs.Responses;

namespace Cw5.Services
{
    public interface IStudentDbService
    {
        //void EnrollStudent(EnrollStudentRequest request);
        //void PromoteStudents(int semester, string studies);
        PromoteStudentResponse PromoteStudents(int semester, string studies);        
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        PromoteStudentResponse PromoteStudent(PromoteStudentRequest request);

    }
}
