using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Special.Reflection;
using System.Reflection;
using Bistro.Reflection.CLRTypeInfo;

namespace Bistro.UnitTestsNew
{

    internal class TestTypeInfo : ITypeInfo
    {
        public class TestAttributeInfo : IAttributeInfo
        {



            public TestAttributeInfo(Type type)
            {
                this.type = type.FullName;
                this.properties = new ParameterCollection();
            }

            public TestAttributeInfo(Type type, params Parameter[] parameters)
            {
                this.type = type.FullName;
                this.properties = new ParameterCollection(parameters);
            }
            string type;
            ParameterCollection properties;

            #region IAttributeInfo Members

            public string Type
            {
                get { return type; }
            }

            public IAttributeProperties Properties
            {
                get { return properties; }
            }

            #endregion
        }

        public class TestFieldInfo : IFieldInfo
        {
            public TestFieldInfo(
                string name,
                string type,
                params TestAttributeInfo[] attributes)
            {
                this.name = name;
                this.type = type;
                this.attributes = attributes;
            }

            string name;
            string type;
            TestAttributeInfo[] attributes;

            #region IFieldInfo Members

            public IEnumerable<IAttributeInfo> Attributes
            {
                get { return new EnumProxy<TestAttributeInfo, IAttributeInfo>(attributes); }
            }

            public IEnumerable<IAttributeInfo> GetCustomAttributes(Type attributeType, bool inherit)
            {
                return new EnumProxy<TestAttributeInfo, IAttributeInfo>(attributes.Where(attrInfo => { return attrInfo.Type == attributeType.FullName; }));
            }



            public string Name
            {
                get { return name; }
            }

            public string Type
            {
                get { return type; }
            }

            #endregion

            #region IMemberInfo Members


            public object Coerce(object value)
            {
                throw new NotImplementedException();
            }

            public void SetValue(object instance, object value)
            {
                throw new NotImplementedException();
            }

            public object GetValue(object instance)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class TestPropertyInfo : IPropertyInfo
        {
            public TestPropertyInfo(
                string name,
                string type,
                params TestAttributeInfo[] attributes)
            {
                this.name = name;
                this.type = type;
                this.attributes = attributes;
            }
            string name;
            string type;
            TestAttributeInfo[] attributes;

            #region IPropertyInfo Members

            public IEnumerable<IAttributeInfo> Attributes
            {
                get { return new EnumProxy<TestAttributeInfo, IAttributeInfo>(attributes); }
            }

            public IEnumerable<IAttributeInfo> GetCustomAttributes(Type attributeType, bool inherit)
            {
                return new EnumProxy<TestAttributeInfo, IAttributeInfo>(attributes.Where(attrInfo => { return attrInfo.Type == attributeType.FullName; }));
            }


            public string Name
            {
                get { return name; }
            }

            public string Type
            {
                get { return type; }
            }

            #endregion

            #region IMemberInfo Members


            public object Coerce(object value)
            {
                throw new NotImplementedException();
            }

            public void SetValue(object instance, object value)
            {
                throw new NotImplementedException();
            }

            public object GetValue(object instance)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public TestTypeInfo(
            string fullName,
            TestAttributeInfo[] attributes,
            TestFieldInfo[] fields,
            TestPropertyInfo[] properties)
        {
            this.fullName = fullName;
            this.attributes = attributes;
            this.fields = fields;
            this.properties = properties;
        }
        string fullName;
        TestAttributeInfo[] attributes;
        TestFieldInfo[] fields;
        TestPropertyInfo[] properties;

        #region ITypeInfo Members

        public bool IsAbstract
        {
            get { return false; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        #region Proxies
        class EnumProxy<TSource, TTarget, TProxy> : IEnumerable<TTarget>
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
                    yield return (TTarget)Activator.CreateInstance(typeof(TProxy), item);
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }


        class EnumProxy<TSource, TTarget> : IEnumerable<TTarget>
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
                    yield return (TTarget) item;
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

        public IEnumerable<IAttributeInfo> GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new EnumProxy<TestAttributeInfo, IAttributeInfo>(attributes.Where(attrInfo => { return attrInfo.Type == attributeType.FullName; }));
        }

        public IEnumerable<IMemberInfo> GetMember(string name,
                                            BindingFlags bindingAttr)
        {
            return GetMember(name, MemberTypes.All, bindingAttr);
        }

        public IEnumerable<IMemberInfo> GetMember(string name,
                                            MemberTypes type,
                                            BindingFlags bindingAttr)
        {
            return Fields.OfType<IMemberInfo>().Union(Properties.OfType<IMemberInfo>()).Where(member => { return member.Name == name; });
        }

        public IEnumerable<IMemberInfo> GetMembers(BindingFlags bindingAttr)
        {
            return Fields.OfType<IMemberInfo>().Union(Properties.OfType<IMemberInfo>());
        }

        public IEnumerable<IAttributeInfo> Attributes
        {
            get { return new EnumProxy<TestAttributeInfo, IAttributeInfo>(attributes); }
        }

        public IEnumerable<IFieldInfo> Fields
        {
            get { return new EnumProxy<TestFieldInfo, IFieldInfo>(fields); }
        }

        public IEnumerable<IPropertyInfo> Properties
        {
            get { return new EnumProxy<TestPropertyInfo, IPropertyInfo>(properties); }
        }

        public ITypeInfo GetInterface(string p)
        {
            return null;
        }

        #endregion

        #region ITypeInfo Members


        public ConstructorInfo GetConstructor(Type[] types)
        {
            return null;
        }

        #endregion

    }
}
