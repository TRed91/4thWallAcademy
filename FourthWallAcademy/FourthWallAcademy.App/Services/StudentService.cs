using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(string connectionString)
    {
        _repo = new StudentRepository(connectionString);
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

    public Result AddStudent(Student student)
    {
        if (DateTime.Today.Year - student.DoB.Year < 14)
        {
            return ResultFactory.Fail("Student is too young");
        }
        try
        {
            var existingStudent = _repo.GetStudentByAlias(student.Alias);
            if (existingStudent != null)
            {
                return ResultFactory.Fail($"Student with Alias {student.Alias} already exists");
            }
            _repo.AddStudent(student);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
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
}