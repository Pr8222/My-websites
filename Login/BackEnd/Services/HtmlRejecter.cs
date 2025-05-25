using System.Text.RegularExpressions;

namespace LoginAPI.Services.HtmlRejecter;

public static class HtmlRejecter
{
    private static readonly Regex HtmlTagRegex = new Regex("<.*?>", RegexOptions.Compiled);

    public static bool ContainsHtml(string input)
    {
        return HtmlTagRegex.IsMatch(input);
    }
}
