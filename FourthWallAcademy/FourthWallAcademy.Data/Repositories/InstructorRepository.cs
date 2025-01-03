using Dapper;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FourthWallAcademy.Data.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly string _connectionString;

    public InstructorRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public Instructor? GetInstructorById(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            return cn.Query<Instructor>("SELECT * FROM Instructor WHERE InstructorID = @id", 
                    new { id })
                .FirstOrDefault();
        }
    }

    public List<Instructor> GetInstructors()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            return cn.Query<Instructor>("SELECT * FROM Instructor")
                .ToList();
        }
    }

    public void AddInstructor(Instructor instructor)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO Instructor (Alias, HireDate, TermDate) 
                    VALUES (@Alias, @HireDate, @TermDate);
                    SELECT SCOPE_IDENTITY();";
            var p = new
            {
                instructor.Alias,
                instructor.HireDate,
                instructor.TermDate
            };
            instructor.InstructorID = cn.ExecuteScalar<int>(sql, p);
        }
    }

    public void UpdateInstructor(Instructor instructor)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"UPDATE Instructor SET 
                      Alias = @Alias,
                      HireDate = @HireDate,
                      TermDate = @TermDate
                      WHERE InstructorID = @InstructorID;";
            var p = new
            {
                instructor.Alias,
                instructor.HireDate,
                instructor.TermDate,
                instructor.InstructorID
            };
            cn.Execute(sql, p);
        }
    }

    public void DeleteInstructor(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            cn.Execute("DELETE FROM Instructor WHERE InstructorID = @id;", 
                new { id });
        }
    }
}