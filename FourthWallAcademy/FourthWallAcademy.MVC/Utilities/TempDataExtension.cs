using Newtonsoft.Json;

namespace FourthWallAcademy.MVC.Utilities;

public static class TempDataSerializer
{
    public static string Serialize<T>(T data)
    {
        return JsonConvert.SerializeObject(data);
    }

    public static T Deserialize<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data);
    }
}

public class TempDataExtension
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public TempDataExtension(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}