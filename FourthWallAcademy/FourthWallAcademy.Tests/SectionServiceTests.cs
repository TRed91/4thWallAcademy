using FourthWallAcademy.App.Services;
using System.Text.Json;

namespace FourthWallAcademy.Tests;

public class SectionServiceTests
{
    [Test]
    public void ConvertsSectionsToSchedule()
    {
        var service =
            new SectionService("");
        var startDate = new DateTime(2022, 9, 6);
        var endDate = new DateTime(2022, 9, 13);
        
        var result = service.GetStudentSchedule(2, startDate, endDate);
        var schedule = result.Data;
        
        Assert.That(result.Message, Is.EqualTo(string.Empty));
        Assert.That(schedule.StudentAlias, Is.EqualTo("Thunder Bolt"));
        Assert.That(schedule.ScheduleDays.Count, Is.EqualTo(7));
        Assert.That(schedule.ScheduleDays[0].Weekday, Is.EqualTo("Tuesday"));
        Assert.That(schedule.ScheduleDays[1].Weekday, Is.EqualTo("Wednesday"));
        Assert.That(schedule.ScheduleDays[1].ScheduleCourses[0].StartTime.ToString(), Is.EqualTo("9:00 AM"));
        Assert.That(schedule.ScheduleDays[1].ScheduleCourses[0].Course, Is.EqualTo("Creative Writing"));
    }
}