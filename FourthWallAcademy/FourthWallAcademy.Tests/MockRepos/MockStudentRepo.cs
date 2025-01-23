using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Tests.MockRepos;

public class MockStudentRepo : IStudentRepository
{
    private readonly MockDB _db;
    private int _studentId;
    private int _passwordId;

    public MockStudentRepo(MockDB db)
    {
        _db = db;
        _studentId = _db.Students.Count + 1;
        _passwordId = _db.StudentPasswords.Count + 1;
    }
    public Student? GetStudentById(int id)
    {
        return _db.Students.FirstOrDefault(s => s.StudentID == id);
    }

    public Student? GetStudentByAlias(string alias)
    {
        return _db.Students.FirstOrDefault(s => s.Alias == alias);
    }

    public List<Student> GetStudents()
    {
        return _db.Students;
    }

    public List<Student> GetStudentByStartingLetter(char letter)
    {
        return _db.Students.Where(s => s.Alias.StartsWith(letter)).ToList();
    }

    public void AddStudentPower(StudentPower studentPower)
    {
        throw new NotImplementedException();
    }

    public void DeleteStudentPower(StudentPower studentPower)
    {
        throw new NotImplementedException();
    }

    public void AddStudentWeakness(StudentWeakness studentWeakness)
    {
        throw new NotImplementedException();
    }

    public void DeleteStudentWeakness(StudentWeakness studentWeakness)
    {
        throw new NotImplementedException();
    }

    public void AddStudent(Student student)
    {
        student.StudentID = _studentId++;
        _db.Students.Add(student);
    }

    public void UpdateStudent(Student student)
    {
        var oldStudent = _db.Students.FirstOrDefault(s => s.StudentID == student.StudentID);
        oldStudent.FirstName = student.FirstName;
        oldStudent.LastName = student.LastName;
        oldStudent.Alias = student.Alias;
        oldStudent.DoB = student.DoB;
    }

    public void DeleteStudent(int id)
    {
        _db.Students.Remove(_db.Students.FirstOrDefault(s => s.StudentID == id));
    }

    public void AddStudentPassword(StudentPassword studentPassword)
    {
        studentPassword.PasswordID = _passwordId++;
        _db.StudentPasswords.Add(studentPassword);
    }

    public void UpdateStudentPassword(StudentPassword studentPassword)
    {
        var oldStudentPassword = _db.StudentPasswords
            .FirstOrDefault(s => s.PasswordID == studentPassword.PasswordID);
        oldStudentPassword.Password = studentPassword.Password;
    }

    public GradesReport GetGradesReport(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}