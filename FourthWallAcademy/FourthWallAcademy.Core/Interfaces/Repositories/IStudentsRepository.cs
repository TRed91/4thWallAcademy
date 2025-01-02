using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IStudentRepository
{
    Student? GetStudentById(int id);
    List<Student> GetStudents();
    List<Student> GetStudentByStartingLetter(char letter);
    void AddStudent(Student student);
    void UpdateStudent(Student student);
    void DeleteStudent(int id);
}