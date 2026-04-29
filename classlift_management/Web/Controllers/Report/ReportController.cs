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
        //private readonly ICourseEnrollmentService _courseEnrollmentService;
        private readonly IReportService _reportService;

        public ReportController(IReportService reportervice)
        {
            _reportService = reportervice;
        }


        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet("List")]
        //public IActionResult List()
        //{
        //    return View();
        //}


        //[HttpGet("GetTopStudents")]
        //public JsonResult GetTopStudents()
        //{
        //    var result = _courseEnrollmentService.GetTopStudents();
        //    return Json(result);
        //}


        //[HttpGet("GetCoursesByStudent")]
        //public JsonResult GetCoursesByStudent(int childId)
        //{
        //    var result = _courseEnrollmentService.GetCoursesByStudent(childId);
        //    return Json(result);
        //}

        [HttpGet("GetChildDetails")]
        public IActionResult GetChildDetails(DateTime? from, DateTime? to)
        {
            var data = _reportService.GetChildDetails(from, to);
            return Json(data);
        }

        [HttpGet("GetCoachDetails")]
        public IActionResult GetCoachDetails(DateTime? from, DateTime? to)
        {
            var data = _reportService.GetCoachDetails(from, to);
            return Json(data);
        }

        [HttpGet("GetCourseStats")]
        public IActionResult GetCourseStats(DateTime? from, DateTime? to)
        {
            var data = _reportService.GetCourseDetails(from, to);
            return Json(data);
        }



    }
}
