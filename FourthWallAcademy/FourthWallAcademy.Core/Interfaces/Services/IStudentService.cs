using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface IStudentService
{
    Result<Student> GetStudentById(int id);
    Result<List<Student>> GetStudents();
    Result<List<Student>> GetStudentsByStartingLetter(char letter);
    Result AddStudentPower(StudentPower studentPower);
    Result DeleteStudentPower(StudentPower studentPower);
    Result AddStudentWeakness(StudentWeakness studentWeakness);
    Result DeleteStudentWeakness(StudentWeakness studentWeakness);
    Result AddStudent(Student student);
    Result UpdateStudent(Student student);
    Result DeleteStudent(int id);
}