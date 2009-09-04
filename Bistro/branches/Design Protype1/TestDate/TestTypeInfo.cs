using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods.Reflection;

namespace Bistro.Tests
{
    public class TestTypeInfo : ITypeInfo
    {
        public class TestAttributeInfo : IAttributeInfo
        {
            public class Parameter : IAttributeParameter
            {
                public Parameter(string value)
                    : this (null, value)
                {
                }

                public Parameter(string name, string value)
                {
                    this.name = name;
                    this.value = value;
                }
                string name;
                string value;

                public string Name { get { return name; } }

                #region IAttributeParameter Members

                public string AsString()
                {
                    return value;
                }

                #endregion
            }

            class ParameterCollection : IAttributeParameters
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
                        Parameter result = new Parameter(null);
                        d.TryGetValue(name, out result);
                        return result;
                    }
                }

                #endregion
            }

            public TestAttributeInfo(Type type)
            {
                this.type = type.FullName;
                this.parameters = new ParameterCollection();
            }

            public TestAttributeInfo(Type type, params Parameter[] parameters)
            {
                this.type = type.FullName;
                this.parameters = new ParameterCollection(parameters);
            }
            string type;
            ParameterCollection parameters;

            #region IAttributeInfo Members

            public string Type
            {
                get { return type; }
            }

            public IAttributeParameters Parameters
            {
                get { return parameters; }
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

            public string Name
            {
                get { return name; }
            }

            public string Type
            {
                get { return type; }
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

            public string Name
            {
                get { return name; }
            }

            public string Type
            {
                get { return type; }
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

        public string FullName
        {
            get { return fullName; }
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

        #endregion
    }
}
