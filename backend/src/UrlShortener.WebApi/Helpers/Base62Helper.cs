using System.Text;

namespace UrlShortener.WebApi.Helpers;

public static class Base62Helper 
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    
    public static string Encode(long value)
    {
        if (value == 0) 
            return Alphabet[0].ToString();
        
        var result = new StringBuilder();
        while (value > 0)
        {
            int remainder = (int)(value % 62);
            result.Insert(0, Alphabet[remainder]);
            value /= 62;
        }
        return result.ToString();
    }

    public static long Decode(string base62)
    {
        long result = 0;
        foreach (char c in base62)
        {
            result = result * 62 + Alphabet.IndexOf(c);
        }
        return result;
    }
}