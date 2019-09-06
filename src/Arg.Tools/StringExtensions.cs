using System;
using System.Collections.Generic;
using System.Text;

namespace Arg.Tools.Ascii
{
    public static class StringExtensions
    {
        public static string Reverse(this string str)
        {
            char[] chars = str.ToCharArray();
            char[] result = new char[chars.Length];
            for (int i = 0, j = str.Length - 1; i < str.Length; i++, j--)
            {
                result[i] = chars[j];
            }
            return new string(result);
        }

        public static bool IsAllDigits(this string str)
        {
            foreach (var c in str)
                if (!char.IsDigit(c))
                    return false;
                
            return true;
        }
    }
}
