using FourthWallAcademy.App.Services;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.Tests;

public class StudentServiceTests
{
    [Test]
    public void StudentService_GetStudent_ReturnsExpectedEntity()
    {
        var service = new StudentService("Server=localhost,1433;Database=FourthWallAcademy;User Id=sa;Password=SQLR0ck$;TrustServerCertificate=true;");
        var result = service.GetStudentById(1);

        var student = result.Data;
        
        Assert.That(student, Is.Not.Null);
        Assert.That(student.Alias, Is.EqualTo("Solar Flare"));
        Assert.That(student.StudentPowers[0].Power.PowerName, Is.EqualTo("Pyrokinesis"));
        Assert.That(student.StudentPowers[1].Power.PowerName, Is.EqualTo("Aerokinesis"));
        Assert.That(student.StudentWeaknesses[1].Weakness.WeaknessName, Is.EqualTo("Arrogance"));
        Assert.That(student.StudentWeaknesses[1].RiskLevel, Is.EqualTo(4));
    }
}