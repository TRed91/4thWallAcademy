using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IWeaknessService
{
    Result<Weakness> GetWeaknessById(int id);
    Result<List<Weakness>> GetWeaknesses();
    Result AddWeakness(Weakness weakness);
    Result UpdateWeakness(Weakness weakness);
    Result DeleteWeakness(int id);
    Result<WeaknessesReport> GetWeaknessReport();
}