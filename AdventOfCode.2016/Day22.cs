using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day22
    {
        private class Graph
        {
            public class Node
            {
                public readonly IntPoint2 Coord;
                public readonly int Size;
                public int Used { get; private set; }
                public int Avail { get; private set; }

                public Node(in IntPoint2 coord, int size, int used, int avail)
                {
                    Coord = coord;
                    Size = size;
                    Used = used;
                    Avail = avail;
                }
            }

            private readonly struct SearchNode
            {
                public readonly IntPoint2 Data;
                public readonly IntPoint2 Empty;
                public readonly IntPoint2 EmptyTarget;
                public readonly int Steps;

                public SearchNode(IntPoint2 data, IntPoint2 empty, IntPoint2 emptyTarget, int steps)
                {
                    Data = data;
                    Empty = empty;
                    EmptyTarget = emptyTarget;
                    Steps = steps;
                }
            }

            private Dictionary<IntPoint2, Node> _nodes;
            private HashSet<IntPoint2> _graph;
            private IntPoint2 _bounds;
            private IntPoint2 _initialEmpty;
            private IntPoint2 _goalData;

            public IReadOnlyCollection<Node> Nodes => _nodes.Values;

            public Graph(string[] input)
            {
                _nodes = new(input.Length);
                foreach (string s in input)
                {
                    Match match = s_Regex.Match(s);
                    if (match.Success)
                    {
                        Node n = new Node(
                            new IntPoint2(match.Groups["x"].Value, match.Groups["y"].Value),
                            int.Parse(match.Groups["size"].Value),
                            int.Parse(match.Groups["used"].Value),
                            int.Parse(match.Groups["avail"].Value));
                        _nodes.Add(n.Coord, n);
                    }
                }

                (_, _bounds) = _nodes.Keys.MinMax();

                _goalData = (_bounds.X, 0);
                _initialEmpty = _nodes.Values.Single(n => n.Used == 0).Coord;
                int maxCapacity = _nodes[_initialEmpty].Size;

                _graph = new HashSet<IntPoint2>(_nodes.Count);
                foreach (Node n in _nodes.Values)
                {
                    if (n.Used <= maxCapacity)
                        _graph.Add(n.Coord);
                }
            }

            public int MinSteps()
            {
                HashSet<(IntPoint2 goal, IntPoint2 empty, IntPoint2 emptyTarget)> visited = new();
                PriorityQueue<SearchNode> queue = new PriorityQueue<SearchNode>(Comparer<SearchNode>.Create(SearchComparsion));
                foreach (IntPoint2 adj in Adjacent(_goalData))
                {
                    queue.Enqueue(new SearchNode(
                        _goalData,
                        _initialEmpty,
                        adj,
                        0));
                }

                while (queue.Count > 0)
                {
                    SearchNode current = queue.Dequeue();

                    if (visited.Contains((current.Data, current.Empty, current.EmptyTarget)))
                        continue;
                    visited.Add((current.Data, current.Empty, current.EmptyTarget));

                    if (current.Data.Equals(IntPoint2.Zero))
                        return current.Steps;

                    if (current.Empty.Equals(current.EmptyTarget))
                    {
                        foreach (IntPoint2 adj in Adjacent(current.Empty))
                        {
                            if (!visited.Contains((current.Empty, current.Data, adj)))
                            {
                                if (!adj.Equals(current.Data))
                                {
                                    queue.Enqueue(new SearchNode(
                                        current.EmptyTarget,
                                        current.Data,
                                        adj,
                                        current.Steps + 1));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (IntPoint2 adj in Adjacent(current.Empty))
                        {
                            if (!adj.Equals(current.Data) && !visited.Contains((current.Data, adj, current.EmptyTarget)))
                            {
                                queue.Enqueue(new SearchNode(
                                    current.Data,
                                    adj,
                                    current.EmptyTarget,
                                    current.Steps + 1));
                            }
                        }
                    }
                }

                throw new InvalidOperationException();
            }

            private IEnumerable<IntPoint2> Adjacent(IntPoint2 p)
            {
                foreach (IntPoint2 adj in p.Adjacent())
                {
                    if (adj.X < 0 || adj.X > _bounds.X || adj.Y < 0 || adj.Y > _bounds.Y)
                        continue;

                    if (!_graph.Contains(adj))
                        continue;

                    yield return adj;
                }
            }

            private static int SearchComparsion(SearchNode lhs, SearchNode rhs)
            {
                return WeightedDistance(lhs) - WeightedDistance(rhs);

                static int WeightedDistance(in SearchNode n)
                {
                    return n.Steps + n.Data.Manhattan + n.EmptyTarget.Manhattan + n.Empty.Distance(n.EmptyTarget);
                }
            }

            private static Regex s_Regex = new Regex(
                @"^/dev/grid/node-x(?'x'\d+)-y(?'y'\d+)\s+(?'size'\d+)T\s+(?'used'\d+)T\s+(?'avail'\d+)T\s+\d+%$",
                RegexOptions.Compiled);
        }

        Graph _graph;


        public Day22()
        {
            _graph = new Graph(File.ReadAllLines("Inputs/Day22.txt"));
        }

        [Fact]
        public void Part1()
        {
            int count = 0;
            foreach (var a in _graph.Nodes)
            {
                foreach (var b in _graph.Nodes)
                {
                    if (a == b) continue;

                    if (a.Used != 0 && a.Used <= b.Avail)
                        count++;
                }
            }

            Assert.Equal(860, count);
        }

        [Fact]
        public void Part2()
        {
            int answer = _graph.MinSteps();
            Assert.Equal(200, answer);
        }

        [Fact]
        public void Example()
        {
            Graph graph = new Graph(new[] {
                "Filesystem            Size  Used  Avail  Use%",
                "/dev/grid/node-x0-y0   10T    8T     2T   80%",
                "/dev/grid/node-x0-y1   11T    6T     5T   54%",
                "/dev/grid/node-x0-y2   32T   28T     4T   87%",
                "/dev/grid/node-x1-y0    9T    7T     2T   77%",
                "/dev/grid/node-x1-y1    8T    0T     8T    0%",
                "/dev/grid/node-x1-y2   11T    7T     4T   63%",
                "/dev/grid/node-x2-y0   10T    6T     4T   60%",
                "/dev/grid/node-x2-y1    9T    8T     1T   88%",
                "/dev/grid/node-x2-y2    9T    6T     3T   66%"
            });

            int answer = graph.MinSteps();
            Assert.Equal(7, answer);
        }
    }
}
