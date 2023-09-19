namespace Felna.Browser.DocumentParsers.TextReferences;

internal static class StringReference
{
    internal const string DocType = "DOCTYPE";

    internal const string DoubleHyphen = "--";
    
    internal const string Public = "public";
    
    internal const string System = "system";

    // Can this be replaced by string.Equals(x, y, StringComparison.OrdinalIgnoreCase)? 
    internal static bool AsciiCaseInsensitiveEquals(string x, string y)
    {
        if (x.Length != y.Length)
            return false;

        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] == y[i]) 
                continue;
            
            var lowX = CharacterReference.ToAsciiLower(x[i]);
            var lowY = CharacterReference.ToAsciiLower(y[i]);

            if (lowX != lowY)
                return false;
        }

        return true;
    }
}