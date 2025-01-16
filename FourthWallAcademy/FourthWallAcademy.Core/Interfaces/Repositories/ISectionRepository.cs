using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Repositories;

public interface ISectionRepository
{
    Section? GetSectionById(int id);
    StudentSection? GetStudentSection(int studentId, int sectionId);
    List<Section> GetSections();
    List<Section> GetSectionsByInstructorId(int instructorId);
    List<Section> GetSectionsByStudent(int studentId);
    List<StudentSection> GetStudentsBySection(int sectionId);
    void AddSection(Section section);
    void UpdateSection(Section section);
    void DeleteSection(int id);
    void AddStudentSection(StudentSection studentSection);
    void UpdateStudentSection(StudentSection studentSection);
    void DeleteStudentSection(int studentId, int sectionId);
    EnrollmentReport GetEnrollmentReport();
}