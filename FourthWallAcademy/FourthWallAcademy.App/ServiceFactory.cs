using FourthWallAcademy.App.Services;
using FourthWallAcademy.Core.Interfaces;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;

namespace FourthWallAcademy.App;

public class ServiceFactory
{
    private readonly IAppConfiguration _appConfig;

    public ServiceFactory(IAppConfiguration appConfig)
    {
        _appConfig = appConfig;
    }

    public ISectionService CreateSectionService()
    {
        return new SectionService(_appConfig.GetSectionRepository());
    }

    public IStudentService CreateStudentService()
    {
        return new StudentService(_appConfig.GetStudentRepository());
    }

    public ICourseService CreateCourseService()
    {
        return new CourseService(_appConfig.GetCourseRepository());
    }

    public IInstructorService CreateInstructorService()
    {
        return new InstructorService(_appConfig.GetInstructorRepository());
    }

    public IPowerService CreatePowerService()
    {
        return new PowerService(_appConfig.GetPowersRepository());
    }

    public IWeaknessService CreateWeaknessService()
    {
        return new WeaknessService(_appConfig.GetWeaknessRepository());
    }
}