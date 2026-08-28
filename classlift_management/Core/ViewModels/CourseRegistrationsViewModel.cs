namespace Core.ViewModels
{
    public class CourseRegistrationsViewModel
    {
        public int CourseID { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public List<CourseRegisteredStudentViewModel> Students { get; set; } = new();
    }

    public class CourseRegisteredStudentViewModel
    {
        public int ChildID { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
