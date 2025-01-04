using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.App.Utilities;

public class ScheduleConverter
{
    /// <summary>
    /// Converts a List of Sections to a Schedule Type
    /// </summary>
    /// <param name="sections"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static StudentSchedule ConvertToSchedule(List<Section> sections, DateTime startDate, DateTime endDate)
    {
        // get the number of days in the schedule
        int days = endDate.Subtract(startDate).Days;
        
        // populate the schedule
        StudentSchedule schedule = new StudentSchedule();
        schedule.StudentAlias = sections[0].StudentSections[0].Student.Alias;
        schedule.ScheduleDays = new List<ScheduleDay>();
        
        for (int i = 0; i < days; i++)
        {
            var scheduleDay = new ScheduleDay
            {
                Date = startDate.AddDays(i),
                Weekday = startDate.AddDays(i).DayOfWeek.ToString(),
                ScheduleCourses = new List<ScheduleCourse>()
            };
            foreach (var section in sections)
            {
                if (section.StartDate <= scheduleDay.Date && section.EndDate >= scheduleDay.Date)
                {
                    var scheduleCourse = new ScheduleCourse
                    {
                        Course = section.Course.CourseName,
                        StartTime = TimeOnly.FromTimeSpan(section.StartTime),
                    };
                    scheduleDay.ScheduleCourses.Add(scheduleCourse);
                }
            }
            schedule.ScheduleDays.Add(scheduleDay);
        }
        return schedule;
    }
}