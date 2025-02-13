namespace Felna.Browser.Parsing.Text;

public static class StringReference
{
    public const string DocType = "DOCTYPE";

    public const string DoubleHyphen = "--";
    
    public const string Public = "public";
    
    public const string System = "system";

    // Can this be replaced by string.Equals(x, y, StringComparison.OrdinalIgnoreCase)? 
    public static bool AsciiCaseInsensitiveEquals(string x, string y)
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