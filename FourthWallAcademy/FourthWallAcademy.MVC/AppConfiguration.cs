using FourthWallAcademy.Core.Interfaces;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Data.Repositories;
using Serilog.Events;

namespace FourthWallAcademy.MVC;

public class AppConfiguration : IAppConfiguration
{
    private readonly IConfiguration _config;

    public AppConfiguration(IConfiguration config)
    {
        _config = config;
    }

    public ICourseRepository GetCourseRepository()
    {
        return new CourseRepository(_config["ConnectionString"]);
    }

    public IInstructorRepository GetInstructorRepository()
    {
        return new InstructorRepository(_config["ConnectionString"]);
    }

    public ISectionRepository GetSectionRepository()
    {
        return new SectionRepository(_config["ConnectionString"]);
    }

    public IStudentRepository GetStudentRepository()
    {
        return new StudentRepository(_config["ConnectionString"]);
    }

    public IWeaknessRepository GetWeaknessRepository()
    {
        return new WeaknessRepository(_config["ConnectionString"]);
    }

    public IPowersRepository GetPowersRepository()
    {
        return new PowerRepository(_config["ConnectionString"]);
    }

    public LogEventLevel GetDbLogLevel()
    {
        LogEventLevel level;

        switch (_config["Logging:DbLogging:LogLevel"])
        {
            case "Debug": level = LogEventLevel.Debug; break;
            case "Information": level = LogEventLevel.Information; break;
            case "Warning": level = LogEventLevel.Warning; break;
            case "Error": level = LogEventLevel.Error; break;
            default: level = LogEventLevel.Warning; break;
        }
        
        return level;
    }
}