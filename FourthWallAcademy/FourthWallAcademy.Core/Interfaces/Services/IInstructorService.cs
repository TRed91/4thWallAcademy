using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface IInstructorService
{
    Result<Instructor> GetInstructorById(int id);
    Result<List<Instructor>> GetInstructors();
    Result AddInstructor(Instructor instructor);
    Result UpdateInstructor(Instructor instructor);
    Result DeleteInstructor(int id);
}