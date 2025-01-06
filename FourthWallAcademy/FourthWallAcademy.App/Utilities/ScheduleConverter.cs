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
        if (sections == null || sections.Count == 0)
        {
            return new StudentSchedule();
        }
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
            // Courses are empty on Weekends and holidays
            if (scheduleDay.Weekday == "Sunday" ||
                scheduleDay.Weekday == "Saturday" ||
                (scheduleDay.Date.Month == 12 && scheduleDay.Date.Day >= 25) ||
                (scheduleDay.Date.Month == 1 && scheduleDay.Date.Day == 1) ||
                scheduleDay.Date.Month == 7 ||
                scheduleDay.Date.Month == 8)
            {
                scheduleDay.ScheduleCourses = new List<ScheduleCourse>();
                for (int j = 0; j < 7; j++)
                {
                    scheduleDay.ScheduleCourses.Add(new ScheduleCourse
                    {
                        Course = "",
                        StartTime = new TimeOnly(9 + j, 0, 0),
                    });
                }
            }
            else
            {
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
            }
            schedule.ScheduleDays.Add(scheduleDay);
        }
        
        return schedule;
    }
}