using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Models;

namespace FourthWallAcademy.Tests.MockRepos;

public class MockSectionRepo : ISectionRepository
{
    private readonly MockDB _db;

    public MockSectionRepo(MockDB db)
    {
        _db = db;
    }

    public Section? GetSectionById(int id)
    {
        return _db.Sections.FirstOrDefault(s => s.SectionID == id);
    }

    public StudentSection? GetStudentSection(int studentId, int sectionId)
    {
        return _db.StudentSections
            .FirstOrDefault(s => s.StudentID == studentId && s.SectionID == sectionId);
    }

    public List<Section> GetSections()
    {
        return _db.Sections;
    }

    public List<Section> GetSectionsByInstructorId(int instructorId)
    {
        return _db.Sections.Where(s => s.InstructorID == instructorId).ToList();
    }

    public List<Section> GetSectionsByStudent(int studentId)
    {
        var studentSections = _db.StudentSections
            .Where(s => s.StudentID == studentId)
            .ToList();

        foreach (var sSection in studentSections)
        {
            sSection.Section = _db.Sections.FirstOrDefault(s => s.SectionID == sSection.SectionID);
            sSection.Student = _db.Students.FirstOrDefault(s => s.StudentID == sSection.StudentID);
            sSection.Section.Instructor =
                _db.Instructors.FirstOrDefault(i => i.InstructorID == sSection.Section.InstructorID);
            sSection.Section.Course = _db.Courses.FirstOrDefault(c => c.CourseID == sSection.Section.CourseID);
        }
        
        var sections = studentSections
            .Select(s => s.Section)
            .ToList();

        foreach (var section in sections)
        {
            section.StudentSections = studentSections.Where(s => s.SectionID == section.SectionID).ToList();
        }
        
        return sections;
    }

    public List<StudentSection> GetStudentsBySection(int sectionId)
    {
        var studentSections = _db.StudentSections
            .Where(s => s.SectionID == sectionId)
            .ToList();

        foreach (var sSection in studentSections)
        {
            sSection.Student = _db.Students.FirstOrDefault(s => s.StudentID == sSection.StudentID);
        }
        
        return studentSections;
    }

    public void AddSection(Section section)
    {
        _db.Sections.Add(section);
    }

    public void UpdateSection(Section section)
    {
        var oldSection = _db.Sections.FirstOrDefault(s => s.SectionID == section.SectionID);
        oldSection.InstructorID = section.InstructorID;
        oldSection.CourseID = section.CourseID;
        oldSection.StartDate = section.StartDate;
        oldSection.EndDate = section.EndDate;
        oldSection.StartTime = section.StartTime;
        oldSection.EndTime = section.EndTime;
    }

    public void DeleteSection(int id)
    {
        _db.Sections.Remove(_db.Sections.FirstOrDefault(s => s.SectionID == id));
    }

    public void AddStudentSection(StudentSection studentSection)
    {
        _db.StudentSections.Add(studentSection);
    }

    public void UpdateStudentSection(StudentSection studentSection)
    {
        var oldSection = _db.StudentSections.FirstOrDefault(s => s.SectionID == studentSection.SectionID && 
                                                                 s.StudentID == studentSection.StudentID);
        oldSection.Absences = studentSection.Absences;
        oldSection.Grade = studentSection.Grade;
    }

    public void DeleteStudentSection(int studentId, int sectionId)
    {
        _db.StudentSections.Remove(_db.StudentSections
            .FirstOrDefault(s => s.StudentID == studentId && s.SectionID == sectionId));
    }

    public EnrollmentReport GetEnrollmentReport(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}