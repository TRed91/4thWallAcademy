using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface IPowersRepository
{
    Power? GetPowerById(int id);
    List<Power> GetPowers();
    List<PowerType> GetPowerTypes();
    void AddPower(Power power);
    void UpdatePower(Power power);
    void DeletePower(int id);
    PowersReport PowersReport();
    List<PowerRatings> PowersReportList();
}