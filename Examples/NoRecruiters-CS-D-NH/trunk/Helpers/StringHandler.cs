using System;
using System.Collections.Generic;
using System.Text;

namespace NoRecruiters.DataAccess
{

    /// <summary>
    /// Summary description for StringHandler
    /// </summary>
    public class StringHandler
    {
        private static StringHandler instance = new StringHandler();
        private Dictionary<char, string> characterMap = new Dictionary<char, string>();

        public static StringHandler Instance { get { return instance; } }

        private StringHandler()
        {
            characterMap.Add('@', "at");
            characterMap.Add('#', "sharp");
            characterMap.Add('$', "dollar");
            characterMap.Add('%', "percent");
            characterMap.Add('*', "star");
            characterMap.Add('+', "plus");
            characterMap.Add(':', "colon");
            characterMap.Add(',', "comma");
            characterMap.Add('.', "dot");
            characterMap.Add(' ', "_");
            characterMap.Add('(', String.Empty);
            characterMap.Add(')', String.Empty);
            characterMap.Add('{', String.Empty);
            characterMap.Add('}', String.Empty);
            characterMap.Add('|', String.Empty);
            characterMap.Add('[', String.Empty);
            characterMap.Add(']', String.Empty);
            characterMap.Add('\\', String.Empty);
            characterMap.Add('"', String.Empty);
            characterMap.Add(';', String.Empty);
            characterMap.Add('\'', String.Empty);
            characterMap.Add('<', String.Empty);
            characterMap.Add('>', String.Empty);
            characterMap.Add('?', String.Empty);
            characterMap.Add('/', String.Empty);
            characterMap.Add('`', String.Empty);
            characterMap.Add('~', String.Empty);
            characterMap.Add('!', String.Empty);
            characterMap.Add('^', String.Empty);
            characterMap.Add('&', String.Empty);
        }

        /// <summary>
        /// Retrieves a standardized representation of c
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public string Lookup(char c)
        {
            string ret = null;

            if (characterMap.TryGetValue(c, out ret))
                return ret;

            return c.ToString();
        }

        /// <summary>
        /// Replaces all non-standard characters with standardized representationts.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public string ReplaceAll(string source) {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
                result.Append(Lookup(source[i]));

            return result.ToString();
        }
    }
}