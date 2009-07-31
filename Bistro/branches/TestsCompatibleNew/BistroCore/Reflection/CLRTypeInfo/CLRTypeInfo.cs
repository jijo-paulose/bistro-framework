using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers;
using Bistro.Special.Reflection;
using System.Reflection;

namespace Bistro.Reflection.CLRTypeInfo
{


    internal class CLRTypeInfo : ITypeInfo
    {
        public CLRTypeInfo(Type clrType)
        {
            systemType = clrType;
            attributes = clrType.GetCustomAttributes(false);


        }

        #region Private members
        private Type systemType;

        private object[] attributes;

        #endregion

        #region SubObjects Interfaces implementation

        internal class CLRAttributeInfo : IAttributeInfo
        {
            public CLRAttributeInfo(object attrObj)
            {
                Type ot = attrObj.GetType();
                type = ot.FullName;
                List<Parameter> plist = new List<Parameter>();
                // Using object here for purpose of Enums usage
                System.Reflection.PropertyInfo[] pis = ot.GetProperties();
                foreach (System.Reflection.PropertyInfo pi in pis)
                {
                    if (pi.CanRead)
                    {
                        string name = pi.Name;
                        object oval = pi.GetValue(attrObj, null);
                        plist.Add(new Parameter(name, oval));
                    }
                }
                parameters = new ParameterCollection(plist);
            }
            #region private members
            private string type;
            private ParameterCollection parameters;
            #endregion


            #region IAttributeInfo Members

            public string Type
            {
                get { return type; }
            }

            public IAttributeParameters Properties
            {
                get { return parameters; }
            }

            #endregion
        }

        internal class CLRMemberInfo : IMemberInfo
        {

            public CLRMemberInfo(MemberInfo memberItem)
            {
                systemMember = memberItem;

                FieldInfo field = (systemMember as FieldInfo);
                PropertyInfo property = (systemMember as PropertyInfo);
                if (field != null)
                {
                    systemType = field.FieldType;
                }
                else if (property != null)
                {
                    systemType = property.PropertyType;
                }

                attributes = systemMember.GetCustomAttributes(false);

            }
            #region Private members

            protected object[] attributes;
            protected MemberInfo systemMember;
            protected Type systemType;

            #endregion

            #region IMemberInfo Members

            public string Name
            {
                get { return systemMember.Name; }
            }

            public string Type
            {
                get
                {

                    return systemType.FullName;

                }
            }

            public object Coerce(object value)
            {
                if (systemType.IsAssignableFrom(value.GetType()))
                    return value;

                return Convert.ChangeType(value, systemType);
            }

            #endregion

            #region IHasAttributes Members

            public IEnumerable<IAttributeInfo> Attributes
            {
                get { return new EnumProxy<object,IAttributeInfo,CLRAttributeInfo>(attributes); }
            }

            public IEnumerable<IAttributeInfo> GetCustomAttributes(Type attributeType, bool inherit)
            {
                return new EnumProxy<object, IAttributeInfo, CLRAttributeInfo>(systemMember.GetCustomAttributes(attributeType,inherit));
            }

            #endregion

            #region IMemberInfo Members

            public object GetValue(object instance)
            {
                PropertyInfo pInfo = systemMember as PropertyInfo;
                if (pInfo == null)
                {
                    FieldInfo fInfo = (FieldInfo)systemMember;
                    return fInfo.GetValue(instance);
                }
                else
                {
                    if (!pInfo.CanRead)
                        return null;
                    else
                        return pInfo.GetValue(instance, null);
                }

            }

            public void SetValue(object instance, object value)
            {
                PropertyInfo pInfo = systemMember as PropertyInfo;
                if (pInfo == null)
                {
                    FieldInfo fInfo = (FieldInfo)systemMember;
                    fInfo.SetValue(instance, value);
                }
                else
                {
                    if (pInfo.CanWrite)
                        pInfo.SetValue(instance, value, null);
                }
            }

            #endregion
        }

        internal class CLRPropertyInfo: CLRMemberInfo, IPropertyInfo
        {
            public CLRPropertyInfo(MemberInfo memberItem) : base(memberItem) { }
        }
        internal class CLRFieldInfo : CLRMemberInfo, IFieldInfo
        {
            public CLRFieldInfo(MemberInfo memberItem) : base(memberItem) { }
        }
        #endregion

        #region ITypeInfo Members

        public bool IsAbstract
        {
            get { return systemType.IsAbstract; }
        }

        public string FullName
        {
            get { return systemType.FullName; }
        }

        public IEnumerable<IFieldInfo> Fields
        {
            get { return new EnumProxy<MemberInfo,IFieldInfo,CLRFieldInfo>(systemType.GetFields()); }
        }

        public IEnumerable<IPropertyInfo> Properties
        {
            get { return new EnumProxy<MemberInfo, IPropertyInfo, CLRPropertyInfo>(systemType.GetProperties()); }
        }

