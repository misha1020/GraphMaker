using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMaker.Model
{
    internal class GraphWithConstraints : Graph
    {
        public IEdge AddEdge(INode first, INode second, int length = 1, int weight = 0)
        {
            var isIncident = first.IncidentNodes.Contains(second);
            var edge = Node.Connect(first, second, length, weight);
            edge = new EdgeWithConstraints(edge, weight);
            if (!isIncident)
            {
                OnChanged(GraphOperation.AddEdge, edge);
            }
            return edge;
        }
    }
}
