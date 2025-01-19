namespace FourthWallAcademy.Core.Models;

public class PowersReport
{
    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    public int AvgRating { get; set; }
}

public class PowerTypeReport
{
    public int PowerTypeID { get; set; }
    public string PowerTypeName { get; set; }
    public List<PowerRatings> PowerRatings { get; set; } = new List<PowerRatings>();
}

public class PowerRatings
{
    public string PowerName { get; set; }
    public int PowerTypeID { get; set; }
    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    public int AvgRating { get; set; }
}