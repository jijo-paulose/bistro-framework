/****************************************************************************
 * 
 *  Bistro Framework Copyright © 2003-2009 Hill30 Inc
 *
 *  This file is part of Bistro Framework.
 *
 *  Bistro Framework is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Bistro Framework is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Bistro Framework.  If not, see <http://www.gnu.org/licenses/>.
 *  
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Reflection;
using Bistro.Controllers.Descriptor;

namespace Bistro.MethodsEngine
{
    /// <summary>
    /// Class, implementing topological sorting algorithm for Bind Points.
    /// </summary>
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
            public Vertex(DependencyGraph graph, IMethodsBindPointDesc bindPoint)
            {
                this.graph = graph;
                this.bindPoint = bindPoint;
                children = new List<Vertex>();
                isRoot = true;
                index = -1;
                visited = false;
            }

            public List<Vertex> Children { get { return children; } }
            IMethodsBindPointDesc bindPoint;
            List<Vertex> children;
            bool visited;
            public bool isRoot;
            public int index;
            DependencyGraph graph;

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


        /// <summary>
        /// Dictionary to link Bind points to vertices
        /// </summary>
        Dictionary<IMethodsBindPointDesc, Vertex> vertices = new Dictionary<IMethodsBindPointDesc, Vertex>();

        /// <summary>
        /// initial list of bind points.
        /// </summary>
        List<IMethodsBindPointDesc> listToSort;


        /// <summary>
        /// total vertices count.
        /// </summary>
        int vertexCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyGraph"/> class.
        /// </summary>
        /// <param name="vertices">List of the bind points.</param>
        public DependencyGraph(List<IMethodsBindPointDesc> vertices)
        {
            listToSort = vertices;
            foreach (IMethodsBindPointDesc bindPoint in vertices)
                this.vertices.Add(bindPoint, new Vertex(this, bindPoint));
        }

        /// <summary>
        /// Adds the edge to the graph.
        /// </summary>
        /// <param name="providingBindPoint">The providing bind point.</param>
        /// <param name="consumingBindPoint">The consuming bind point.</param>
        internal void AddEdge(IMethodsBindPointDesc providingBindPoint, IMethodsBindPointDesc consumingBindPoint)
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
        /// Sorts listToSort if that's possible
        /// </summary>
        /// <returns>true if sort succeeded, otherwise - false</returns>
        internal bool TopologicalSort()
        {
            int index = 0;
            foreach (Vertex origin in vertices.Values)
                if (origin.isRoot)
                    if ((index = origin.Traverse(index)) == -1)
                        return false;
            if (vertexCount == vertices.Count)
            {
                Comparison<IMethodsBindPointDesc> vertexCompare =
                    (left, right) => vertices[left].index.CompareTo(vertices[right].index);
                listToSort.Sort(vertexCompare);

                return true;
            }
            return false;
        }
    }
}
