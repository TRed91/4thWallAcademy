﻿namespace FourthWallAcademy.Core.Entities;

public class Student
{
    public int StudentID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Alias  { get; set; }
    public DateTime DoB { get; set; }
    
    public List<StudentPower> StudentPowers { get; set; }
    public List<StudentWeakness> StudentWeaknesses { get; set; }
    public List<StudentSection> StudentSections { get; set; }
}