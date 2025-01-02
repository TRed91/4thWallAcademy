using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface ISectionRepository
{
    Section? GetSectionById(int id);
    List<Section> GetSections();
    List<Section> GetSectionsByInstructorId(int instructorId);
    void AddSection(Section section);
    void UpdateSection(Section section);
    void DeleteSection(int id);
}