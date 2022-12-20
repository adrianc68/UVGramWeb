namespace UVGramWeb.Shared.Helpers;

public class EnumHelper
{
    public static T GetEnumValue<T>(string str) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new Exception("T must be an Enumeration type.");
        }
        T val = ((T[])Enum.GetValues(typeof(T)))[0];
        if (!string.IsNullOrEmpty(str))
        {
            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
            {
                if (enumValue.ToString().ToUpper().Equals(str.ToUpper()))
                {
                    val = enumValue;
                    break;
                }
            }
        }
        return val;
    }

}


