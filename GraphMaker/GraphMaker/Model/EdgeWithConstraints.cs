using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMaker.Model
{
    internal class EdgeWithConstraints : Edge
    {
        public EdgeWithConstraints(INode first, INode second, int length = 1, int weight = 0) : base(first, second, length)
        {
            MaxWeight = weight;
        }

        public EdgeWithConstraints(IEdge edge, int weight = 0) : base (edge.First, edge.Second, edge.Length)
        {
            MaxWeight = weight;
        }

        public int MaxWeight { get; set; }
    }
}
