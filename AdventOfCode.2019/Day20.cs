using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdventOfCode.Common;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day20
    {
        private class Graph
        {
            private enum Orientation
            { 
                Inside,
                Outside
            }

            [DebuggerDisplay("{Name} {Orientation} {Coord}")]
            private readonly struct Node : IEquatable<Node>
            {
                public readonly string Name;
                public readonly Orientation Orientation;
                public readonly IntPoint2 Coord;
                public readonly IntPoint2 LabelCoord;

                public Node(string name, Orientation orientation, IntPoint2 coord, IntPoint2 labelCoord)
                {
                    Name = name;
                    Orientation = orientation;
                    Coord = coord;
                    LabelCoord = labelCoord;
                }

                public bool Equals(Node other)
                {
                    return Name == other.Name &&
                           Orientation == other.Orientation;
                }

                public override bool Equals(object obj)
                {
                    return obj is Node node && Equals(node);
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(Name, Orientation);
                }
            }

            char[,] _map;
            IntPoint2 _bounds;
            Dictionary<Node, int> _nodes;
            (int distance, int levelDelta)[,] _graph;

            public Graph(char[,] map)
            {
                _map = map;
                _bounds = (map.GetLength(0), map.GetLength(1));
                _nodes = new Dictionary<Node, int>();

                DiscoverNodes();
                BuildGraph();
            }

            public int Part1()
            {
                int source = GetNodeIndex(GetNode("AA", Orientation.Outside));
                int sink = GetNodeIndex(GetNode("ZZ", Orientation.Outside));

                (int index, int distance) current = (source, 0);
                bool[] visited = new bool[_nodes.Count];
                PriorityQueue<(int index, int distance)> queue = new PriorityQueue<(int index, int distance)>(
                    Comparer<(int index, int distance)>.Create((lhs, rhs) =>
                    {
                        return lhs.distance - rhs.distance;
                    }));

                queue.Enqueue(current);

                while (queue.Count > 0)
                {
                    current = queue.Dequeue();

                    if (visited[current.index])
                        continue;
                    visited[current.index] = true;

                    if (current.index == sink)
                        return current.distance;
                    else
                    {
                        foreach (var adj in Adjacent(current.index))
                        {
                            if (!visited[adj.index])
                            {
                                queue.Enqueue((adj.index, current.distance + adj.distance));
                            }
                        }
                    }
                }

                throw new InvalidOperationException();
            }

            public int Part2()
            {
                int source = GetNodeIndex(GetNode("AA", Orientation.Outside));
                int sink = GetNodeIndex(GetNode("ZZ", Orientation.Outside));

                (int index, int distance, int level) current = (source, 0, 0);
                HashSet<(int index, int level)> visited = new HashSet<(int index, int level)>();

                PriorityQueue<(int index, int distance, int level)> queue = new PriorityQueue<(int index, int distance, int level)>(
                    Comparer<(int index, int distance, int level)>.Create((lhs, rhs) =>
                    {
                        return (lhs.distance + lhs.level) - (rhs.distance + rhs.level);
                    }));

                queue.Enqueue(current);

                while (queue.Count > 0)
                {
                    current = queue.Dequeue();

                    if (visited.Contains((current.index, current.level)))
                        continue;
                    visited.Add((current.index, current.level));

                    if (current.index == sink && current.level == 0)
                        return current.distance;
                    else
                    {
                        foreach (var adj in Adjacent(current.index))
                        {
                            if (current.level + adj.levelDelta < 0)
                                continue;

                            var next = (adj.index, current.distance + adj.distance, current.level + adj.levelDelta);
                            if (!visited.Contains((next.index, next.Item3)))
                                queue.Enqueue(next);
                        }
                    }
                }

                throw new InvalidOperationException();
            }

            private IEnumerable<(int index, int distance, int levelDelta)> Adjacent(int index)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    if (_graph[index, i].distance != -1)
                    {
                        yield return (i, _graph[index, i].distance, _graph[index, i].levelDelta);
                    }
                }
            }

            private void BuildGraph()
            {
                _graph = new (int, int)[_nodes.Count, _nodes.Count];

                for (int i = 0; i < _nodes.Count; i++)
                    for (int j = 0; j < _nodes.Count; j++)
                        _graph[i, j] = (-1, 0);

                foreach (var node in _nodes.Keys)
                {
                    Bfs(node);
                }
            }

            private void Bfs(in Node n)
            {
                int source = GetNodeIndex(n);
                (IntPoint2 p, int dist) current = (n.Coord, 0);
                HashSet<IntPoint2> visited = new HashSet<IntPoint2>();
                Queue<(IntPoint2, int)> toVisit = new Queue<(IntPoint2 p, int dist)>();

                toVisit.Enqueue(current);

                while (toVisit.Count > 0)
                {
                    current = toVisit.Dequeue();

                    if (visited.Contains(current.p))
                        continue;
                    visited.Add(current.p);

                    foreach (var adj in current.p.Adjacent(_bounds))
                    {
                        char c = _map[adj.X, adj.Y];
                        if (c == '.' && !visited.Contains(adj))
                            toVisit.Enqueue((adj, current.dist + 1));
                        else if (char.IsLetter(c) && !adj.Equals(n.LabelCoord))
                        {
                            Node sink = GetNode(current.p);
                            int distance = current.dist;
                            int levelDelta = 0;

                            if (sink.Name != "AA" && sink.Name != "ZZ")
                            {
                                if (sink.Orientation == Orientation.Outside)
                                    levelDelta = -1;
                                else
                                    levelDelta = 1;

                                distance++;
                                sink = GetNode(sink.Name, sink.Orientation == Orientation.Outside ? Orientation.Inside : Orientation.Outside);
                            }
                           
                            int sinkIndex = GetNodeIndex(sink);
                            _graph[source, sinkIndex] = (distance, levelDelta);
                        }
                    }
                }
            }

            private int GetNodeIndex(in Node n)
            {
                return _nodes[n];
            }

            private Node GetNode(string name, Orientation orientation)
            {
                return _nodes.Keys.Where(n => n.Name == name && n.Orientation == orientation).Single();
            }

            private Node GetNode(IntPoint2 p)
            {
                return _nodes.Keys.Where(n => n.Coord.Equals(p)).Single();
            }

            private void DiscoverNodes()
            {
                for (int j = 0; j < _bounds.Y; j++)
                {
                    for (int i = 0; i < _bounds.X; i++)
                    {
                        if (TryDiscoverNode((i, j), out Node node))
                        {
                            _nodes.Add(node, _nodes.Count);
                        }
                    }
                }
            }

            private bool TryDiscoverNode(IntPoint2 p, out Node node)
            {
                node = default;

                char first = _map[p.X, p.Y];
                if (!char.IsLetter(first))
                    return false;

                char second;

                IntPoint2 down = p + IntPoint2.UnitY;
                if (Bounds(down))
                {
                    second = _map[down.X, down.Y];
                    if (char.IsLetter(second))
                    {
                        Orientation orientation =
                            p.Y == 0 || p.Y == _bounds.Y - 2 ?
                            Orientation.Outside :
                            Orientation.Inside;

                        IntPoint2 coord;
                        IntPoint2 labelCoord;
                        if (Bounds(p - IntPoint2.UnitY) &&
                            _map[p.X, p.Y - 1] == '.')
                        {
                            coord = p - IntPoint2.UnitY;
                            labelCoord = p;
                        }
                        else if (Bounds(down + IntPoint2.UnitY) &&
                            _map[down.X, down.Y + 1] == '.')
                        {
                            coord = down + IntPoint2.UnitY;
                            labelCoord = down;
                        }
                        else
                            throw new InvalidOperationException();

                        node = new Node(new string(new[] { first, second }), orientation, coord, labelCoord);
                        return true;
                    }
                }

                IntPoint2 right = p + IntPoint2.UnitX;
                if (Bounds(right))
                {
                    second = _map[right.X, right.Y];
                    if (char.IsLetter(second))
                    {
                        Orientation orientation =
                            p.X == 0 || p.X == _bounds.X - 2 ?
                            Orientation.Outside :
                            Orientation.Inside;

                        IntPoint2 coord;
                        IntPoint2 labelCoord;
                        if (Bounds(p - IntPoint2.UnitX) &&
                            _map[p.X - 1, p.Y] == '.')
                        {
                            coord = p - IntPoint2.UnitX;
                            labelCoord = p;
                        }
                        else if (Bounds(right + IntPoint2.UnitX) &&
                            _map[right.X + 1, p.Y] == '.')
                        {
                            coord = right + IntPoint2.UnitX;
                            labelCoord = right;
                        }
                        else
                            throw new InvalidOperationException();

                        node = new Node(new string(new[] { first, second }), orientation, coord, labelCoord);
                        return true;
                    }
                }

                return false;
            }

            private bool Bounds(in IntPoint2 p)
            {
                return p.X >= 0 && p.X < _bounds.X &&
                       p.Y >= 0 && p.Y < _bounds.Y;
            }
        }

        Graph _graph;

        public Day20()
        {
            _graph = new Graph(ParseMap(File.ReadAllLines("Inputs/Day20.txt")));
        }

        [Fact]
        public void Part1()
        {
            int answer = _graph.Part1();
            Assert.Equal(692, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = _graph.Part2();
            Assert.Equal(8314, answer);
        }

        private static char[,] ParseMap(string[] lines)
        {
            int width = lines[0].Length;
            int height = lines.Length;

            char[,] map = new char[width, height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    map[i, j] = lines[j][i];

            return map;
        }
    }
}
