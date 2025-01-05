using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface IPowerService
{
    Result<Power> GetPowerById(int id);
    Result<List<Power>> GetPowers();
    Result AddPower(Power power);
    Result UpdatePower(Power power);
    Result DeletePower(int id);
}