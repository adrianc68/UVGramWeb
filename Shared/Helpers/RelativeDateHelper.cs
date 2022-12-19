using System.Collections.Generic;
using System.Linq;

namespace UVGramWeb.Shared.Helpers;
public static class RelativeDateHelper
{
    private static Dictionary<double, Func<double, string>> sm_Dict = null;

    private static Dictionary<double, Func<double, string>> DictionarySetup()
    {
        var dict = new Dictionary<double, Func<double, string>>();
        dict.Add(0.75, (mins) => "menos de un minuto");
        dict.Add(45, (mins) => string.Format("{0} minutos", Math.Round(mins)));
        dict.Add(90, (mins) => "menos de una hora");
        dict.Add(1440, (mins) => string.Format("{0} horas", Math.Round(Math.Abs(mins / 60)))); // 60 * 24
        dict.Add(2880, (mins) => "1 dia"); // 60 * 48
        dict.Add(43200, (mins) => string.Format("{0} dias", Math.Floor(Math.Abs(mins / 1440)))); // 60 * 24 * 30
        dict.Add(86400, (mins) => "menos de un mes"); // 60 * 24 * 60
        dict.Add(525600, (mins) => string.Format("{0} meses", Math.Floor(Math.Abs(mins / 43200)))); // 60 * 24 * 365 
        dict.Add(1051200, (mins) => "menos de un año"); // 60 * 24 * 365 * 2
        dict.Add(double.MaxValue, (mins) => string.Format("{0} años", Math.Floor(Math.Abs(mins / 525600))));

        return dict;
    }

    public static string ToRelativeDate(this DateTime input)
    {
        TimeSpan oSpan = DateTime.Now.Subtract(input);
        double TotalMinutes = oSpan.TotalMinutes;
        string Suffix = "hace ";

        if (TotalMinutes < 0.0)
        {
            TotalMinutes = Math.Abs(TotalMinutes);
        }

        if (null == sm_Dict)
            sm_Dict = DictionarySetup();

        return Suffix + sm_Dict.First(n => TotalMinutes < n.Key).Value.Invoke(TotalMinutes);
    }
}
