using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class PowerService : IPowerService
{
    private readonly IPowersRepository _repo;

    public PowerService(string connectionString)
    {
        _repo = new PowerRepository(connectionString);
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
}