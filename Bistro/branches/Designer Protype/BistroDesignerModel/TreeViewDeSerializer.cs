using System;
using System.Xml;
using System.Windows.Forms;
using System.Text;

namespace TreeViewSerialization
{
	/// <summary>
	/// Summary description for TreeViewDeSerializer.
	/// </summary>
	public class TreeViewDeSerializer
	{
		
		// Xml tag for node, e.g. 'node' in case of <node></node>
		private const string XmlNodeTag = "node";

        // Xml attributes for node e.g. <node text="DataAccessControl" imageindex="6" tag="Controller"></node>
		private const string XmlNodeTextAtt = "text";
		private const string XmlNodeTagAtt = "tag";
        private const string XmlNodeMarkAtt = "mark";
        private const string XmlNodeImageIndexAtt = "imageindex";

		public TreeViewDeSerializer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void DeserializeTreeView(TreeView treeView, string fileName)
		{
			XmlTextReader reader = null;
			try
			{
                // disabling re-drawing of treeview till all nodes are added
				treeView.BeginUpdate();				
				reader =
					new XmlTextReader(fileName);

				TreeNode parentNode = null;
				
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{	
						if (reader.Name == XmlNodeTag)
						{
							TreeNode newNode = new TreeNode();
							bool isEmptyElement = reader.IsEmptyElement;

                            // loading node attributes
							int attributeCount = reader.AttributeCount;
							if (attributeCount > 0)
							{
								for (int i = 0; i < attributeCount; i++)
								{
									reader.MoveToAttribute(i);
									SetAttributeValue(newNode, reader.Name, reader.Value);
                              	}								
							}

                            // add new node to Parent Node or TreeView
                            if(parentNode != null)
                                parentNode.Nodes.Add(newNode);
                            else
                                treeView.Nodes.Add(newNode);

                            // making current node 'ParentNode' if its not empty
							if (!isEmptyElement)
							{
                                parentNode = newNode;
							}
														
						}						                    
					}
                    // moving up to in TreeView if end tag is encountered
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (reader.Name == XmlNodeTag)
						{
							parentNode = parentNode.Parent;
						}
					}
					else if (reader.NodeType == XmlNodeType.XmlDeclaration)
					{ //Ignore Xml Declaration                    
					}
					else if (reader.NodeType == XmlNodeType.None)
					{
						return;
					}
					else if (reader.NodeType == XmlNodeType.Text)
					{
						parentNode.Nodes.Add(reader.Value);
					}

				}
			}
			finally
			{
                // enabling redrawing of treeview after all nodes are added
				treeView.EndUpdate();      
                reader.Close();	
			}
		}

		/// <summary>
		/// Used by Deserialize method for setting properties of TreeNode from xml node attributes
		/// </summary>
		/// <param name="node"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		private void SetAttributeValue(TreeNode node, string propertyName, string value)
		{
            if (propertyName == XmlNodeMarkAtt) {
                node.BackColor = System.Drawing.Color.FromName(value);
            }
			if (propertyName == XmlNodeTextAtt)
			{                
				node.Text = value;
			}
			else if (propertyName == XmlNodeImageIndexAtt) 
			{
				node.ImageIndex = int.Parse(value);
                node.SelectedImageIndex = int.Parse(value);
			}
			else if (propertyName == XmlNodeTagAtt)
			{
				node.Tag = value;
                node.ContextMenuStrip = GetContextMenu(new ContextMenuStrip(), value); 
			}

		}

       
        protected ContextMenuStrip GetContextMenu(ContextMenuStrip contextMenu, string mode)
        {
            if (null == contextMenu ||string.IsNullOrEmpty(mode))
                return null;
            if (mode == "Binding")
                contextMenu.Items.Add(new ToolStripMenuItem("Bindings", null, new EventHandler(ShowBindings), "Bindings"));
            if (mode == "Controller")
                contextMenu.Items.Add(new ToolStripMenuItem("Properties", null, new EventHandler(ShowProperties), "Properties"));
            if (mode == "Resource")
                contextMenu.Items.Add(new ToolStripMenuItem("Resource", null, new EventHandler(ShowResource), "Resource"));
            contextMenu.Items.Add(new ToolStripMenuItem("Show Source", null, new EventHandler(ShowSource), "ShowSource"));

            return contextMenu;
        }

        protected void ShowBindings(object sender, EventArgs args)
        {
            MessageBox.Show("Binding Message");
        }

        protected void ShowProperties(object sender, EventArgs args)
        {
            MessageBox.Show("Properties Message");
        }

        protected void ShowSource(object sender, EventArgs args)
        {
            MessageBox.Show("Source Message");
        }

        protected void ShowResource(object sender, EventArgs args)
        {
            MessageBox.Show("Resource Message");
        }
	}
}
