namespace Core.ViewModels;

public class ActivityRegistrationsViewModel
{
    public int ActivityID { get; set; }
    public string ActivityTitle { get; set; } = string.Empty;
    public List<ActivityRegisteredStudentViewModel> Students { get; set; } = new();
}

public class ActivityRegisteredStudentViewModel
{
    public int ChildID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
