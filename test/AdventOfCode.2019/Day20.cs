using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode._2019;

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
            public readonly IntVec2 Coord;
            public readonly IntVec2 LabelCoord;

            public Node(string name, Orientation orientation, IntVec2 coord, IntVec2 labelCoord)
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

            public override bool Equals(object? obj)
            {
                return obj is Node node && Equals(node);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Orientation);
            }
        }

        char[,] _map;
        IntVec2 _bounds;
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
            PriorityQueue<(int index, int distance), int> queue = new();
            int[] distances = new int[_nodes.Count];
            for (int i = 0; i < distances.Length; i++)
                distances[i] = int.MaxValue;

            queue.Enqueue(current, 0);
            distances[current.index] = 0;

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                if (current.index == sink)
                    return current.distance;

                if (current.distance == distances[current.index])
                {
                    foreach (var adj in Adjacent(current.index))
                    {
                        int adjDistance = current.distance + adj.distance;
                        ref int existingDist = ref distances[adj.index];
                        if (adjDistance < existingDist)
                        {
                            existingDist = adjDistance;
                            queue.Enqueue((adj.index, adjDistance), adjDistance);
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

            PriorityQueue<(int index, int distance, int level), int> queue = new();
            Dictionary<(int index, int level), int> distances = new();

            queue.Enqueue(current, 0);
            distances[(current.index, current.level)] = 0;

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                if (current.index == sink && current.level == 0)
                    return current.distance;

                if (current.distance == distances[(current.index, current.level)])
                {
                    foreach (var adj in Adjacent(current.index))
                    {
                        int nextLevel = current.level + adj.levelDelta;
                        if (nextLevel < 0)
                            continue;

                        int nextDistance = current.distance + adj.distance;
                        if (!distances.TryGetValue((adj.index, nextLevel), out int adjDist) || nextDistance < adjDist)
                        {
                            distances[(adj.index, nextLevel)] = nextDistance;
                            queue.Enqueue((adj.index, nextDistance, nextLevel), nextDistance);
                        }
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

        [MemberNotNull(nameof(_graph))]
        private void BuildGraph()
        {
            _graph = new (int, int)[_nodes.Count, _nodes.Count];

            for (int i = 0; i < _nodes.Count; i++)
                for (int j = 0; j < _nodes.Count; j++)
                    _graph[i, j] = (-1, 0);

            foreach (var node in _nodes.Keys)
            {
                Dfs(node);
            }
        }

        private void Dfs(in Node n)
        {
            int source = GetNodeIndex(n);
            Stack<(IntVec2 p, int dist)> stack = new();
            HashSet<IntVec2> visited = new();

            stack.Push((n.Coord, 0));
            while (stack.TryPop(out var current))
            {
                if (!visited.Contains(current.p))
                {
                    visited.Add(current.p);

                    int newDistance = current.dist + 1;
                    foreach (IntVec2 adj in current.p.Adjacent(_bounds))
                    {
                        char c = _map[adj.X, adj.Y];
                        if (c == '.' && !visited.Contains(adj))
                            stack.Push((adj, newDistance));
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
        }

        private int GetNodeIndex(in Node n)
        {
            return _nodes[n];
        }

        private Node GetNode(string name, Orientation orientation)
        {
            return _nodes.Keys.Where(n => n.Name == name && n.Orientation == orientation).Single();
        }

        private Node GetNode(IntVec2 p)
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

        private bool TryDiscoverNode(IntVec2 p, out Node node)
        {
            node = default;

            char first = _map[p.X, p.Y];
            if (!char.IsLetter(first))
                return false;

            char second;

            IntVec2 down = p + IntVec2.UnitY;
            if (Bounds(down))
            {
                second = _map[down.X, down.Y];
                if (char.IsLetter(second))
                {
                    Orientation orientation =
                        p.Y == 0 || p.Y == _bounds.Y - 2 ?
                        Orientation.Outside :
                        Orientation.Inside;

                    IntVec2 coord;
                    IntVec2 labelCoord;
                    if (Bounds(p - IntVec2.UnitY) &&
                        _map[p.X, p.Y - 1] == '.')
                    {
                        coord = p - IntVec2.UnitY;
                        labelCoord = p;
                    }
                    else if (Bounds(down + IntVec2.UnitY) &&
                        _map[down.X, down.Y + 1] == '.')
                    {
                        coord = down + IntVec2.UnitY;
                        labelCoord = down;
                    }
                    else
                        throw new InvalidOperationException();

                    node = new Node(new string(new[] { first, second }), orientation, coord, labelCoord);
                    return true;
                }
            }

            IntVec2 right = p + IntVec2.UnitX;
            if (Bounds(right))
            {
                second = _map[right.X, right.Y];
                if (char.IsLetter(second))
                {
                    Orientation orientation =
                        p.X == 0 || p.X == _bounds.X - 2 ?
                        Orientation.Outside :
                        Orientation.Inside;

                    IntVec2 coord;
                    IntVec2 labelCoord;
                    if (Bounds(p - IntVec2.UnitX) &&
                        _map[p.X - 1, p.Y] == '.')
                    {
                        coord = p - IntVec2.UnitX;
                        labelCoord = p;
                    }
                    else if (Bounds(right + IntVec2.UnitX) &&
                        _map[right.X + 1, p.Y] == '.')
                    {
                        coord = right + IntVec2.UnitX;
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

        private bool Bounds(in IntVec2 p)
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
