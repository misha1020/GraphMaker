using GraphMaker.Extensions;
using GraphMaker.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GraphMaker.Extensions.GraphExtensions;

namespace GraphMaker.Tests
{
    [TestFixture]
    internal class DjikstraTests
    {

        private GraphWithConstraints MakeGraph(int nodesCount, params (int First, int Second, int Length, int weight)[] incidentEdges)
        {
            var graph = new GraphWithConstraints();

            for (var i = 0; i < nodesCount; i++)
            {
                graph.AddNode();
            }

            var nodes = graph.Nodes;
            foreach (var (First, Second, Length, Weight) in incidentEdges)
            {
                var first = nodes[First];
                var second = nodes[Second];
                var edge = graph.AddEdge(first, second, Length, Weight);
            }

            return graph;
        }

        [Test, TestCaseSource(typeof(TestsFactory), nameof(TestsFactory.TestCases))]
        public void Test(int nodesCount, int s, int maxWeight, (int First, int Second, int Length, int Weght)[] incidentEdges, (int index, int minLen, int[] path)[] expectedPath)
        {
            var graph = MakeGraph(nodesCount, incidentEdges);

            var shortestPaths = graph.GetShortestPathDijkstra(graph.Nodes[s], maxWeight);

            var pathInEdges = new Dictionary<int, PathToNode>();

            var expected = expectedPath
                .ToDictionary(tuple => 
                    tuple.index,
                    tuple => {
                        var path = new PathToNode(new Node(tuple.index), tuple.minLen, new List<IEdge>() );
                        var expectedNodes = tuple.path;
                        for (int i = 0; i < expectedNodes.Length - 1; i++)
                        {
                            path.Path.Add(new Edge(new Node(expectedNodes[i]), new Node(expectedNodes[i + 1])));
                        }
                        return path;
                    });

            Assert.IsTrue(new ResComparer().Compare(expected, shortestPaths) == 0);
            
        }

        private class ResComparer
        { 
            public int Compare(object x, object y)
            {
                if (!(x is Dictionary<int, PathToNode> xx && y is Dictionary<int, PathToNode> yy))
                    return -1;

                if (xx.Count != yy.Count)
                    return -1;

                foreach (var key in xx.Keys)
                {
                    var path1 = xx[key];
                    
                    if (!yy.TryGetValue(key, out var path2))
                        return -1;


                    if (path1.Node.Number != path2.Node.Number)
                        return -1;

                    if (path1.Path.Count != path2.Path.Count)
                        return -1;

                    for (int i = 0; i < path1.Path.Count; i++)
                    {
                        if (path1.Path[i].First.Number != path2.Path[i].First.Number)
                            return -1;

                        if (path1.Path[i].Second.Number != path2.Path[i].Second.Number)
                            return -1;
                    }
                }

                return 0;
            }
        }

