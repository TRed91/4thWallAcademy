using FourthWallAcademy.App.Services;

namespace FourthWallAcademy.Tests.MockRepos;

public class SectionServiceTests
{
    [Test]
    public void GetSchedule_ConvertsSectionsToSchedule()
    {
        var service = new SectionService(new MockSectionRepo(new MockDB()));
        var scheduleResult = service.GetStudentSchedule(1, new DateTime(2024, 9, 1), new DateTime(2024, 9, 8));

        var schedule = scheduleResult.Data;
        
        Assert.That(scheduleResult.Message, Is.EqualTo(""));
        Assert.That(scheduleResult.Ok, Is.True);
        Assert.That(schedule.StudentAlias , Is.EqualTo("Solar Flare"));
        Assert.That(schedule.ScheduleDays.Count, Is.EqualTo(7));
        Assert.That(schedule.ScheduleDays[1].ScheduleCourses.Count, Is.EqualTo(3));
        Assert.That(schedule.ScheduleDays[1].ScheduleCourses[0].Course, Is.EqualTo("Course1"));
        Assert.That(schedule.ScheduleDays[1].ScheduleCourses[0].StartTime, Is.EqualTo(new TimeOnly(9, 0)));
    }
}