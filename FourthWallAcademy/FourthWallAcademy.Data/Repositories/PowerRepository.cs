using Dapper;
using Dapper.Transaction;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FourthWallAcademy.Data.Repositories;

public class PowerRepository : IPowersRepository
{
    private readonly string _connectionString;

    public PowerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public Power? GetPowerById(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var power = cn.Query<Power>("SELECT * FROM Power WHERE PowerID = @id", new { id })
                .FirstOrDefault();
            
            if (power == null) return null;
            
            power.PowerType = cn.Query<PowerType>("SELECT * FROM PowerType WHERE PowerTypeID = @PowerTypeID", 
                new { power.PowerTypeID })
                .FirstOrDefault();
            
            return power;
        }
    }

    public List<Power> GetPowers()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"SELECT p.*, 
                    pt.PowerTypeID AS ptId, PowerTypeName, PowerTypeDescription FROM Power p 
                    INNER JOIN PowerType pt ON pt.PowerTypeID = p.PowerTypeID";
            return cn.Query<Power, PowerType, Power>(sql, (power, type) =>
            {
                power.PowerType = type;
                return power;
            }, splitOn: "ptId").ToList();
        }
    }

    public List<PowerType> GetPowerTypes()
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            return cn.Query<PowerType>("SELECT * FROM PowerType").ToList();
        }
    }

    public void AddPower(Power power)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"INSERT INTO Power (PowerTypeID, PowerName, PowerDescription) 
                        VALUES (@PowerTypeID, @PowerName, @PowerDescription);
                        SELECT SCOPE_IDENTITY();";
            var p = new
            {
                power.PowerTypeID,
                power.PowerName,
                power.PowerDescription
            };
            power.PowerID = cn.ExecuteScalar<int>(sql, p);
        }
    }

    public void UpdatePower(Power power)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql = @"UPDATE Power SET 
                 PowerName = @PowerName, 
                 PowerDescription = @PowerDescription,
                 PowerTypeID = @PowerTypeID
                 WHERE PowerID = @PowerID;";
            var p = new
            {
                power.PowerID,
                power.PowerName,
                power.PowerDescription,
                power.PowerTypeID
            };
            cn.Execute(sql, p);
        }
    }

    public void DeletePower(int id)
    {
        using (var cn = new SqlConnection(_connectionString))
        {
            var sql1 = @"DELETE FROM Power WHERE PowerID = @id;";
            var sql2 = @"DELETE FROM StudentPower WHERE PowerID = @id;";

            cn.Open();
            using (var tr = cn.BeginTransaction())
            {
                tr.Execute(sql2, new { id });
                tr.Execute(sql1, new { id });
                tr.Commit();
            }
        }
    }
}