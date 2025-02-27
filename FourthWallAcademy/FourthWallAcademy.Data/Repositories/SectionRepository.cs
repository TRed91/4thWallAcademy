﻿using Dapper;
using Dapper.Transaction;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Models;
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
            var section = cn.QueryFirstOrDefault<StudentSection>(sql, new { studentId, sectionId });
            section.Student = cn.QueryFirstOrDefault<Student>("SELECT * FROM Student WHERE StudentID = @studentId", new { studentId });
            section.Section = cn.QueryFirstOrDefault<Section>("SELECT * FROM Section WHERE SectionID = @sectionId", new { sectionId });
            section.Section.Course = cn.QueryFirstOrDefault<Course>("SELECT * FROM Course WHERE CourseID = @CourseID", new { section.Section.CourseID });
            return section;
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
                    student.StudentID = section.StudentID;
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
                            Absences = @Absences
                        WHERE SectionID = @SectionID AND StudentID = @StudentID;";
            var p = new
            {
                studentSection.Grade,
                studentSection.Absences,
                studentSection.SectionID,
                studentSection.StudentID
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

    public EnrollmentReport GetEnrollmentReport(DateTime startDate, DateTime endDate)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var enrollmentsQuery = @"SELECT COUNT(*) FROM StudentSection ss 
                         INNER JOIN Section s ON ss.SectionID = s.SectionID
                         WHERE s.StartDate >= @startDate AND s.StartDate <= @endDate;";
            var absencesQuery = @"SELECT SUM(Absences) FROM StudentSection ss 
                         INNER JOIN Section s ON ss.SectionID = s.SectionID
                         WHERE s.StartDate >= @startDate AND s.StartDate <= @endDate;";
            var studentsQuery = "SELECT COUNT(*) FROM Student";
            var sectionsQuery = "SELECT COUNT(*) FROM Section";
            var sectionListQuery = @"SELECT Alias AS InstructorAlias, CourseName, StartDate, EndDate, 
                                            COUNT(DISTINCT StudentID) AS StudentCount, 
                                            SUM(Absences) AS Absences 
                                     FROM Section s
                                     INNER JOIN Instructor i ON i.InstructorID = s.InstructorID
                                     INNER JOIN Course c ON c.CourseID = s.CourseID
                                     INNER JOIN StudentSection ss ON ss.SectionID = s.SectionID
                                     WHERE StartDate >= @startDate AND StartDate <= @endDate
                                     GROUP BY CourseName, Alias, StartDate, EndDate
                                     ORDER BY StartDate";
            var studentListQuery = @"SELECT Alias AS StudentAlias, 
                                            COUNT(ss.SectionID) AS SectionsCount, 
                                            SUM(Absences) AS Absences
                                     FROM Student s 
                                     INNER JOIN StudentSection ss ON ss.StudentID = s.StudentID
                                     INNER JOIN Section se ON ss.SectionID = se.SectionID
                                     WHERE StartDate >= @startDate AND StartDate <= @endDate
                                     GROUP BY Alias;";
            
            var p = new
            {
                startDate,
                endDate,
            };
            
            var report = new EnrollmentReport();
            report.CountEnrollments = cn.ExecuteScalar<int>(enrollmentsQuery, p);
            report.SumAbsences = cn.ExecuteScalar<int>(absencesQuery, p);
            report.CountStudents = cn.ExecuteScalar<int>(studentsQuery, p);
            report.CountSections = cn.ExecuteScalar<int>(sectionsQuery, p);
            report.SectionEnrollments = cn.Query<SectionEnrollment>(sectionListQuery, p).ToList();
            report.StudentEnrollments = cn.Query<StudentEnrollment>(studentListQuery, p).ToList();
            return report;
        }
    }
}