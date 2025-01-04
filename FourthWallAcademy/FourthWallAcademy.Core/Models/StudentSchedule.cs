using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Models;

public class StudentSchedule
{
    public string StudentAlias { get; set; }
    public List<ScheduleDay> ScheduleDays { get; set; }
}

public class ScheduleDay
{
    public string Weekday { get; set; }
    public DateTime Date { get; set; }
    public List<ScheduleCourse> ScheduleCourses { get; set; }
}

public class ScheduleCourse
{ 
    public TimeOnly StartTime { get; set; }
    public string Course { get; set; }
}