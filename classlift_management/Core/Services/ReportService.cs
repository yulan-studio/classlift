using Core.DTOs.Report;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public List<ChildReportDto> GetChildDetails(DateTime? from, DateTime? to)
        {
            var data = _repo.GetCompletedEnrollments(from, to);

            return data
                .GroupBy(e => e.Child)
                .Select(g => new ChildReportDto
                {
                    ChildName = g.Key.Name,
                    TotalCourses = g.Select(x => x.CourseID).Distinct().Count(),

                    Courses = g.GroupBy(x => x.Course)
                        .Select(c => new ChildCourseDto
                        {
                            CourseName = c.Key.Title,
                            SessionsCompleted = c.Count(),

                            // 👇 NEW: last attended date
                            LastSessionDate = c.Max(x => x.ScheduledAt)
                        })
                        .OrderByDescending(c => c.LastSessionDate) // optional
                        .ToList()
                })
                .OrderByDescending(x => x.TotalCourses)
                .ToList();
        }

        public List<CoachReportDto> GetCoachDetails(DateTime? from, DateTime? to)
        {
            var data = _repo.GetCompletedEnrollments(from, to);

            return data
                .GroupBy(e => e.Course.Coach)
                .Select(g => new CoachReportDto
                {
                    CoachName = g.Key.Name,
                    TotalCourses = g.Select(x => x.CourseID).Distinct().Count(),

                    Courses = g.GroupBy(x => x.Course)
                        .Select(c => new CoachCourseDto
                        {
                            CourseName = c.Key.Title,
                            SessionsFinished = c.Count(),

                            // 👇 NEW: child + session count
                            Children = c.GroupBy(x => x.Child)
                                .Select(childGroup => new CoachCourseChildDto
                                {
                                    ChildName = childGroup.Key.Name,
                                    SessionsCompleted = childGroup.Count()
                                })
                                .OrderByDescending(x => x.SessionsCompleted) // optional
                                .ToList()
                        })
                        .OrderByDescending(x => x.SessionsFinished) // optional
                        .ToList()
                })
                .OrderByDescending(x => x.TotalCourses) // optional
                .ToList();
        }



        public List<CourseReportDto> GetCourseDetails(DateTime? from, DateTime? to)
        {
            var data = _repo.GetCompletedEnrollments(from, to);

            return data
                .GroupBy(e => e.Course)
                .Select(g => new CourseReportDto
                {
                    CourseName = g.Key.Title,
                    SessionsFinished = g.Count(),

                    Children = g.GroupBy(x => x.Child)
                        .Select(c => new CourseChildDto
                        {
                            ChildName = c.Key.Name,
                            SessionsCompleted = c.Count()
                        })
                        .OrderByDescending(c => c.SessionsCompleted) // optional
                        .ToList()
                })
                .OrderByDescending(x => x.SessionsFinished) // optional
                .ToList();
        }
    }
}
