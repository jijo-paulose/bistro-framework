using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Methods.Reflection
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
        object AsObject();
    }

    public interface IFieldInfo
    {
        IEnumerable<IAttributeInfo> Attributes { get; }
        string Name { get; }
        string Type { get; }
    }

    public interface IPropertyInfo
    {
        IEnumerable<IAttributeInfo> Attributes { get; }
        string Name { get; }
        string Type { get; }
    }

    public interface ITypeInfo
    {
        string FullName { get; }
        IEnumerable<IAttributeInfo> Attributes { get; }
        IEnumerable<IFieldInfo> Fields { get; }
        IEnumerable<IPropertyInfo> Properties { get; }
    }
}
