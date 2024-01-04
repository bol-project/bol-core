using System;

namespace Bol.Core.Serializers;

public class StringUtils
{
    public static string NumberEllipsis(string input, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
            
        var length = input.Length;
        var take = Math.Min((int)Math.Ceiling((double)input.Length / 2), maxChars);
        return $"{input.Substring(0, take)}{length-take}";
    }
}
