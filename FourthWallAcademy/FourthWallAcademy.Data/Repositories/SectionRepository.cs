using Dapper;
using Dapper.Transaction;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FourthWallAcademy.Data.Repositories;

public class SectionRepository : ISectionRepository
{
    private readonly string _connectionString;

    public SectionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public Section? GetSectionById(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"
                SELECT se.*, 
                       i.InstructorID AS iId, Alias, HireDate, TermDate, 
                       c.CourseID AS cId, c.SubjectID, CourseName, CourseDescription, Credits, 
                       su.SubjectID AS suId, SubjectName FROM Section se
                INNER JOIN Instructor i ON i.InstructorID = se.InstructorID
                INNER JOIN Course c ON c.CourseID = se.CourseID
                INNER JOIN Subject su ON su.SubjectID = c.SubjectID
                WHERE se.SectionID = @id;";

            return cn.Query<Section, Instructor, Course, Subject, Section>(sql, 
                (section, instructor, course, subject) =>
                {
                    section.Instructor = instructor;
                    section.Course = course;
                    section.Course.Subject = subject;
                    return section;
                }, new { id }, splitOn: "iId, cId, suId")
                .FirstOrDefault();
        }
    }

    public StudentSection? GetStudentSection(int studentId, int sectionId)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"SELECT * FROM StudentSection 
                        WHERE StudentID = @studentId AND SectionID = @sectionId";
            return cn.QueryFirstOrDefault<StudentSection>(sql, new { studentId, sectionId });
        }
    }

    public List<Section> GetSections()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"
                SELECT se.*, 
                       i.InstructorID AS iId, Alias, HireDate, TermDate, 
                       c.CourseID AS cId, c.SubjectID, CourseName, CourseDescription, Credits, 
                       su.SubjectID AS suId, SubjectName 
                FROM Section se
                INNER JOIN Instructor i ON i.InstructorID = se.InstructorID
                INNER JOIN Course c ON c.CourseID = se.CourseID
                INNER JOIN Subject su ON su.SubjectID = c.SubjectID";
            
            return cn.Query<Section, Instructor, Course, Subject, Section>(sql,
                (section, instructor, course, subject) =>
                {
                    section.Instructor = instructor;
                    section.Course = course;
                    section.Course.Subject = subject;
                    return section;
                }, splitOn: "iId, cId, suId")
                .ToList();
        }
    }

    public List<Section> GetSectionsByInstructorId(int instructorId)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"
                SELECT se.*, 
                       i.InstructorID AS iId, Alias, HireDate, TermDate, 
                       c.CourseID AS cId, c.SubjectID, CourseName, CourseDescription, Credits, 
                       su.SubjectID AS suId, SubjectName 
                FROM Section se
                INNER JOIN Instructor i ON i.InstructorID = se.InstructorID
                INNER JOIN Course c ON c.CourseID = se.CourseID
                INNER JOIN Subject su ON su.SubjectID = c.SubjectID
                WHERE i.InstructorID = @instructorId;";
            
            return cn.Query<Section, Instructor, Course, Subject, Section>(sql,
                    (section, instructor, course, subject) =>
                    {
                        section.Instructor = instructor;
                        section.Course = course;
                        section.Course.Subject = subject;
                        return section;
                    }, new { instructorId }, splitOn: "iId, cId, suId")
                .ToList();
        }
    }

    public List<Section> GetSectionsByStudent(int studentId)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"
                SELECT 
                    se.*,
                    c.CourseID AS cId, SubjectID, CourseName, CourseDescription, Credits , 
                    ss.SectionID AS ssId, ss.StudentID, Grade, Absences,
                    s.StudentID AS sId, FirstName, LastName, Alias, DoB 
                FROM Section se
                INNER JOIN Course c ON c.CourseID = se.CourseID
                INNER JOIN StudentSection ss ON ss.SectionID = se.SectionID
                INNER JOIN Student s ON s.StudentID = ss.StudentID
                WHERE s.StudentID = @studentId
                ORDER BY se.StartDate DESC, se.StartTime";
            
            return cn.Query<Section, Course, StudentSection, Student, Section>(sql,
                (section, course, studentSection, student) =>
                {
                    section.Course = course;
                    section.StudentSections = new List<StudentSection>();
                    section.StudentSections.Add(studentSection);
                    section.StudentSections.ForEach(s => s.Student = student);
                    return section;
                }, new { studentId }, splitOn: "cId, ssId, sId")
                .ToList();
        }
    }

    public List<StudentSection> GetStudentsBySection(int sectionId)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"
                    SELECT s.*, 
                           st.StudentID AS stId, FirstName, LastName, Alias, DoB 
                    FROM StudentSection s 
                    INNER JOIN Student st ON s.StudentID = st.StudentID
                    WHERE s.SectionID = @sectionId;";
            return cn.Query<StudentSection, Student, StudentSection>(sql, (section, student) =>
                {
                    section.Student = student;
                    return section;
                }, new { sectionId }, splitOn: "stId")
                .ToList();
        }
    }

    public void AddSection(Section section)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"
                INSERT INTO Section(InstructorID, CourseID, StartDate, EndDate, StartTime, EndTime)
                VALUES (@InstructorID, @CourseID, @StartDate, @EndDate, @StartTime, @EndTime);
                SELECT SCOPE_IDENTITY();";
            var p = new
            {
                section.InstructorID,
                section.CourseID,
                section.StartDate,
                section.EndDate,
                section.StartTime,
                section.EndTime
            };
            section.SectionID = cn.ExecuteScalar<int>(sql, p);
        }
    }

    public void UpdateSection(Section section)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"
                UPDATE Section SET 
                    StartDate = @StartDate, 
                    EndDate = @EndDate, 
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    CourseID = @CourseID,
                    InstructorID = @InstructorID
                WHERE SectionID = @SectionID;";
            var p = new
            {
                section.SectionID,
                section.StartDate,
                section.EndDate,
                section.StartTime,
                section.EndTime,
                section.CourseID,
                section.InstructorID,
            };
            cn.Execute(sql, p);
        }
    }

    public void DeleteSection(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql1 = "DELETE FROM Section WHERE SectionID = @id;";
            var sql2 = "DELETE FROM StudentSection WHERE SectionID = @id;";

            cn.Open();
            using (var tr = cn.BeginTransaction())
            {
                tr.Execute(sql2, new { id });
                tr.Execute(sql1, new { id });
                tr.Commit();
            }
        }
    }

    public void AddStudentSection(StudentSection studentSection)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO StudentSection(StudentID, SectionID, Grade, Absences)
                        VALUES (@StudentID, @SectionID, @Grade, @Absences);";
            var p = new
            {
                studentSection.StudentID,
                studentSection.SectionID,
                studentSection.Grade,
                studentSection.Absences
            };
            cn.Execute(sql, p);
        }
    }

    public void UpdateStudentSection(StudentSection studentSection)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"UPDATE StudentSection 
                        SET Grade = @Grade, 
                            Absences = @Absences;";
            var p = new
            {
                studentSection.Grade,
                studentSection.Absences
            };
            cn.Execute(sql, p);
        }
    }

    public void DeleteStudentSection(int studentId, int sectionId)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"DELETE FROM StudentSection 
                        WHERE StudentID = @studentId AND SectionID = @sectionId;";
            var p = new
            {
                studentId,
                sectionId
            };
            cn.Execute(sql, p);
        }
    }
}