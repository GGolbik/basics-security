using System.Text.Json;

namespace GGolbik.SecurityTools.Io;

public class PascalCaseJsonNamingPolicy : JsonNamingPolicy
{
    public static readonly JsonNamingPolicy PascalCase = new PascalCaseJsonNamingPolicy();
    public override string ConvertName(string name)
    {
        if (String.IsNullOrEmpty(name))
        {
            return name;
        }
        var pascal = $"{name[0]}".ToUpperInvariant();
        return pascal + name.Substring(1);
    }
}