using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models.InstructorsModels;

public class InstructorDetailsModel
{
    public int InstructorID { get; set; }
    public string Alias  { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TermDate { get; set; }
    
    public List<Section> Sections { get; set; }

    public InstructorDetailsModel(Instructor entity)
    {
        InstructorID = entity.InstructorID;
        Alias = entity.Alias;
        HireDate = entity.HireDate;
        TermDate = entity.TermDate;
        Sections = entity.Sections;
    }

    public Instructor ToEntity()
    {
        return new Instructor
        {
            InstructorID = InstructorID,
            Alias = Alias,
            HireDate = HireDate,
            TermDate = TermDate
        };
    }
}