        private class TestsFactory
        {
            public static TestCaseData[] TestCases => new[]
            {
                new TestCaseData(6, 0, -1,
                    new[] {
                        (0, 1, 1, 1),
                        (0, 2, 8, 1), 
                        (0, 5, 14, 1), 
                        (1, 2, 10, 1),
                        (1, 3, 15, 1),
                        (2, 3, 11, 1), 
                        (2, 5, 2, 1), 
                        (3, 4, 6, 1), 
                        (4, 5, 9, 1), 
                    },
                    new[] {
                        (0, 0, new int[]{ 0 }),
                        (1, 1, new int[]{ 0, 1 }),
                        (2, 8, new int[]{ 0, 2 }),
                        (5, 10, new int[]{ 0, 2, 5 }),
                        (3, 16, new int[]{ 0, 1, 3 }),
                        (4, 19, new int[]{ 0, 2, 5, 4 }),
                    }),

                new TestCaseData(6, 0, 2,
                    new[] {
                        (0, 1, 1, 2),
                        (0, 2, 8, 2),
                        (0, 5, 14, 2),
                        (1, 2, 10, 2),
                        (1, 3, 15, 2),
                        (2, 3, 11, 2),
                        (2, 5, 2, 1), // weight > 1. can't get this way
                        (3, 4, 6, 2),
                        (4, 5, 9, 2),
                    },
                    new[] {
                        (0, 0, new int[]{ 0 }),
                        (1, 1, new int[]{ 0, 1 }),
                        (2, 8, new int[]{ 0, 2 }),
                        (5, 14, new int[]{ 0, 5 }),
                        (3, 16, new int[]{ 0, 1, 3 }),
                        (4, 23, new int[]{ 0, 1, 3, 4 }),
                    }),

                new TestCaseData(6, 0, -1,
                    new[] {
                        (0, 1, 3, 1),
                        (0, 2, 9, 1),
                        (0, 5, 14, 1),
                        (1, 2, 2, 1),
                        (1, 3, 15, 1),
                        (2, 3, 11, 1),
                        (2, 5, 2, 1),
                        (3, 4, 6, 1),
                        (4, 5, 9, 1),
                    },
                    new[] {
                        (0, 0, new int[]{ 0 }),
                        (1, 3, new int[]{ 0, 1 }),
                        (2, 5, new int[]{ 0, 1, 2 }),
                        (5, 7, new int[]{ 0, 1, 2, 5 }),
                        (3, 16, new int[]{ 0, 1, 2, 3 }),
                        (4, 16, new int[]{ 0, 1, 2, 5, 4 }),
                    }),

                 new TestCaseData(6, 0, 3,
                    new[] {
                        (0, 1, 3, 4),
                        (0, 2, 9, 4),
                        (0, 5, 14, 5),
                        (1, 2, 2, 1),
                        (1, 3, 15, 3),
                        (2, 3, 11, 5),
                        (2, 5, 2, 6),
                        (3, 4, 6, 4),
                        (4, 5, 9, 5),
                    },
                    new[] {
                        (0, 0, new int[]{ 0 }),
                        (1, 3, new int[]{ 0, 1 }),
                        (2, 9, new int[]{ 0, 2 }),
                        (5, 11, new int[]{ 0, 2, 5 }),
                        (3, 18, new int[]{ 0, 1, 3 }),
                        (4, 20, new int[]{ 0, 2, 5, 4 }),
                    }),

                 new TestCaseData(6, 0, 3,
                    new[] {
                        (0, 1, 3, 4),
                        (0, 2, 9, 4),
                        (0, 5, 14, 5),
                        (1, 2, 2, 1),
                        (1, 3, 15, 3),
                        (2, 3, 11, 5),
                        (2, 5, 2, 2),
                        (3, 4, 6, 4),
                        (4, 5, 9, 5),
                    },
                    new[] {
                        (0, 0, new int[]{ 0 }),
                        (1, 3, new int[]{ 0, 1 }),
                        (2, 9, new int[]{ 0, 2 }),
                        (5, 14, new int[]{ 0, 5 }),
                        (3, 18, new int[]{ 0, 1, 3 }),
                        (4, 23, new int[]{ 0, 5, 4 }),
                    }),

                 new TestCaseData(6, 0, 3,
                    new[] {
                        (0, 1, 3, 4),
                        (0, 2, 9, 4),
                        (0, 5, 14, 5),
                        (1, 2, 2, 1),
                        (1, 3, 15, 3),
                        (2, 3, 11, 5),
                        (2, 5, 2, 2),
                        (3, 4, 2, 4),
                        (4, 5, 9, 5),
                    },
                    new[] {
                        (0, 0, new int[]{ 0 }),
                        (1, 3, new int[]{ 0, 1 }),
                        (2, 9, new int[]{ 0, 2 }),
                        (5, 14, new int[]{ 0, 5 }),
                        (3, 18, new int[]{ 0, 1, 3 }),
                        (4, 20, new int[]{ 0, 1, 3, 4 }),
                    }),
            };
        }
    }
}
