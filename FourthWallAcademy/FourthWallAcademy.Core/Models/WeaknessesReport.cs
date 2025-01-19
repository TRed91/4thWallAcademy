namespace FourthWallAcademy.Core.Models;

public class WeaknessesReport
{
    public int MinRiskLv { get; set; }
    public int MaxRiskLv { get; set; }
    public int AvgRiskLv { get; set; }
}

public class WeaknessTypeReport
{
    public int WeaknessTypeID { get; set; }
    public string WeaknessTypeName { get; set; }
    public List<WeaknessRiskLvs> WeaknessRiskLvs { get; set; } = new List<WeaknessRiskLvs>();
}

public class WeaknessRiskLvs
{
    public string WeaknessName { get; set; }
    public int WeaknessTypeID { get; set; }
    public int MinRiskLv { get; set; }
    public int MaxRiskLv { get; set; }
    public int AvgRiskLv { get; set; }
}