﻿using Dapper;
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
            var studentSql = "SELECT * FROM Student WHERE StudentID = @id";
            var studentPowerSql = @"SELECT * FROM StudentPower sp
                            INNER JOIN Power p ON sp.PowerID = p.PowerID
                            WHERE StudentID = @id";
            var studentWeaknessSql = @"SELECT * FROM StudentWeakness sw
                            INNER JOIN Weakness w ON sw.WeaknessID = w.WeaknessID
                            WHERE StudentID = @id";
            
            // fetch student
            var student = cn.Query<Student>(studentSql, new { id })
                .FirstOrDefault();
            
            if (student == null) return null;
            
            // fetch powers and weaknesses
            student.StudentPowers = new List<StudentPower>();
            student.StudentWeaknesses = new List<StudentWeakness>();
            
            var cmdPowers = new SqlCommand(studentPowerSql, cn);
            var cmdWeaknesses = new SqlCommand(studentWeaknessSql, cn);
            cmdPowers.Parameters.AddWithValue("@id", id);
            cmdWeaknesses.Parameters.AddWithValue("@id", id);
            cn.Open();
            using (var reader = cmdPowers.ExecuteReader())
            {
                while (reader.Read())
                {
                    var studentPower = new StudentPower();
                    studentPower.PowerID = (int)reader["PowerID"];
                    studentPower.StudentID = (int)reader["StudentID"];
                    studentPower.Rating = (byte)reader["Rating"];
                    studentPower.Power = new Power
                    {
                        PowerID = (int)reader["PowerID"],
                        PowerTypeID = (int)reader["PowerTypeID"],
                        PowerName = (string)reader["PowerName"],
                        PowerDescription = (string)reader["PowerDescription"],
                    };
                    student.StudentPowers.Add(studentPower);
                }
            }
            using (var reader = cmdWeaknesses.ExecuteReader())
            {
                while (reader.Read())
                {
                    var studentWeakness = new StudentWeakness();
                    studentWeakness.WeaknessID = (int)reader["WeaknessID"];
                    studentWeakness.StudentID = (int)reader["StudentID"];
                    studentWeakness.RiskLevel = (byte)reader["RiskLevel"];
                    studentWeakness.Weakness = new Weakness
                    {
                        WeaknessID = (int)reader["WeaknessID"],
                        WeaknessTypeID = (int)reader["WeaknessTypeID"],
                        WeaknessName = (string)reader["WeaknessName"],
                        WeaknessDescription = (string)reader["WeaknessDescription"],
                    };
                    student.StudentWeaknesses.Add(studentWeakness);
                }
            }
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