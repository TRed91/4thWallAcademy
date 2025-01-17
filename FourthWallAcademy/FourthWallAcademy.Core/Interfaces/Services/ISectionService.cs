using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Core.Interfaces.Services;

public interface ISectionService
{
    Result<StudentSchedule> GetStudentSchedule(int studentId, DateTime startDate, DateTime endDate);
    Result<Section> GetSectionById(int sectionId);
    Result<List<Section>> GetSections();
    Result<List<Section>> GetStudentSections(int studentId);
    Result<List<Section>> GetInstructorSections(int instructorId);
    Result<List<StudentSection>> GetStudentsBySection(int sectionId);
    Result AddSection(Section section);
    Result UpdateSection(Section section);
    Result DeleteSection(Section section);
    Result AddStudentSection(StudentSection studentSection);
    Result UpdateStudentSection(StudentSection studentSection);
    Result DeleteStudentSection(int studentId, int sectionId);
    Result<EnrollmentReport> GetEnrollmentReport(DateTime startDate, DateTime endDate);
}