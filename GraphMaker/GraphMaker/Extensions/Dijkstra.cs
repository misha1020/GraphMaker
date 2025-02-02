using FluentAssertions;
using GraphMaker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMaker.Extensions
{
    public static partial class GraphExtensions
    {
        public static Dictionary<int, PathToNode> GetShortestPathDijkstra(this IGraph graph, INode start)
        {

            var nodePaths = new Dictionary<int, PathToNode>();
            nodePaths.Add(start.Number, new PathToNode(start, 0, new List<IEdge>()));

            var queue = new Queue<INode>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var currNode = queue.Dequeue();
                var currPath = nodePaths[currNode.Number];
                foreach (Edge edge in currNode.IncidentEdges)
                {
                    var node = edge.OtherNode(currNode);
                    var distance = nodePaths[currNode.Number].MinimalLenght + edge.Length;
                    
                    var path = new List<IEdge>(currPath.Path)
                        {
                            edge
                        };

                    if (!nodePaths.ContainsKey(node.Number))
                    {
                        nodePaths.Add(node.Number, new PathToNode(node, distance, path));
                        queue.Enqueue(node);
                    }
                    else 
                    {
                        if (nodePaths[node.Number].MinimalLenght > distance)
                        {
                            nodePaths[node.Number] = new PathToNode(node, distance, path);
                        }

                        //if (!nodePaths[node.Number].Viseted)
                        //    queue.Enqueue(node);
                    }

                }

                currPath.Viseted = true;
                nodePaths[currNode.Number] = currPath;
            }

            return nodePaths;
        }

    }
}
