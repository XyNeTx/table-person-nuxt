using System;

namespace modal_view_api.helpers;

public class CalAge
{
    public static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age)) age--;

        return age;
    }
}
