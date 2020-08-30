using System;
namespace lol_cd_quiz
{
    public class Filter
    {
        public Filter(string attr, string path)
        {
            Attribute = attr;
            XPath = path;
        }
        public string Attribute;
        public string XPath;
    }
}
