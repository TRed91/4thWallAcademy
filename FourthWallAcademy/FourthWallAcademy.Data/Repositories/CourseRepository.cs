using Dapper;
using Dapper.Transaction;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FourthWallAcademy.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly string _connectionString;

    public CourseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public Course? GetCourseById(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"SELECT c.*, s.SubjectID AS sId, SubjectName FROM Course c 
                         INNER JOIN Subject s ON c.SubjectID = s.SubjectID
                         WHERE CourseID = @id;";
            return cn.Query<Course, Subject, Course>(sql, 
                (course, subject) =>
                {
                    course.Subject = subject;
                    return course;
                }, splitOn: "sId",
                param: new { id }).FirstOrDefault();
        }
    }

    public List<Course> GetCourses()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"SELECT c.*, s.SubjectID AS sId, SubjectName 
                        FROM Course c 
                        INNER JOIN Subject s ON c.SubjectID = s.SubjectID";
            return cn.Query<Course, Subject, Course>(sql, (course, subject) =>
            {
                course.Subject = subject;
                return course;
            }, splitOn: "sId").ToList();
        }
    }

    public List<Subject> GetSubjects()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            return cn.Query<Subject>("SELECT * FROM Subject")
                .ToList();
        }
    }

    public void AddCourse(Course course)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO Course (SubjectID, CourseName, CourseDescription, Credits) 
                        VALUES (@SubjectID, @CourseName, @CourseDescription, @Credits);
                        SELECT SCOPE_IDENTITY();";
            var p = new
            {
                course.SubjectID,
                course.CourseName,
                course.CourseDescription,
                course.Credits
            };
            course.CourseID = cn.ExecuteScalar<int>(sql, p);
        }
    }

    public void UpdateCourse(Course course)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"UPDATE Course SET 
                   SubjectID = @SubjectID,
                   CourseName = @CourseName,
                   CourseDescription = @CourseDescription,
                   Credits = @Credits
                   WHERE CourseID = @CourseID;";
            var p = new
            {
                course.SubjectID,
                course.CourseName,
                course.CourseDescription,
                course.Credits,
                course.CourseID
            };
            cn.Execute(sql, p);
        }
    }

    public void DeleteCourse(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql1 = "DELETE FROM Course WHERE CourseID = @id;";
            var sql2 = "DELETE FROM Section WHERE CourseID = @id;";
            var sql3 = @"DELETE ss FROM StudentSection ss 
                    INNER JOIN Section s ON ss.SectionID = s.SectionID 
                    WHERE CourseID = @id;";
            using (var tr = cn.BeginTransaction())
            {
                tr.Execute(sql3, new { id });
                tr.Execute(sql2, new { id });
                tr.Execute(sql1, new { id });
                tr.Commit();
            }
        }
    }
}