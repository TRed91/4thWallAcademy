using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface ISectionRepository
{
    Section? GetSectionById(int id);
    List<Section> GetSections();
    List<Section> GetSectionsByInstructorId(int instructorId);
    List<Section> GetSectionsByStudent(int studentId);
    List<StudentSection> GetStudentsBySection(int sectionId);
    void AddSection(Section section);
    void UpdateSection(Section section);
    void DeleteSection(int id);
}