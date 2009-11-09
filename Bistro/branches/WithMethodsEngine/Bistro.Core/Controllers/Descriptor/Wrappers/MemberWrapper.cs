using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Reflection;
using System.Reflection;

namespace Bistro.Controllers.Descriptor.Wrappers
{
    public class MemberWrapper : IMemberInfo
    {

        public MemberWrapper(MemberInfo memberInfo)
        {
            name = memberInfo.Name;
            typeName = memberInfo.ReflectedType.FullName;
        }


        private string name;
        private string typeName;


        #region IMemberInfo Members

        public string Name
        {
            get { return name; }
        }

        public string Type
        {
            get { return typeName; }
        }

        #endregion

        #region IHasAttributes Members

        public IEnumerable<IAttributeInfo> Attributes
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
