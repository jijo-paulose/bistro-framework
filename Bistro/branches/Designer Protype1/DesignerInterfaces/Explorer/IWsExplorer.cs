using System;
using System.Collections.Generic;
using System.Text;

namespace Bistro.Designer.Explorer
{
    public interface IWsExplorer
    {
        void PaintTreeNode(ExplorerNode node);
        void SetSelected(ExplorerNode node);
        void AddApplication(IBsApplicationDesigner application);
        void BeginUpdate();
        void EndUpdate();
    }

    public interface SWsExplorer
    {
    }
}
