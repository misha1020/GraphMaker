﻿using System.Collections.Generic;
using GraphMaker.Model;

namespace GraphMaker.Extensions
{
    public static partial class GraphExtensions
    {
        private static void StackDFS(Node node, Dictionary<int, bool> used)
        {
            Stack<Node> nodes = new Stack<Node>();
            nodes.Push(node);

            while (nodes.Count != 0)
            {
                var currNode = nodes.Pop();
                used[currNode.Number] = true;

                foreach (var incNode in currNode.IncidentNodes)
                {
                    if (!used[incNode.Number])
                    {
                        nodes.Push((Node)incNode);
                    }
                }
            }
        }

        public static int CCcountStackDFS(this IGraph graph)
        {
            var listOfNodes = graph.Nodes;
            Dictionary<int, bool> used = new Dictionary<int, bool>();
            for(int i =0; i<listOfNodes.Count;i++)
            {
                used.Add(listOfNodes[i].Number, false);
            }
            //bool[] used = new bool[listOfNodes[listOfNodes.Count - 1].Number + 1];
            int ConnectedComponentCount = 0;

            foreach (var node in listOfNodes)
            {
                if (!used[node.Number])
                {
                    ConnectedComponentCount++;
                    StackDFS((Node)node, used);
                }
            }
            return ConnectedComponentCount;
        }

        private static void RecursiveDFS(Node node, Dictionary<int, bool> used)
        {
            foreach (var incNode in node.IncidentNodes)
            {
                if (!used[incNode.Number])
                {
                    used[incNode.Number] = true;
                    RecursiveDFS((Node)incNode, used);
                }
            }
        }

        public static int CCcountRecursiveDFS(this IGraph graph)
        {
            var listOfNodes = graph.Nodes;
            //bool[] used = new bool[listOfNodes.Count];
            Dictionary<int, bool> used = new Dictionary<int, bool>();
            for (int i = 0; i < listOfNodes.Count; i++)
            {
                used.Add(listOfNodes[i].Number, false);
            }
            int ConnectedComponentCount = 0;

            foreach (var node in listOfNodes)
            {
                if (!used[node.Number])
                {
                    ConnectedComponentCount++;
                    used[node.Number] = true;
                    RecursiveDFS((Node)node, used);
                }
            }
            return ConnectedComponentCount;
        }

        public static List<List<INode>> GetListOfComponents(this IGraph graph)
        {
            var listOfNodes = graph.Nodes;
            Dictionary<int, bool> used = new Dictionary<int, bool>();
            for (int i = 0; i < listOfNodes.Count; i++)
            {
                used.Add(listOfNodes[i].Number, false);
            }
            var listOfComponents = new List<List<INode>>();

            foreach (var node in listOfNodes)
            {
                if (!used[node.Number])
                {
                    var component = NodeExtensions.DepthSearch(node);
                    listOfComponents.Add(component);
                    foreach (var usedNode in component)
                        used[usedNode.Number] = true;
                }
            }

            return listOfComponents;
        }
    }
}
