using FourthWallAcademy.Core.Entities;

namespace FourthWallAcademy.MVC.Models;

public class CourseDetailsModel
{
    public int CourseID { get; set; }
    public string CourseName { get; set; }
    public string CourseDescription { get; set; }
    public decimal Credits { get; set; }
    public string Subject { get; set; }

    public CourseDetailsModel() { }

    public CourseDetailsModel(Course entity)
    {
        CourseID = entity.CourseID;
        CourseName = entity.CourseName;
        CourseDescription = entity.CourseDescription;
        Credits = entity.Credits;
        Subject = entity.Subject.SubjectName;
    }
}