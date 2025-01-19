using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Models;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class WeaknessService : IWeaknessService
{
    private readonly IWeaknessRepository _repo;

    public WeaknessService(IWeaknessRepository repo)
    {
        _repo = repo;
    }
    
    public Result<Weakness> GetWeaknessById(int id)
    {
        try
        {
            var weakness = _repo.GetWeaknessById(id);
            if (weakness == null)
            {
                return ResultFactory.Fail<Weakness>("Weakness not found");
            }

            return ResultFactory.Success(weakness);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<Weakness>(ex.Message);
        }
    }

    public Result<List<Weakness>> GetWeaknesses()
    {
        try
        {
            var weaknesses = _repo.GetWeaknesses();
            return ResultFactory.Success(weaknesses);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Weakness>>(ex.Message);
        }
    }

    public Result AddWeakness(Weakness weakness)
    {
        try
        {
            _repo.AddWeakness(weakness);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result UpdateWeakness(Weakness weakness)
    {
        try
        {
            _repo.UpdateWeakness(weakness);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeleteWeakness(int id)
    {
        try
        {
            _repo.DeleteWeakness(id);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result<WeaknessesReport> GetWeaknessReport()
    {
        try
        {
            var weaknessReport = _repo.WeaknessReport();
            return ResultFactory.Success(weaknessReport);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<WeaknessesReport>(ex.Message);
        }
    }

    public Result<List<WeaknessTypeReport>> GetWeaknessReportList()
    {
        try
        {
            var types = _repo.GetWeaknessTypes();
            var reports = _repo.WeaknessReportList();
            var typeReports = types.Select(t => new WeaknessTypeReport
            {
                WeaknessTypeID = t.WeaknessTypeID,
                WeaknessTypeName = t.WeaknessTypeName,
                WeaknessRiskLvs = reports
                    .Where(r => r.WeaknessTypeID == t.WeaknessTypeID)
                    .ToList()
            }).ToList();
            return ResultFactory.Success(typeReports);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<WeaknessTypeReport>>(ex.Message);
        }
    }
}