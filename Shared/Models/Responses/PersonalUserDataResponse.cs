using UVGramWeb.Shared.Data;

public class PersonalUserDataResponse : UserDataResponse
{
 public int? Id_Educational_Program { get; set; }
 public int? Id_Region { get; set; }
 public int? Id_Faculty { get; set; }
  public GenderType Gender { get; set; }
}