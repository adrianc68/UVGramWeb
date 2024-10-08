using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace UVGramWeb.Shared.Helpers;
public static class FormatStringHelper
{
  private static Dictionary<double, Func<double, string>> sm_Dict = null;

  private static Dictionary<double, Func<double, string>> DictionarySetup()
  {
    var dict = new Dictionary<double, Func<double, string>>();
    dict.Add(0.75, (mins) => "menos de un min");
    dict.Add(45, (mins) => string.Format("{0} min", Math.Round(mins)));
    dict.Add(90, (mins) => "menos de una hora");
    dict.Add(1440, (mins) => string.Format("{0} horas", Math.Round(Math.Abs(mins / 60))));
    dict.Add(2880, (mins) => "1 dia");
    dict.Add(43200, (mins) => string.Format("{0} dias", Math.Floor(Math.Abs(mins / 1440))));
    dict.Add(86400, (mins) => "menos de un mes");
    dict.Add(525600, (mins) => string.Format("{0} meses", Math.Floor(Math.Abs(mins / 43200))));
    dict.Add(1051200, (mins) => "menos de un año");
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
  public static DateTime ConvertToTimeZone(DateTime utcDateTime, string timeZoneId)
  {
    if (utcDateTime.Kind != DateTimeKind.Utc)
    {
      utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
    }

    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
  }

  public static string FormatDescriptionBreakLines(String description)
  {
    if (string.IsNullOrWhiteSpace(description))
      return string.Empty;

    string normalizedText = description.Replace("\r\n", "\n");
    string singleLineText = System.Text.RegularExpressions.Regex.Replace(normalizedText, @"(\n\s*){2,}", "\n");
    string formattedText = singleLineText.Replace("\n", "<br>");
    return HtmlEncoder.Default.Encode(formattedText).Replace("&lt;br&gt;", "<br>");

  }

}




