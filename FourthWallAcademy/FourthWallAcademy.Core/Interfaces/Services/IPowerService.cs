using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface IPowerService
{
    Result<Power> GetPowerById(int id);
    Result<List<Power>> GetPowers();
    Result<List<PowerType>> GetPowerTypes();
    Result AddPower(Power power);
    Result UpdatePower(Power power);
    Result DeletePower(int id);
    Result<PowersReport> GetPowersReport();
    Result<List<PowerTypeReport>> GetPowersReportList();
}