using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Core.Models;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class PowerService : IPowerService
{
    private readonly IPowersRepository _repo;

    public PowerService(IPowersRepository repo)
    {
        _repo = repo;
    }
    
    public Result<Power> GetPowerById(int id)
    {
        try
        {
            var power = _repo.GetPowerById(id);
            if (power == null)
            {
                return ResultFactory.Fail<Power>("Power not found");
            }

            return ResultFactory.Success(power);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<Power>(ex.Message);
        }
    }

    public Result<List<Power>> GetPowers()
    {
        try
        {
            var powers = _repo.GetPowers();
            return ResultFactory.Success(powers);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Power>>(ex.Message);
        }
    }

    public Result<List<PowerType>> GetPowerTypes()
    {
        try
        {
            var powerTypes = _repo.GetPowerTypes();
            var powers = _repo.GetPowers();
            foreach (var type in powerTypes)
            {
                foreach (var power in powers)
                {
                    if (type.PowerTypeID == power.PowerTypeID)
                    {
                        type.Powers.Add(power);
                    }
                }
            }
            return ResultFactory.Success(powerTypes);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<PowerType>>(ex.Message);
        }
    }

    public Result AddPower(Power power)
    {
        try
        {
            _repo.AddPower(power);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result UpdatePower(Power power)
    {
        try
        {
            _repo.UpdatePower(power);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeletePower(int id)
    {
        try
        {
            _repo.DeletePower(id);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result<PowersReport> GetPowersReport()
    {
        try
        {
            var report = _repo.PowersReport();
            return ResultFactory.Success(report);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<PowersReport>(ex.Message);
        }
    }

    public Result<List<PowerTypeReport>> GetPowersReportList()
    {
        try
        {
            var types = _repo.GetPowerTypes();
            var reports = _repo.PowersReportList();
            var typeReports = types.Select(t => new PowerTypeReport
            {
                PowerTypeID = t.PowerTypeID,
                PowerTypeName = t.PowerTypeName,
                PowerRatings = reports
                    .Where(r => r.PowerTypeID == t.PowerTypeID)
                    .ToList()
            }).ToList();
            return ResultFactory.Success(typeReports);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<PowerTypeReport>>(ex.Message);
        }
    }
}