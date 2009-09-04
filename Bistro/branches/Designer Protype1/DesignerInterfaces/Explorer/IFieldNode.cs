using System;
using System.Collections.Generic;
using System.Text;

namespace Bistro.Designer.Explorer
{
    public interface IFieldNode
    {
        string FactoryName { get;}
        bool IsCollection { get;}
        string Name { get;}
        string GetDatamemberName();
        bool IsTypeEligible(Type type);
    }
}
