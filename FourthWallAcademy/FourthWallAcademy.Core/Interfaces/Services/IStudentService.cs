using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface IStudentService
{
    Result<Student> GetStudentById(int id);
    Result<List<Student>> GetStudents();
    Result<List<Student>> GetStudentsByStartingLetter(char letter);
    Result AddStudentPower(StudentPower studentPower);
    Result UpdateStudentPower(StudentPower studentPower);
    Result DeleteStudentPower(StudentPower studentPower);
    Result AddStudentWeakness(StudentWeakness studentWeakness);
    Result UpdateStudentWeakness(StudentWeakness studentWeakness);
    Result DeleteStudentWeakness(StudentWeakness studentWeakness);
    Result<StudentPassword> AddStudent(Student student);
    Result UpdateStudent(Student student);
    Result DeleteStudent(int id);
    Result<GradesReport> GetGradesReport(DateTime startDate, DateTime endDate);
}