using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IInstructorRepository
{
    Instructor? GetInstructorById(int id);
    Instructor? GetInstructorByAlias(string alias);
    List<Instructor> GetInstructors();
    void AddInstructor(Instructor instructor);
    void UpdateInstructor(Instructor instructor);
    void DeleteInstructor(int id);
}