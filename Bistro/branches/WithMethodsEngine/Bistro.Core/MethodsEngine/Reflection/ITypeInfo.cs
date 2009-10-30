using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.MethodsEngine.Reflection
{
    public interface IAttributeInfo
    {
        string Type { get; }
        IAttributeParameters Parameters { get; }
    }

    public interface IAttributeParameters
    {
        int Count { get; }
        IAttributeParameter this[int index] { get; }
        IAttributeParameter this[string name] { get; }
    }

    public interface IAttributeParameter
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
        IEnumerable<IAttributeInfo> Attributes { get; }
    }



    public interface IMemberInfo : IHasAttributes
    {
        string Name { get; }
        string Type { get; }
    }


    public interface ITypeInfo : IHasAttributes
    {
        bool IsAbstract { get; }
        string FullName { get; }
        IEnumerable<IFieldInfo> Fields { get; }
        IEnumerable<IPropertyInfo> Properties { get; }

    }
}
