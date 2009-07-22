using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class TemplateMappingAttribute : Attribute
    {
        public TemplateMappingAttribute(string extension)
        {
            this.extension = extension;
        }
        private string extension;

        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }
    }
}
