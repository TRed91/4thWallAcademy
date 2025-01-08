using Dapper;
using Dapper.Transaction;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FourthWallAcademy.Data.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public Student? GetStudentById(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql1 = "SELECT * FROM Student WHERE StudentID = @id";
            var sql2 = @"SELECT sp.*,
                         p.PowerID AS pId, p.PowerTypeID, PowerName, PowerDescription,
                         pt.PowerTypeID AS ptId, PowerTypeName, PowerTypeDescription
                         FROM StudentPower sp
                         INNER JOIN Power p ON p.PowerID = sp.PowerID 
                         INNER JOIN PowerType pt ON pt.PowerTypeID = p.PowerTypeID
                         WHERE StudentID = @id";
            var sql3 = @"SELECT sw.*, 
                        w.WeaknessID AS wId, w.WeaknessTypeID, WeaknessName, WeaknessDescription,
                        wt.WeaknessTypeID AS wtId, WeaknessTypeName, WeaknessTypeDescription
                        FROM StudentWeakness sw
                        INNER JOIN Weakness w ON w.WeaknessID = sw.WeaknessID
                        INNER JOIN WeaknessType wt ON wt.WeaknessTypeID = w.WeaknessTypeID
                        WHERE StudentID = @id";
            
            var student = cn.Query<Student>(sql1, new { id }).FirstOrDefault();
            
            if (student == null) return null;
            
            student.StudentPowers = cn.Query<StudentPower, Power, PowerType, StudentPower>(sql2, 
                    (sp, p, pt) =>
                    {
                        sp.Power = p;
                        sp.Power.PowerType = pt;
                        return sp;
                    }, new { id }, splitOn: "pId, ptId")
                .ToList();
            
            student.StudentWeaknesses = cn.Query<StudentWeakness, Weakness, WeaknessType, StudentWeakness>(sql3, 
                    (sw, w, wt) =>
                    {
                        sw.Weakness = w;
                        sw.Weakness.WeaknessType = wt;
                        return sw;
                    }, new { id }, splitOn: "wId, wtId")
                .ToList();
            
            return student;
        }
    }

    public Student? GetStudentByAlias(string alias)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql1 = "SELECT * FROM Student WHERE Alias = @alias";
            var sql2 = @"SELECT sp.*,
                         p.PowerID AS pId, PowerTypeID, PowerName, PowerDescription 
                         FROM StudentPower sp
                         INNER JOIN Power p ON p.PowerID = sp.PowerID 
                         WHERE StudentID = @id";
            var sql3 = @"SELECT sw.*, 
                        w.WeaknessID AS wId, WeaknessTypeID, WeaknessName, WeaknessDescription 
                        FROM StudentWeakness sw
                        INNER JOIN Weakness w ON w.WeaknessID = sw.WeaknessID
                        WHERE StudentID = @id";
            
            var student = cn.Query<Student>(sql1, new { alias }).FirstOrDefault();
            
            if (student == null) return null;
            
            student.StudentPowers = cn.Query<StudentPower, Power, StudentPower>(sql2, 
                (sp, p) =>
                {
                    sp.Power = p;
                    return sp;
                }, new { id = student.StudentID }, splitOn: "pId")
                .ToList();
            
            student.StudentWeaknesses = cn.Query<StudentWeakness, Weakness, StudentWeakness>(sql3, 
                    (sw, w) =>
                    {
                        sw.Weakness = w;
                        return sw;
                    }, new { id = student.StudentID }, splitOn: "wId")
                .ToList();
            
            return student;
        }
    }

    public List<Student> GetStudents()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            return cn.Query<Student>("SELECT * FROM Student").ToList();
        }
    }

    public List<Student> GetStudentByStartingLetter(char letter)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = "SELECT * FROM Student WHERE Alias LIKE @letter + '%'";
            return cn.Query<Student>(sql, new { letter }).ToList();
        }
    }

    public void AddStudentPower(StudentPower studentPower)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO StudentPower (StudentID, PowerID, Rating)
                        VALUES (@StudentID, @PowerID, @Rating);";
            cn.Execute(sql, new
            {
                studentPower.StudentID, 
                studentPower.PowerID, 
                studentPower.Rating
            });
        }
    }

    public void DeleteStudentPower(StudentPower studentPower)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"DELETE FROM StudentPower 
                        WHERE StudentID = @StudentID AND PowerID = @PowerID";
            cn.Execute(sql, new
            {
                studentPower.StudentID, 
                studentPower.PowerID
            });
        }
    }

    public void AddStudentWeakness(StudentWeakness studentWeakness)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO StudentWeakness (StudentID, WeaknessID, RiskLevel)
                        VALUES (@StudentID, @WeaknessID, @RiskLevel);";
            cn.Execute(sql, new
            {
                studentWeakness.StudentID,
                studentWeakness.WeaknessID,
                studentWeakness.RiskLevel
            });
        }
    }

    public void DeleteStudentWeakness(StudentWeakness studentWeakness)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"DELETE FROM StudentWeakness 
                        WHERE StudentID = @StudentID AND WeaknessID = @WeaknessID";
            cn.Execute(sql, new
            {
                studentWeakness.StudentID,
                studentWeakness.WeaknessID,
            });
        }
    }

    public void AddStudent(Student student)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO Student (FirstName, LastName, Alias, DoB) 
                        VALUES (@FirstName, @LastName, @Alias, @DoB);

                        SELECT SCOPE_IDENTITY();";
            var p = new
            {
                student.FirstName, 
                student.LastName, 
                student.Alias, 
                student.DoB
            };
            
            student.StudentID = cn.ExecuteScalar<int>(sql, p);
        }
    }

    public void UpdateStudent(Student student)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"UPDATE Student SET 
                   FirstName = @FirstName,
                   LastName = @LastName,
                   Alias = @Alias,
                   DoB = @DoB
                   WHERE StudentID = @StudentID;";
            var p = new
            {
                student.FirstName,
                student.LastName,
                student.Alias,
                student.DoB,
                student.StudentID
            };
            cn.Execute(sql, p);
        }
    }

    public void DeleteStudent(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql1 = @"DELETE FROM Student WHERE StudentID = @id;";
            var sql2 = @"DELETE FROM StudentPower WHERE StudentID = @id;";
            var sql3 = @"DELETE FROM StudentWeakness WHERE StudentID = @id;";
            var sql4 = @"DELETE FROM StudentSection WHERE StudentID = @id;";

            var p = new { id };

            using (var tr = cn.BeginTransaction())
            {
                tr.Execute(sql2, p);
                tr.Execute(sql3, p);
                tr.Execute(sql4, p);
                tr.Execute(sql1, p);
                tr.Commit();
            }
        }
    }
}