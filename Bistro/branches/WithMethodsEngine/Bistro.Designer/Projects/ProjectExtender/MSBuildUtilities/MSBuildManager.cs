using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.BuildEngine;
using System.IO;

namespace FSharp.ProjectExtender
{
    public class MSBuildManager
    {
        struct Tuple<T1, T2, T3>
        {
            public Tuple(T1 element, T2 moveBy, T3 index)
            {
                this.element = element;
                this.moveBy = moveBy;
                this.index = index;
            }
            T1 element;
            T2 moveBy;
            T3 index;
            public T1 Element { get { return element; } }
            public T2 MoveBy { get { return moveBy; } }
            public T3 Index { get { return index; } }
        }

        Project project;
        public MSBuildManager(string projectFile)
        {
            project = Engine.GlobalEngine.GetLoadedProject(projectFile);

            var item_list = new List<BuildElement>();
            var fixup_list = new List<Tuple<BuildElement, int, int>>();

            foreach (var item in GetElements(
                    n => n.Name == "Compile" || n.Name == "Content" || n.Name == "None"
                    ))
            {
                item_list.Add(item);
                int offset;
                if (int.TryParse(item.BuildItem.GetMetadata("Offset"), out offset))
                    fixup_list.Insert(0, new Tuple<BuildElement,int,int>(item, offset, item_list.Count - 1));
            }

            foreach (var item in fixup_list)
            {
                for (int i = 1; i <= item.MoveBy; i++)
                    item.Element.SwapWith(item_list[item.Index + i]);
                item_list.Remove(item.Element);
                item_list.Insert(item.Index + item.MoveBy, item.Element);
            }
        }

        public IEnumerable<BuildElement> GetElements(Predicate<BuildItem> condition)
        {
            foreach (BuildItemGroup group in project.ItemGroups)
            {
                int i = -1;
                foreach (BuildItem item in group)
                {
                    i++;
                    if (condition(item))
                        yield return new BuildElement(group, i, item);
                }
            }
        }

        internal void FixupProject()
        {

            var fixup_dictionary = new Dictionary<string, int>();
            var fixup_list = new List<Tuple<BuildElement, int, int>>();
            var itemList = new List<BuildElement>();
            int count = 0;

            foreach (BuildElement item in GetElements(
                    n => n.Name == "Compile" || n.Name == "Content" || n.Name == "None"
                    ))
            {
                item.BuildItem.RemoveMetadata("Offset");
                itemList.Add(item);
                count++;
                string path = '\\' + Path.GetDirectoryName(item.Path);
                string partial_path = path;
                int location;
                while (true)
                {
                    if (fixup_dictionary.TryGetValue(partial_path, out location))
                    {
                        int offset = count - 1 - location;
                        if (offset > 0)
                        {
                            if (item.BuildItem.Name == "Compile")
                                item.BuildItem.SetMetadata("Offset", offset.ToString());

                            fixup_list.Add(new Tuple<BuildElement, int, int>(item, offset, count - 1));
                            foreach (KeyValuePair<string, int> d_item in fixup_dictionary.ToList())
                            {
                                if (d_item.Value > location)
                                    fixup_dictionary[d_item.Key] += 1;
                            }
                        }
                        break;
                    }
                    var ndx = partial_path.LastIndexOf('\\');
                    if (ndx < 0)
                    {
                        location = count - 1;  // this is a brand new path - let us put it in the bottom
                        break;
                    }
                    partial_path = partial_path.Substring(0, ndx);
                }
                partial_path = path;
                while (true)
                {
                    fixup_dictionary[partial_path] = location + 1; // the index for the slot to put the next item in

                    var ndx = partial_path.LastIndexOf('\\');
                    if (ndx < 0)
                        break;
                    partial_path = partial_path.Substring(0, ndx);
                }
            }
            foreach (var item in fixup_list)
            {
                for (int i = 1; i <= item.MoveBy; i++)
                    item.Element.SwapWith(itemList[item.Index - i]);
                itemList.Remove(item.Element);
                itemList.Insert(item.Index - item.MoveBy, item.Element);
            }
            project.Save(project.FullFileName);
        }
    }
}
