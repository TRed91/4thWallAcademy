using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface ICourseRepository
{
    Course? GetCourseById(int id);
    List<Course> GetCourses();
    List<Subject> GetSubjects();
    void AddCourse(Course course);
    void UpdateCourse(Course course);
    void DeleteCourse(int id);
}