using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core;
using Web.Filters;

namespace Web.Controllers.Report
{
    [Route("Report")]
    [Authorize(Roles = "Admin")]
    [RequiresFeature(FeatureCodes.StandardReporting)]
    public class ReportController : Controller
    {
        //private readonly ICourseEnrollmentService _courseEnrollmentService;
        private readonly IReportService _reportService;
        private readonly ITimeZoneService _timeZoneService;

        public ReportController(IReportService reportervice, ITimeZoneService timeZoneService)
        {
            _reportService = reportervice;
            _timeZoneService = timeZoneService;
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
            var (fromUtc, toUtc) = ConvertRangeToUtc(from, to);
            var data = _reportService.GetChildDetails(fromUtc, toUtc);
            return Json(data);
        }

        [HttpGet("GetCoachDetails")]
        public IActionResult GetCoachDetails(DateTime? from, DateTime? to)
        {
            var (fromUtc, toUtc) = ConvertRangeToUtc(from, to);
            var data = _reportService.GetCoachDetails(fromUtc, toUtc);
            return Json(data);
        }

        [HttpGet("GetCourseDetails")]
        public IActionResult GetCourseDetails(DateTime? from, DateTime? to)
        {
            var (fromUtc, toUtc) = ConvertRangeToUtc(from, to);
            var data = _reportService.GetCourseDetails(fromUtc, toUtc);
            return Json(data);
        }

        private (DateTime? FromUtc, DateTime? ToUtc) ConvertRangeToUtc(DateTime? from, DateTime? to)
        {
            var zoneId = User.GetTimeZoneId();
            return (
                from.HasValue ? _timeZoneService.ConvertLocalToUtc(from.Value, zoneId) : null,
                to.HasValue ? _timeZoneService.ConvertLocalToUtc(to.Value, zoneId) : null);
        }



    }
}
