using System.Net;

namespace Core.Email;

public static class EmailHtml
{
    public static string Encode(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
