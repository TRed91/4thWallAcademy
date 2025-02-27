﻿using _4thWallCafe.Core.Utilities;
using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;
using FourthWallAcademy.Core.Interfaces.Services;
using FourthWallAcademy.Data.Repositories;

namespace FourthWallAcademy.App.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _repo;

    public InstructorService(IInstructorRepository repo)
    {
        _repo = repo;
    }
    
    public Result<Instructor> GetInstructorById(int id)
    {
        try
        {
            var instructor = _repo.GetInstructorById(id);
            if (instructor == null)
            {
                return ResultFactory.Fail<Instructor>("Instructor not found");
            }

            return ResultFactory.Success(instructor);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<Instructor>(ex.Message);
        }
    }

    public Result<List<Instructor>> GetInstructors()
    {
        try
        {
            var instructors = _repo.GetInstructors();
            return ResultFactory.Success(instructors);
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail<List<Instructor>>(ex.Message);
        }
    }

    public Result AddInstructor(Instructor instructor)
    {
        try
        {
            var existingInstructor = _repo.GetInstructorByAlias(instructor.Alias);
            if (existingInstructor != null)
            {
                return ResultFactory.Fail($"Instructor with alias {instructor.Alias} already exists");
            }
            instructor.HireDate = DateTime.Now;
            _repo.AddInstructor(instructor);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result UpdateInstructor(Instructor instructor)
    {
        try
        {
            var instructorToUpdate = _repo.GetInstructorById(instructor.InstructorID);
            if (instructorToUpdate == null)
            {
                return ResultFactory.Fail($"Instructor not found");
            }
            instructorToUpdate.Alias = instructor.Alias;
            
            _repo.UpdateInstructor(instructorToUpdate);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result DeleteInstructor(int id)
    {
        try
        {
            _repo.DeleteInstructor(id);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }

    public Result TerminateInstructor(int id, DateTime terminationDate)
    {
        try
        {
            var instructorToTerminate = _repo.GetInstructorById(id);
            if (instructorToTerminate == null)
            {
                return ResultFactory.Fail($"Instructor not found");
            }

            instructorToTerminate.TermDate = terminationDate;
            
            _repo.UpdateInstructor(instructorToTerminate);
            return ResultFactory.Success();
        }
        catch (Exception ex)
        {
            return ResultFactory.Fail(ex.Message);
        }
    }
}