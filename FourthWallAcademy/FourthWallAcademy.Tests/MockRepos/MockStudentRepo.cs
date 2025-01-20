using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Tests.MockRepos;

public class MockStudentRepo : IStudentRepository
{
    private List<Student> _students;
    private List<StudentPassword> _studentPasswords;

    private int _studentId;
    private int _passwordId;

    public MockStudentRepo()
    {
        _students = new List<Student>
        {
            new Student
            {
                StudentID = 1, FirstName = "Emily", LastName = "Smith", Alias = "Solar Flare",
                DoB = new DateTime(2004, 03, 12)
            },
            new Student
            {
                StudentID = 2, FirstName = "Oliver", LastName = "Johnson", Alias = "Thunder Bolt",
                DoB = new DateTime(2005, 10, 22)
            },
            new Student
            {
                StudentID = 3, FirstName = "Sophia", LastName = "Williams", Alias = "Astral Whisper",
                DoB = new DateTime(2006, 06, 30)
            }
        };
        _studentPasswords = new List<StudentPassword>
        {
            new StudentPassword { PasswordID = 1, StudentID = 1, Password = "SolarFlare1234" },
            new StudentPassword { PasswordID = 2, StudentID = 2, Password = "ThunderBold1234" },
            new StudentPassword { PasswordID = 3, StudentID = 3, Password = "AstralWhisper1234" },
        };
        
        _studentId = _students.Count + 1;
        _passwordId = _studentPasswords.Count + 1;
    }
    public Student? GetStudentById(int id)
    {
        return _students.FirstOrDefault(s => s.StudentID == id);
    }

    public Student? GetStudentByAlias(string alias)
    {
        return _students.FirstOrDefault(s => s.Alias == alias);
    }

    public List<Student> GetStudents()
    {
        return _students;
    }

    public List<Student> GetStudentByStartingLetter(char letter)
    {
        return _students.Where(s => s.Alias.StartsWith(letter)).ToList();
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
        _students.Add(student);
    }

    public void UpdateStudent(Student student)
    {
        var oldStudent = _students.FirstOrDefault(s => s.StudentID == student.StudentID);
        oldStudent.FirstName = student.FirstName;
        oldStudent.LastName = student.LastName;
        oldStudent.Alias = student.Alias;
        oldStudent.DoB = student.DoB;
    }

    public void DeleteStudent(int id)
    {
        _students.Remove(_students.FirstOrDefault(s => s.StudentID == id));
    }

    public void AddStudentPassword(StudentPassword studentPassword)
    {
        studentPassword.PasswordID = _passwordId++;
        _studentPasswords.Add(studentPassword);
    }

    public void UpdateStudentPassword(StudentPassword studentPassword)
    {
        var oldStudentPassword = _studentPasswords
            .FirstOrDefault(s => s.PasswordID == studentPassword.PasswordID);
        oldStudentPassword.Password = studentPassword.Password;
    }

    public GradesReport GetGradesReport(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}