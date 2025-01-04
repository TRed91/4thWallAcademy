using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface ISectionService
{
    Result<List<Section>> GetStudentSchedule(int studentId);
}