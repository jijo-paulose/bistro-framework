using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace NoRecruiters3.Helpers
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
            characterMap.Add('`', "apos");
            characterMap.Add('~', "tildae");
            characterMap.Add('!', "excl");
            characterMap.Add('@', "at");
            characterMap.Add('#', "sharp");
            characterMap.Add('$', "dollar");
            characterMap.Add('%', "percent");
            characterMap.Add('^', "caret");
            characterMap.Add('&', "amp");
            characterMap.Add('*', "star");
            characterMap.Add('(', "l_paren");
            characterMap.Add(')', "r_paren");
            characterMap.Add('+', "plus");
            characterMap.Add('{', "l_curly");
            characterMap.Add('}', "r_curly");
            characterMap.Add('|', "pipe");
            characterMap.Add('[', "l_bracket");
            characterMap.Add(']', "r_bracket");
            characterMap.Add('\\', "backslash");
            characterMap.Add(':', "colon");
            characterMap.Add('"', "quote");
            characterMap.Add(';', "semicolon");
            characterMap.Add('\'', "apos");
            characterMap.Add('<', "l_arrow");
            characterMap.Add('>', "r_arrow");
            characterMap.Add('?', "question");
            characterMap.Add(',', "coma");
            characterMap.Add('.', "dot");
            characterMap.Add('/', "forward_slash");
            characterMap.Add(' ', "_");
        }

        public string Lookup(char c)
        {
            string ret = null;

            if (characterMap.TryGetValue(c, out ret))
                return ret;

            return c.ToString();
        }
    }
}