using FourthWallAcademy.Core.Entities;
using FourthWallAcademy.Core.Interfaces.Repositories;

namespace FourthWallAcademy.Tests.MockRepos;

public class MockInstructorRepo : IInstructorRepository
{
    private List<Instructor> _data;
    private int _index;

    public MockInstructorRepo()
    {
        _data = new List<Instructor>
        {
            new Instructor
                { InstructorID = 1, Alias = "Captain Quantum", HireDate = new DateTime(2022, 1, 15), TermDate = null },
            new Instructor
                { InstructorID = 2, Alias = "Mystic Shadow", HireDate = new DateTime(2022, 2, 1), TermDate = null },
            new Instructor
                { InstructorID = 3, Alias = "The Falconer", HireDate = new DateTime(2022, 3, 10), TermDate = null },
            new Instructor
                { InstructorID = 4, Alias = "Dr. Vortex", HireDate = new DateTime(2022, 4, 20), TermDate = new DateTime(2023, 2, 28) },
            new Instructor
                { InstructorID = 5, Alias = "Ms. Mirage", HireDate = new DateTime(2022, 6, 5), TermDate = null },
            new Instructor
                { InstructorID = 6, Alias = "Nimbus Knight", HireDate = new DateTime(2022, 8, 15), TermDate = new DateTime(2023, 1, 15) },
            new Instructor
                { InstructorID = 7, Alias = "Electra Storm", HireDate = new DateTime(2022, 9, 1), TermDate = null },
            new Instructor
                { InstructorID = 8, Alias = "Chronos", HireDate = new DateTime(2022, 10, 10), TermDate = null },
            new Instructor
                { InstructorID = 9, Alias = "Siren Song", HireDate = new DateTime(2022, 11, 20), TermDate = null },
            new Instructor
                { InstructorID = 10, Alias = "Titanium Templar", HireDate = new DateTime(2022, 12, 5), TermDate = new DateTime(2023, 3, 1) },
        };

        _index = _data.Count;
    }
    public Instructor? GetInstructorById(int id)
    {
        return _data.FirstOrDefault(i => i.InstructorID == id);
    }

    public Instructor? GetInstructorByAlias(string alias)
    {
        return _data.FirstOrDefault(i => i.Alias == alias);
    }

    public List<Instructor> GetInstructors()
    {
        return _data;
    }

    public void AddInstructor(Instructor instructor)
    {
        _data.Add(instructor);
    }

    public void UpdateInstructor(Instructor instructor)
    {
        var instructorToUpdate = _data.FirstOrDefault(i => i.InstructorID == instructor.InstructorID);
        instructorToUpdate.Alias = instructor.Alias;
        instructorToUpdate.HireDate = instructor.HireDate;
        instructorToUpdate.TermDate = instructor.TermDate;
    }

    public void DeleteInstructor(int id)
    {
        var instructorToDelete = _data.FirstOrDefault(i => i.InstructorID == id);
        _data.Remove(instructorToDelete);
    }
}