using Core.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReportService
    {
        List<ChildReportDto> GetChildDetails(DateTime? from, DateTime? to);
        List<CoachReportDto> GetCoachDetails(DateTime? from, DateTime? to);
        List<CourseReportDto> GetCourseDetails(DateTime? from, DateTime? to);
    }
}