        public IEnumerable<IMemberInfo> GetMember(string name, System.Reflection.BindingFlags bindingAttr)
        {
            return new EnumProxy<MemberInfo, IMemberInfo, CLRMemberInfo>(systemType.GetMember(name, bindingAttr));
        }

        public IEnumerable<IMemberInfo> GetMember(string name, System.Reflection.MemberTypes type, System.Reflection.BindingFlags bindingAttr)
        {
            return new EnumProxy<MemberInfo, IMemberInfo, CLRMemberInfo>(systemType.GetMember(name, type, bindingAttr));
        }

        public IEnumerable<IMemberInfo> GetMembers(System.Reflection.BindingFlags bindingAttr)
        {
            return new EnumProxy<MemberInfo, IMemberInfo, CLRMemberInfo>(systemType.GetMembers(bindingAttr));
        }

        public ConstructorInfo GetConstructor(Type[] types)
        {
            return systemType.GetConstructor(types);
        }

        public ITypeInfo GetInterface(string name)
        {
            Type retInterface = systemType.GetInterface(name);
            if (retInterface != null)
                return new CLRTypeInfo(retInterface);
            return null;
        }

        #endregion

        #region IHasAttributes Members

        public IEnumerable<IAttributeInfo> Attributes
        {
            get { return new EnumProxy<object,IAttributeInfo,CLRAttributeInfo>(attributes); }
        }

        public IEnumerable<IAttributeInfo> GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new EnumProxy<object, IAttributeInfo, CLRAttributeInfo>(systemType.GetCustomAttributes(attributeType,inherit));
        }

        #endregion

    }

    #region Proxies
    internal class EnumProxy<TSource, TTarget, TProxy> : IEnumerable<TTarget>
        where TSource : class
        where TTarget : class
        where TProxy : TTarget
    {
        IEnumerable<TSource> list;

        public EnumProxy(IEnumerable<TSource> list)
        {
            this.list = list;
        }

        #region IEnumerable<TTarget> Members

        public IEnumerator<TTarget> GetEnumerator()
        {
            foreach (TSource item in list)
                yield return (TTarget)Activator.CreateInstance(typeof(TProxy), (TSource)item);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }


    internal class EnumProxy<TSource, TTarget> : IEnumerable<TTarget>
        where TSource : TTarget
        where TTarget : class
    {
        IEnumerable<TSource> list;

        public EnumProxy(IEnumerable<TSource> list)
        {
            this.list = list;
        }

        #region IEnumerable<TTarget> Members

        public IEnumerator<TTarget> GetEnumerator()
        {
            foreach (TSource item in list)
                yield return (TTarget)item;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
    #endregion



    #region Attribute Parameters
    public class Parameter : IAttributeParameter
    {
        public Parameter(object value)
            : this(null, value)
        {
        }

        public Parameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
        string name;
        object value;

        public string Name { get { return name; } }

        #region IAttributeParameter Members

        public string AsString()
        {
            //return value.ToString();
            if (value == null)
                return null;
            else
                return value.ToString();
        }

        public bool? AsNBoolean() { return AsNBoolean(null); }

        public bool? AsNBoolean(bool? @default)
        {
            if (value != null)
                if (value is bool)
                    return (bool)value;
            return @default;
        }

        public int? AsNInt32() { return AsNInt32(null); }

        public int? AsNInt32(int? @default)
        {
            if (value != null)
                if (value is int)
                    return (int)value;
            return @default;
        }

        public Enum AsEnum() { return AsEnum(null); }
        public Enum AsEnum(Enum @default)
        {
            if (value != null)
                if (value is Enum)
                    return (Enum)value;
            return @default;
        }

        #endregion
    }

    public class ParameterCollection : IAttributeParameters
    {
        public ParameterCollection()
            : this(new Parameter[] { })
        {
        }

        public ParameterCollection(Parameter[] parameters)
        {
            this.parameters = parameters;
            foreach (Parameter p in parameters)
                if (p.Name != null)
                    d.Add(p.Name, p);
        }

        public ParameterCollection(IEnumerable<Parameter> parameters)
        {
            this.parameters = parameters.ToArray();
            foreach (Parameter p in parameters)
                if (p.Name != null)
                    d.Add(p.Name, p);
        }

        Parameter[] parameters;
        Dictionary<string, Parameter> d = new Dictionary<string, Parameter>();

        #region IAttributeParameters Members

        public int Count
        {
            get { return parameters.Length; }
        }

        public IAttributeParameter this[int index]
        {
            get { return parameters[index]; }
        }

        public IAttributeParameter this[string name]
        {
            get
            {
                Parameter result = null;
                if (!d.TryGetValue(name, out result))
                {
                    result = new Parameter(null);
                }
                return result;
            }
        }

        #endregion
    }
    #endregion

}
