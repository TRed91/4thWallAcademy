using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IWeaknessRepository
{
    Weakness? GetWeaknessById(int id);
    List<Weakness> GetWeaknesses();
    void AddWeakness(Weakness weakness);
    void UpdateWeakness(Weakness weakness);
    void DeleteWeakness(int id);
}