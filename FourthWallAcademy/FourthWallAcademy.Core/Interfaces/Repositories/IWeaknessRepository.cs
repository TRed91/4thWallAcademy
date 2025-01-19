using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IWeaknessRepository
{
    Weakness? GetWeaknessById(int id);
    List<Weakness> GetWeaknesses();
    List<WeaknessType> GetWeaknessTypes();
    void AddWeakness(Weakness weakness);
    void UpdateWeakness(Weakness weakness);
    void DeleteWeakness(int id);
    WeaknessesReport WeaknessReport();
    List<WeaknessRiskLvs> WeaknessReportList();
}