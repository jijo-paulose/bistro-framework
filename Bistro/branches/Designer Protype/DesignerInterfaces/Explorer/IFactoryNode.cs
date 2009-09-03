using System;
using System.Collections.Generic;
using System.Text;

namespace Bistro.Designer.Explorer
{
    public interface IFactoryNode
    {
        string FactoryName { get;}
        void Clear();
    }
}
