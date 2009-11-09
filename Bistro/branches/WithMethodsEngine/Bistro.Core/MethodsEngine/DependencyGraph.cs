using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Reflection;

namespace Bistro.MethodsEngine
{
    internal class DependencyGraph
    {
        #region Vertex
        class Vertex
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="graph"></param>
            /// <param name="bindPoint"></param>
            public Vertex(DependencyGraph graph, IBindPointDescriptor bindPoint)
            {
                this.graph = graph;
                this.bindPoint = bindPoint;
                children = new List<Vertex>();
                isRoot = true;
                index = -1;
                visited = false;
            }

            public List<Vertex> Children { get { return children; } }
            IBindPointDescriptor bindPoint;
            List<Vertex> children;
            bool visited;
            public bool isRoot;
            public int index;
            DependencyGraph graph;
            int seqNumber;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            internal int Traverse(int index)
            {
                if (visited)
                    return -1;  // we ran into a loop
                visited = true;
                if (this.index == -1)
                    graph.vertexCount++;
//                this.seqNumber = this.index = ++index;
                this.index = ++index;
                foreach (Vertex child in children)
                    if ((index = child.Traverse(index)) == -1)
                        return -1;
                visited = false;
                return index;
            }
        }
        #endregion


        Dictionary<IBindPointDescriptor, Vertex> vertices = new Dictionary<IBindPointDescriptor, Vertex>();
        List<IBindPointDescriptor> listToSort;
        int vertexCount = 0;
        public DependencyGraph(List<IBindPointDescriptor> vertices)
        {
            listToSort = vertices;
            foreach (IBindPointDescriptor bindPoint in vertices)
                this.vertices.Add(bindPoint, new Vertex(this, bindPoint));
        }

        internal void AddEdge(IBindPointDescriptor providingBindPoint, IBindPointDescriptor consumingBindPoint)
        {
            Vertex endpoint = vertices[consumingBindPoint];
            Vertex startpoint = vertices[providingBindPoint];
            if (!startpoint.Children.Contains(endpoint))
            {
                startpoint.Children.Add(endpoint);
                endpoint.isRoot = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool TopologicalSort()
        {
            int index = 0;
            foreach (Vertex origin in vertices.Values)
                if (origin.isRoot)
                    if ((index = origin.Traverse(index)) == -1)
                        return false;
            if (vertexCount == vertices.Count)
            {
                Comparison<IBindPointDescriptor> vertexCompare =
                    (left, right) => vertices[left].index.CompareTo(vertices[right].index);
                listToSort.Sort(vertexCompare);

                return true;
            }
            return false;
        }
    }
}
