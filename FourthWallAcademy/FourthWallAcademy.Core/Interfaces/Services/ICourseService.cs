using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface ICourseService
{
    Result<Course> GetCourseById(int id);
    Result<List<Course>> GetCourses();
    Result AddCourse(Course course);
    Result UpdateCourse(Course course);
    Result DeleteCourse(int id);
}