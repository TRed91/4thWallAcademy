using Dapper;
using Dapper.Transaction;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Models;
using Microsoft.Data.SqlClient;

namespace FourthWallAcademy.Data.Repositories;

public class WeaknessRepository : IWeaknessRepository
{
    private readonly string _connectionString;

    public WeaknessRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public Weakness? GetWeaknessById(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var weakness = cn.Query<Weakness>("SELECT * FROM Weakness WHERE WeaknessID = @id", new { id })
                .FirstOrDefault();
            
            if (weakness == null) return null;
            
            weakness.WeaknessType = cn.Query<WeaknessType>("SELECT * FROM WeaknessType WHERE WeaknessTypeID = @WeaknessTypeID", 
                    new { weakness.WeaknessTypeID })
                .FirstOrDefault();
            
            return weakness;
        }
    }

    public List<Weakness> GetWeaknesses()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            return cn.Query<Weakness>("SELECT * FROM Weakness").ToList();
        }
    }

    public void AddWeakness(Weakness weakness)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO Weakness (WeaknessTypeID, WeaknessName, WeaknessDescription) 
                        VALUES (@WeaknessTypeID, @WeaknessName, @WeaknessDescription);
                        SELECT SCOPE_IDENTITY();";
            var p = new
            {
                weakness.WeaknessTypeID,
                weakness.WeaknessName,
                weakness.WeaknessDescription
            };
            weakness.WeaknessID = cn.ExecuteScalar<int>(sql, p);
        }
    }

    public void UpdateWeakness(Weakness weakness)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"UPDATE Weakness SET 
                    WeaknessName = @WeaknessName, 
                    WeaknessDescription = @WeaknessDescription,
                    WeaknessTypeID = @WeaknessTypeID
                    WHERE WeaknessID = @WeaknessID;";
            var P = new
            {
                weakness.WeaknessID,
                weakness.WeaknessName,
                weakness.WeaknessDescription,
                weakness.WeaknessTypeID
            };
            cn.Execute(sql, P);
        }
    }

    public void DeleteWeakness(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql1 = @"DELETE FROM Weakness WHERE WeaknessID = @id;";
            var sql2 = @"DELETE FROM StudentWeakness WHERE WeaknessID = @id;";

            cn.Open();
            using (var tr = cn.BeginTransaction())
            {
                tr.Execute(sql2, new { id });
                tr.Execute(sql1, new { id });
                tr.Commit();
            }
        }
    }

    public WeaknessesReport WeaknessReport()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"SELECT MIN(RiskLevel) AS MinRiskLv,
                               AVG(RiskLevel) AS AvgRiskLv,
                               MAX(RiskLevel) AS MaxRiskLv
                               FROM StudentWeakness;";
            
            return cn.Query<WeaknessesReport>(sql).FirstOrDefault();
        }
    }
}