using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _repo;

    public SectionService(string connectionString)
    {
        _repo = new SectionRepository(connectionString);
    }
    
    public Result<List<Section>> GetStudentSchedule(int studentId)
    {
        try
        {
            var sections = _repo.GetSectionsByStudent(1);
            return ResultFactory.Success(sections);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Section>>(ex.Message);
        }
    }
}