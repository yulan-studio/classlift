using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;

namespace Web.Controllers.Report
{
    [Route("Report")]

    public class ReportController : Controller
    {
        private readonly ICourseEnrollmentService _courseEnrollmentService;

        public ReportController(ICourseEnrollmentService courseEnrollmentService)
        {
            _courseEnrollmentService = courseEnrollmentService;
        }


        [HttpGet("List")]
        public IActionResult List()
        {
            return View();
        }


        [HttpGet("GetTopStudents")]
        public JsonResult GetTopStudents()
        {
            var result = _courseEnrollmentService.GetTopStudents();
            return Json(result);
        }

        [HttpGet("GetCoursesByStudent")]
        public JsonResult GetCoursesByStudent(int childId)
        {
            var result = _courseEnrollmentService.GetCoursesByStudent(childId);
            return Json(result);
        }


    }
}
