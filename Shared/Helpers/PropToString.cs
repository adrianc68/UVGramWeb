public class PropToString
{
 public static void PrintData<T>(T obj)
 {
  Type type = obj.GetType();
    System.Reflection.PropertyInfo[] properties = type.GetProperties();

    Console.WriteLine($"Propiedades de {type.Name}:");
    foreach (System.Reflection.PropertyInfo property in properties)
    {
        object value = property.GetValue(obj);
        if (value != null && value.GetType().IsArray)
        {
            Array arrayValue = (Array)value;
            Console.WriteLine($"{property.Name}:");
            foreach (object item in arrayValue)
            {
                PrintData(item);
            }
        }
        else
        {
            Console.WriteLine($"{property.Name}: {value}");
        }
    }
}
 
}