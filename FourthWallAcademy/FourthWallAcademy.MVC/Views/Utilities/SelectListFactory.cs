using Microsoft.AspNetCore.Mvc.Rendering;

namespace FourthWallAcademy.MVC.Views.Utilities;

public class SelectListFactory
{
    public static SelectList CreateAlphabetSelectList()
    {
        return new SelectList(new List<SelectListItem>
        {
            new SelectListItem("A", "A"),
            new SelectListItem("B", "B"),
            new SelectListItem("C", "C"),
            new SelectListItem("D", "D"),
            new SelectListItem("E", "E"),
            new SelectListItem("F", "F"),
            new SelectListItem("G", "G"),
            new SelectListItem("H", "H"),
            new SelectListItem("I", "I"),
            new SelectListItem("J", "J"),
            new SelectListItem("K", "K"),
            new SelectListItem("L", "L"),
            new SelectListItem("M", "M"),
            new SelectListItem("N", "N"),
            new SelectListItem("O", "O"),
            new SelectListItem("P", "P"),
            new SelectListItem("Q", "Q"),
            new SelectListItem("R", "R"),
            new SelectListItem("S", "S"),
            new SelectListItem("T", "T"),
            new SelectListItem("U", "U"),
            new SelectListItem("V", "V"),
            new SelectListItem("W", "W"),
            new SelectListItem("X", "X"),
            new SelectListItem("Y", "Y"),
            new SelectListItem("Z", "Z"),
        }, "Value", "Text");
    }
}