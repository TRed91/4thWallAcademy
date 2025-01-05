using FourthWallAcademy.Core.Interfaces.Repositories;
using Serilog.Events;

namespace FourthWallAcademy.Core.Interfaces;

public interface IAppConfiguration
{
    ICourseRepository GetCourseRepository();
    IInstructorRepository GetInstructorRepository();
    ISectionRepository GetSectionRepository();
    IStudentRepository GetStudentRepository();
    IWeaknessRepository GetWeaknessRepository();
    IPowersRepository GetPowersRepository();
    LogEventLevel GetDbLogLevel();
}