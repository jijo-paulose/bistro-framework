using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Methods
{
    internal class DependencyGraph
    {
        class Vertex
        {
            public Vertex(DependencyGraph graph, Controller controller)
            {
                this.graph = graph;
                this.controller = controller;
                children = new List<Vertex>();
                isRoot = true;
                index = -1;
                visited = false;
            }

            public List<Vertex> Children { get { return children; } }
            Controller controller;
            List<Vertex> children;
            bool visited;
            public bool isRoot;
            public int index;
            DependencyGraph graph;

            internal int Traverse(int index)
            {
                if (visited)
                    return -1;  // we ran into a loop
                visited = true;
                if (this.index == -1)
                    graph.vertexCount++;
                this.controller.SeqNumber = this.index = ++index;
                foreach (Vertex child in children)
                    if((index = child.Traverse(index)) == -1)
                        return -1;
                visited = false;
                return index;
            }
        }

        Dictionary<ControllerType, Vertex> vertices = new Dictionary<ControllerType, Vertex>();
        int vertexCount = 0;
        public DependencyGraph(List<Controller> vertices)
        {
            foreach (Controller controller in vertices)
                this.vertices.Add(controller.Type, new Vertex(this, controller));
        }

        internal void AddEdge(ControllerType providingController, ControllerType consumingController)
        {
            Vertex endpoint = vertices[consumingController];
            Vertex startpoint = vertices[providingController];
            if (!startpoint.Children.Contains(endpoint))
            {
                startpoint.Children.Add(endpoint);
                endpoint.isRoot = false;
            }
        }

        internal bool TopologicalSort()
        {
            int index = 0;
            foreach (Vertex origin in vertices.Values)
                if (origin.isRoot)
                    if ((index = origin.Traverse(index)) == -1)
                        return false;
            return vertexCount == vertices.Count;
        }
    }
}
