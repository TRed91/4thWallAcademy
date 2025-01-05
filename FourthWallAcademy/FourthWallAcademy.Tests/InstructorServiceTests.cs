using FourthWallAcademy.App.Services;

namespace FourthWallAcademy.Tests;

public class InstructorServiceTests
{
    [Test]
    public void GetInstructorByID_ShouldReturnInstructor()
    {
        var service =
            new InstructorService(
                "Server=localhost,1433;Database=FourthWallAcademy;User Id=sa;Password=SQLR0ck$;TrustServerCertificate=true;");
        var result = service.GetInstructorById(1);

        var i = result.Data;
        
        Assert.That(result.Message, Is.EqualTo(string.Empty));
        Assert.That(i.Alias, Is.EqualTo("Captain Quantum"));
        Assert.That(i.Sections.Count, Is.EqualTo(21));
        Assert.That(i.Sections[4].Course.CourseName, Is.EqualTo("Power Synergy"));
    }
}