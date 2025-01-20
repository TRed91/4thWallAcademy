using FourthWallAcademy.App.Services;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Tests.MockRepos;

namespace FourthWallAcademy.Tests;

public class StudentServiceTests
{
    [Test]
    public void GenerateStudentPasswordTest_FormatsAlias()
    {
        var service = new StudentService(new MockStudentRepo());
        var student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            Alias = "Dr. Tester",
            DoB = new DateTime(2000, 1, 1),
        };
        var result = service.AddStudent(student);
        
        Assert.That(result.Data.Password.Length, Is.EqualTo(14));
        Assert.That(result.Data.Password.Substring(0, 10), Is.EqualTo("!Dr.Tester"));
    }

    [Test]
    public void AddStudent_Fails_TooYoung()
    {
        var service = new StudentService(new MockStudentRepo());
        var student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            Alias = "Dr. Tester",
            DoB = DateTime.Today.AddYears(-10),
        };
        var result = service.AddStudent(student);
        
        Assert.That(result.Ok, Is.False);
        Assert.That(result.Message, Is.EqualTo("Student is too young"));
    }
}