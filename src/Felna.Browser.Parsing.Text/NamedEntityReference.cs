using System.Text.Json;

namespace Felna.Browser.Parsing.Text;

public static class NamedEntityReference
{
    static NamedEntityReference()
    {
        // https://html.spec.whatwg.org/multipage/named-characters.html#named-character-references
        // https://html.spec.whatwg.org/entities.json
        var manifestResourceName = "Felna.Browser.Parsing.Text.entities.json";
        var jsonStream = typeof(NamedEntityReference).Assembly.GetManifestResourceStream(manifestResourceName) ?? new MemoryStream();
        var jsonDoc = JsonDocument.Parse(jsonStream);
        var dictionary = new Dictionary<string, (int[] CodePoints, string Characters)>();
        foreach (var property in jsonDoc.RootElement.EnumerateObject())
        {
            var characters = property.Value.GetProperty("characters").GetString() ?? "";
            var codePoints = property.Value.GetProperty("codepoints").EnumerateArray().Select(element => element.GetInt32()).ToArray();
            dictionary.Add(property.Name, (codePoints, characters));
        }
        Entities = dictionary.AsReadOnly();
    }
    
    public static IReadOnlyDictionary<string, (int[] CodePoints, string Characters)> Entities { get; }
}