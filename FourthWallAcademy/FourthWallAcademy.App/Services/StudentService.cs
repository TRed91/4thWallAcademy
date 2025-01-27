using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Core.Models;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo)
    {
        _repo = repo;
    }
    
    public Result<Student> GetStudentById(int id)
    {
        try
        {
            var student = _repo.GetStudentById(id);
            if (student == null)
            {
                return ResultFactory.Fail<Student>("Student not found");
            }

            return ResultFactory.Success(student);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<Student>(ex.Message);
        }
    }

    public Result<List<Student>> GetStudents()
    {
        try
        {
            var students = _repo.GetStudents();
            return ResultFactory.Success(students);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Student>>(ex.Message);
        }
    }

    public Result<List<Student>> GetStudentsByStartingLetter(char letter)
    {
        try
        {
            var students = _repo.GetStudentByStartingLetter(letter);
            return ResultFactory.Success(students);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Student>>(ex.Message);
        }
    }

    public Result AddStudentPower(StudentPower studentPower)
    {
        if (studentPower.Rating < 1 || studentPower.Rating > 100)
        {
            return ResultFactory.Fail("Rating must be between 1 and 100");
        }
        try
        {
            _repo.AddStudentPower(studentPower);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result UpdateStudentPower(StudentPower studentPower)
    {
        try
        {
            _repo.UpdateStudentPower(studentPower);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeleteStudentPower(StudentPower studentPower)
    {
        if (studentPower.PowerID == 0)
        {
            return ResultFactory.Fail("Power ID must be set");
        }
        if (studentPower.StudentID == 0)
        {
            return ResultFactory.Fail("Student ID must be set");
        }
        try
        {
            _repo.DeleteStudentPower(studentPower);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result AddStudentWeakness(StudentWeakness studentWeakness)
    {
        if (studentWeakness.RiskLevel < 1 || studentWeakness.RiskLevel > 10)
        {
            return ResultFactory.Fail("Risk level must be between 1 and 10");
        }
        try
        {
            _repo.AddStudentWeakness(studentWeakness);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result UpdateStudentWeakness(StudentWeakness studentWeakness)
    {
        try
        {
            _repo.UpdateStudentWeakness(studentWeakness);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeleteStudentWeakness(StudentWeakness studentWeakness)
    {
        if (studentWeakness.WeaknessID == 0)
        {
            return ResultFactory.Fail("Weakness ID must be set");
        }
        if (studentWeakness.StudentID == 0)
        {
            return ResultFactory.Fail("Student ID must be set");
        }
        try
        {
            _repo.DeleteStudentWeakness(studentWeakness);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result<StudentPassword> AddStudent(Student student)
    {
        if (DateTime.Today.Year - student.DoB.Year < 14)
        {
            return ResultFactory.Fail<StudentPassword>("Student is too young");
        }
        try
        {
            var existingStudent = _repo.GetStudentByAlias(student.Alias);
            if (existingStudent != null)
            {
                return ResultFactory
                    .Fail<StudentPassword>($"Student with Alias {student.Alias} already exists");
            }
            _repo.AddStudent(student);
            var password = GenerateStudentPassword(student.Alias);
            var studentPassword = new StudentPassword
            {
                StudentID = student.StudentID,
                Password = password
            };
            _repo.AddStudentPassword(studentPassword);
            return ResultFactory.Success(studentPassword);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<StudentPassword>(ex.Message);
        }
    }

    public Result UpdateStudent(Student student)
    {
        if (DateTime.Today.Year - student.DoB.Year < 14)
        {
            return ResultFactory.Fail("Student is too young");
        }
        try
        {
            _repo.UpdateStudent(student);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeleteStudent(int id)
    {
        try
        {
            _repo.DeleteStudent(id);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result<GradesReport> GetGradesReport(DateTime startDate, DateTime endDate)
    {
        try
        {
            var gradesReport = _repo.GetGradesReport(startDate, endDate);
            return ResultFactory.Success(gradesReport);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<GradesReport>(ex.Message);
        }
    }

    private static string GenerateStudentPassword(string alias)
    {
        string formattedAlias = string.Join("", alias.Where(c => !char.IsWhiteSpace(c)));
        var rng = new Random();
        int[] nums =
        {
            rng.Next(0, 10),
            rng.Next(0, 10),
            rng.Next(0, 10),
            rng.Next(0, 10),
        };
        var numsString = string.Join("", nums);
        return "!" + formattedAlias + numsString;
    }
}