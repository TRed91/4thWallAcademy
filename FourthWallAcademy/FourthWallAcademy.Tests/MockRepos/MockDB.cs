using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.Tests.MockRepos;

public class MockDB
{
    public List<Student> Students;
    public List<StudentSection> StudentSections;
    public List<StudentPassword> StudentPasswords;
    public List<Instructor> Instructors;
    public List<Section> Sections;
    public List<Course> Courses;

    public MockDB()
    {
        Students = new List<Student>
        {
            new Student
            {
                StudentID = 1, FirstName = "Emily", LastName = "Smith", Alias = "Solar Flare",
                DoB = new DateTime(2004, 03, 12)
            },
            new Student
            {
                StudentID = 2, FirstName = "Oliver", LastName = "Johnson", Alias = "Thunder Bolt",
                DoB = new DateTime(2005, 10, 22)
            },
            new Student
            {
                StudentID = 3, FirstName = "Sophia", LastName = "Williams", Alias = "Astral Whisper",
                DoB = new DateTime(2006, 06, 30)
            }
        };
        
        StudentPasswords = new List<StudentPassword>
        {
            new StudentPassword { PasswordID = 1, StudentID = 1, Password = "SolarFlare1234" },
            new StudentPassword { PasswordID = 2, StudentID = 2, Password = "ThunderBold1234" },
            new StudentPassword { PasswordID = 3, StudentID = 3, Password = "AstralWhisper1234" },
        };

        StudentSections = new List<StudentSection>
        {
            new StudentSection{ StudentID = 1, SectionID = 1, Absences = 2, Grade = 83 },
            new StudentSection{ StudentID = 1, SectionID = 2, Absences = 1, Grade = 86 },
            new StudentSection{ StudentID = 1, SectionID = 3, Absences = 3, Grade = 85 },
            new StudentSection{ StudentID = 2, SectionID = 2, Absences = 0, Grade = 93 },
            new StudentSection{ StudentID = 3, SectionID = 3, Absences = 5, Grade = 70 }
        };

        Sections = new List<Section>
        {
            new Section{  SectionID = 1, CourseID = 1, InstructorID = 1, 
                StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 31),
                StartTime = new TimeSpan(9, 00, 00), EndTime = new TimeSpan(9, 45, 00),
            },
            new Section{  SectionID = 2, CourseID = 2, InstructorID = 2, 
                StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 31),
                StartTime = new TimeSpan(10, 00, 00), EndTime = new TimeSpan(10, 45, 00),
            },
            new Section{  SectionID = 3, CourseID = 3, InstructorID = 3, 
                StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 31),
                StartTime = new TimeSpan(11, 00, 00), EndTime = new TimeSpan(11, 45, 00),
            }
        };

        Courses = new List<Course>
        {
            new Course{ CourseID = 1, SubjectID = 1, CourseName = "Course1", CourseDescription = "CourseDescription1", Credits = 3.00m },
            new Course{ CourseID = 2, SubjectID = 2, CourseName = "Course2", CourseDescription = "CourseDescription2", Credits = 2.00m },
            new Course{ CourseID = 3, SubjectID = 3, CourseName = "Course3", CourseDescription = "CourseDescription3", Credits = 1.00m },
        };
        
        Instructors = new List<Instructor>
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
    }
}