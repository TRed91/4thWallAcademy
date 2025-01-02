using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IInstructorRepository
{
    public Instructor? GetInstructorById(int id);
    public List<Instructor> GetInstructors();
    void AddInstructor(Instructor instructor);
    void UpdateInstructor(Instructor instructor);
    void DeleteInstructor(int id);
}