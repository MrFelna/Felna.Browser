namespace Felna.Browser.DocumentParsers.HtmlTokens;

internal abstract class HtmlToken
{
    internal List<HtmlTokenAttribute> TokenAttributes { get; } = new List<HtmlTokenAttribute>();

    // May move these methods to tests if unused in library
    internal abstract bool AreValueEqual(HtmlToken other);

    protected bool TokenAttributesEqual(HtmlToken other)
    {
        if (TokenAttributes.Count != other.TokenAttributes.Count)
            return false;

        for (var i = 0; i < TokenAttributes.Count; i++)
        {
            var x = TokenAttributes[i];
            var y = other.TokenAttributes[i];

            if (x.Name != y.Name || x.Value != y.Value)
                return false;
        }

        return true;
    }
}