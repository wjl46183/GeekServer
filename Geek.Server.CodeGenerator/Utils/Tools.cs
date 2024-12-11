using System;

namespace Geek.Server.CodeGenerator.Utils
{
    public static class Tools
    {

        public static string GetNameSpace(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return fullName;
            int i = fullName.LastIndexOf('.');
            if(i < 0)
                return fullName;
            return fullName.Substring(0, i);
        }

        public static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        
        public static string GetWithoutNamespace(this string str)
        {
            string[] strArr = str.Split('.');
            return strArr[strArr.Length - 1];
        }
        
        public static int GetStringHash(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            const int prime = 31; // 常用的质数用于哈希
            int hash = 0;

            foreach (char c in input)
            {
                hash = (hash * prime) + c;
            }

            return hash;
        }

    }
}
