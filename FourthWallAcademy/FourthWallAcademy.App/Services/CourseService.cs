using System.Xml.Linq;
using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo)
    {
        _repo = repo;
    }
    
    public Result<Course> GetCourseById(int id)
    {
        try
        {
            var course = _repo.GetCourseById(id);
            if (course == null)
            {
                return ResultFactory.Fail<Course>("Course not found");
            }

            return ResultFactory.Success(course);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<Course>(ex.Message);
        }
    }

    public Result<List<Course>> GetCourses()
    {
        try
        {
            var courses = _repo.GetCourses();
            return ResultFactory.Success(courses);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Course>>(ex.Message);
        }
    }

    public Result<List<Subject>> GetSubjects()
    {
        try
        {
            var subjects = _repo.GetSubjects();
            return ResultFactory.Success(subjects);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Subject>>(ex.Message);
        }
    }

    public Result AddCourse(Course course)
    {
        try
        {
            _repo.AddCourse(course);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result UpdateCourse(Course course)
    {
        try
        {
            var originalCourse = _repo.GetCourseById(course.CourseID);
            if (originalCourse == null)
            {
                return ResultFactory.Fail<Course>($"Course with id {course.CourseID} not found");
            }
            _repo.UpdateCourse(course);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeleteCourse(int id)
    {
        try
        {
            var course = _repo.GetCourseById(id);
            if (course == null)
            {
                return ResultFactory.Fail<Course>($"Course with id {id} not found");
            }
            _repo.DeleteCourse(id);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }
}