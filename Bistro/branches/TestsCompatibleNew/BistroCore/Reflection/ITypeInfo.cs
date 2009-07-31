using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Bistro.Special.Reflection
{
    public interface IAttributeInfo
    {
        string Type { get; }
        IAttributeProperties Properties { get; } 
    }

    public interface IAttributeProperties
    {
        int Count { get; }
        IAttributeProperty this[int index] { get; }
        IAttributeProperty this[string name] { get; }
    }

    public interface IAttributeProperty
    {
        string AsString();
        bool? AsNBoolean();
        int? AsNInt32();
        Enum AsEnum();
    }

    public interface IFieldInfo : IMemberInfo
    {
    }

    public interface IPropertyInfo : IMemberInfo
    {
    }

    public interface IHasAttributes
    {
        //IEnumerable<IAttributeInfo> Attributes { get; }
        IEnumerable<IAttributeInfo> GetCustomAttributes(Type attributeType, bool inherit);
    }



    public interface IMemberInfo : IHasAttributes
    {
        string Name { get; }
        string Type { get; }
        object Coerce( object value);

        void SetValue(object instance, object value);
        object GetValue(object instance);
    }


    public interface ITypeInfo : IHasAttributes
    {
        bool IsAbstract { get; }
        string FullName { get; }
        //IEnumerable<IFieldInfo> Fields { get; }
        //IEnumerable<IPropertyInfo> Properties { get; }

        IEnumerable<IMemberInfo> GetMember(string name,
                                    BindingFlags bindingAttr);

        IEnumerable<IMemberInfo> GetMember(string name,
                                            MemberTypes type,
                                            BindingFlags bindingAttr);

        IEnumerable<IMemberInfo> GetMembers(BindingFlags bindingAttr);

        ConstructorInfo GetConstructor(Type[] types);

        ITypeInfo GetInterface(string p);
    }
}
