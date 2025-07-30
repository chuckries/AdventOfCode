namespace AdventOfCode._2016;

public class Day24
{
    private class Graph
    {
        string[] _map;
        IntVec2[] _nodes;
        int[,] _edges;

        public Graph(string[] map)
        {
            _map = map;
            Parse();
        }

        private void Parse()
        {
            DiscoverNodes();

            _edges = new int[_nodes.Length, _nodes.Length];
            for (int i = 0; i < _nodes.Length; i++)
                for (int j = 0; j < _nodes.Length; j++)
                    _edges[i, j] = -1;

            for (int i = 0; i < _nodes.Length; i++)
                Bfs(i);
        }

        public int MinDistance(int index)
        {
            return MinDistanceHelper(index, _ => 0);
        }

        public int MinDistanceWithReturn(int index)
        {
            return MinDistanceHelper(index, final => _edges[final, index]);
        }

        private int MinDistanceHelper(int index, Func<int, int> foundLast)
        {
            int[,] distances = new int[_nodes.Length, (1 << _nodes.Length) - 1];
            for (int i = 0; i < _nodes.Length; i++)
                for (int j = 0; j < (1 << _nodes.Length) - 1; j++)
                    distances[i, j] = int.MaxValue;

            return DynamicRecurse(0, 1, distances);

            int DynamicRecurse(int sourceIndex, int visitedMask, int[,] distances)
            {
                if (visitedMask == (1 << _nodes.Length) - 1)
                    return (foundLast(sourceIndex));

                if (distances[sourceIndex, visitedMask] != int.MaxValue)
                    return distances[sourceIndex, visitedMask];

                foreach ((int index, int distance) adj in AdjacentNodes(sourceIndex))
                {
                    if ((visitedMask & (1 << adj.index)) > 0)
                        continue;

                    int distance = adj.distance + DynamicRecurse(adj.index, visitedMask | 1 << adj.index, distances);
                    ref int min = ref distances[sourceIndex, visitedMask];
                    if (distance < min)
                        min = distance;
                }

                return distances[sourceIndex, visitedMask];
            }
        }

        private IEnumerable<(int index, int distance)> AdjacentNodes(int index)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_edges[index, i] != -1)
                    yield return (i, _edges[index, i]);
            }
        }

        private void Bfs(int index)
        {
            (IntVec2 p, int distance) current = (_nodes[index], 0);
            HashSet<IntVec2> visited = new();
            Queue<(IntVec2, int distance)> queue = new();
            queue.Enqueue(current);

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                if (visited.Contains(current.p))
                    continue;
                visited.Add(current.p);

                if (char.IsNumber(_map[current.p.Y][current.p.X]))
                {
                    int sinkIndex = _map[current.p.Y][current.p.X] - '0';
                    if (sinkIndex != index)
                        _edges[index, sinkIndex] = current.distance;
                }

                foreach (IntVec2 adj in current.p.Adjacent())
                {
                    if (adj.Y < 0 || adj.Y >= _map.Length || adj.X < 0 || adj.X >= _map[adj.Y].Length)
                        continue;

                    char c = _map[adj.Y][adj.X];
                    if (char.IsNumber(c) && c - '0' == index)
                        continue;
                    if (_map[adj.Y][adj.X] == '#')
                        continue;

                    queue.Enqueue((adj, current.distance + 1));
                }
            }
        }

        private void DiscoverNodes()
        {
            List<(int index, IntVec2 pos)> nodes = new();
            for (int j = 0; j < _map.Length; j++)
                for (int i = 0; i < _map[j].Length; i++)
                {
                    char c = _map[j][i];
                    if (char.IsNumber(c))
                        nodes.Add((c - '0', (i, j)));
                }

            nodes.Sort((lhs, rhs) => lhs.index - rhs.index);
            _nodes = nodes.Select(n => n.pos).ToArray();
        }
    }

    Graph _graph;

    public Day24()
    {
        _graph = new Graph(File.ReadAllLines("Inputs/Day24.txt"));
    }

    [Fact]
    public void Part1()
    {
        int answer = _graph.MinDistance(0);

        Assert.Equal(428, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = _graph.MinDistanceWithReturn(0);

        Assert.Equal(680, answer);
    }
}
