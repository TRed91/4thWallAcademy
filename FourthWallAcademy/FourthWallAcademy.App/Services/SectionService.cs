using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.App.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Core.Models;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _repo;

    public SectionService(ISectionRepository repo)
    {
        _repo = repo;
    }
    
    public Result<StudentSchedule> GetStudentSchedule(int studentId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var sections = _repo.GetSectionsByStudent(studentId);
            var filteredSections = sections
                .Where(s => s.EndDate >= startDate && s.StartDate <= endDate)
                .OrderBy(s => s.StartDate)
                .ThenBy(s => s.StartTime)
                .ToList();

            var schedule = new StudentSchedule();
            
            if (filteredSections.Count > 0)
            {
                schedule = ScheduleConverter.ConvertToSchedule(sections, startDate, endDate);
            }
            else
            {
                schedule.StudentAlias = sections[0].StudentSections[0].Student.Alias;
                schedule.ScheduleDays = new List<ScheduleDay>();
            }
            
            return ResultFactory.Success(schedule);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<StudentSchedule>(ex.Message);
        }
    }

    public Result<Section> GetSectionById(int sectionId)
    {
        try
        {
            var section = _repo.GetSectionById(sectionId);
            if (section == null)
            {
                return ResultFactory.Fail<Section>("Section not found");
            }

            return ResultFactory.Success(section);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<Section>(ex.Message);
        }
    }

    public Result<List<Section>> GetStudentSections(int studentId)
    {
        try
        {
            var sections = _repo.GetSectionsByStudent(studentId);
            return ResultFactory.Success(sections);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Section>>(ex.Message);
        }
    }

    public Result<List<Section>> GetInstructorSections(int instructorId)
    {
        try
        {
            var sections = _repo.GetSectionsByInstructorId(instructorId);
            return ResultFactory.Success(sections);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Section>>(ex.Message);
        }
    }

    public Result<List<StudentSection>> GetStudentsBySection(int sectionId)
    {
        try
        {
            var students = _repo.GetStudentsBySection(sectionId);
            return ResultFactory.Success(students);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<StudentSection>>(ex.Message);
        }
    }

    public Result AddSection(Section section)
    {
        try
        {
            _repo.AddSection(section);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result UpdateSection(Section section)
    {
        try
        {
            _repo.UpdateSection(section);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeleteSection(Section section)
    {
        try
        {
            _repo.DeleteSection(section.SectionID);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }
}