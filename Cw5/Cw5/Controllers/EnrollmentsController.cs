using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cw5.DTOs.Requests;
using Cw5.DTOs.Responses;
using Cw5.Models;
using Cw5.Services;

namespace Cw5.Controllers
{
    [Route("api/enrollments")]
    [ApiController] //-> implicit model validation
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }


        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            _service.EnrollStudent(request);
            var response = new EnrollStudentResponse();    
            if (response == null)
                return NotFound("Wrong data was passed");
            else
                //return Ok(response);
                return StatusCode(201, response);
        }

        //..

        //..
        [HttpPost("{promotions}")]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            PromoteStudentResponse response = _service.PromoteStudent(request);
            if (response == null)
                return StatusCode(400, BadRequest("Wrong data"));
            else
                return StatusCode(201, response);
        }

    }
}