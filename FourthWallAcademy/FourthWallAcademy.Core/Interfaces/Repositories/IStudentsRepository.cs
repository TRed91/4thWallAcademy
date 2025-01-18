using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IStudentRepository
{
    Student? GetStudentById(int id);
    Student? GetStudentByAlias(string alias);
    List<Student> GetStudents();
    List<Student> GetStudentByStartingLetter(char letter);
    void AddStudentPower(StudentPower studentPower);
    void DeleteStudentPower(StudentPower studentPower);
    void AddStudentWeakness(StudentWeakness studentWeakness);
    void DeleteStudentWeakness(StudentWeakness studentWeakness);
    void AddStudent(Student student);
    void UpdateStudent(Student student);
    void DeleteStudent(int id);
    GradesReport GetGradesReport(DateTime startDate, DateTime endDate);
}