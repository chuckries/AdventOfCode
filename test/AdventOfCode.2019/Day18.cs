using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode._2019;

public class Day18
{
    class Graph
    {
        public Graph(int count)
        {
            _count = count;
            _graph = new List<List<Edge>>(_count);
            for (int i = 0; i < _count; i++)
                _graph.Add(new List<Edge>(_count));
        }

        struct Edge
        {
            public readonly int SinkIndex;
            public readonly int SinkKeyFlag;
            public readonly int Distance;
            public readonly int KeysRequiredMask;

            public Edge(int sinkIndex, int distance, int keysRequiredMask)
            {
                SinkIndex = sinkIndex;
                SinkKeyFlag = 1 << sinkIndex;
                Distance = distance;
                KeysRequiredMask = keysRequiredMask;
            }
        }

        public void AddEdge(int source, int sink, int distance, int[] keysRequired)
        {
            int mask = 0;
            for (int i = 0; i < keysRequired.Length; i++)
                mask |= (1 << keysRequired[i]);

            _graph[source].Add(new Edge(sink, distance, mask));
        }

        public int MinDistance()
        {
            Dictionary<(int, int), int> states = new Dictionary<(int, int), int>((1 << (_count - 1)) - 1);
            return DynamicHelper(_count - 1, 0, 0, states);
        }

        private int DynamicHelper(int pos, int keystate, int keycount, Dictionary<(int pos, int state), int> states)
        {
            if (keycount == _count - 1)
                return 0;

            if (states.TryGetValue((pos, keystate), out int cached))
                return cached;

            int count = _graph[pos].Count;
            for (int i = 0; i < count; i++)
            {
                Edge edge = _graph[pos][i];

                if ((keystate & edge.SinkKeyFlag) > 0)
                    continue;

                if ((keystate & edge.KeysRequiredMask) != edge.KeysRequiredMask)
                    continue;

                int distance = edge.Distance + DynamicHelper(edge.SinkIndex, keystate | edge.SinkKeyFlag, keycount + 1, states);
                if (!states.TryGetValue((pos, keystate), out int minDistance) || distance < minDistance)
                    states[(pos, keystate)] = distance;
            }

            return states[(pos, keystate)];
        }

        class ArrayEqualityComparer : IEqualityComparer<int[]>
        {
            public bool Equals(int[] x, int[] y)
            {
                if (x.Length != y.Length)
                    return false;

                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                        return false;
                }

                return true;
            }

            public int GetHashCode([DisallowNull] int[] obj)
            {
                if (obj.Length == 0)
                {
                    return obj.GetHashCode();
                }

                int hash = HashCode.Combine(obj[0]);
                for (int i = 1; i < obj.Length; i++)
                {
                    hash = HashCode.Combine(hash, obj[i]);
                }
                return hash;
            }
        }

        class EqualityComparer : IEqualityComparer<(int[], int)>
        {
            static ArrayEqualityComparer s_arrayComparer = new ArrayEqualityComparer();

            public bool Equals((int[], int) x, (int[], int) y)
            {
                return x.Item2 == y.Item2 && s_arrayComparer.Equals(x.Item1, y.Item1);
            }

            public int GetHashCode([DisallowNull] (int[], int) obj)
            {
                return HashCode.Combine(obj.Item2, s_arrayComparer.GetHashCode(obj.Item1));
            }
        }

        public int MinDistance4()
        {
            Dictionary<(int[], int), int> states = new(new EqualityComparer());
            return DynamicHelper4(new int[] { _count - 4, _count - 3, _count - 2, _count - 1 }, 0, 0, states);
        }

        private int DynamicHelper4(int[] pos, int keyState, int keycount, Dictionary<(int[], int), int> states)
        {
            if (keycount == _count - 4)
                return 0;

            if (states.TryGetValue((pos, keyState), out int cached))
                return cached;

            int min = int.MaxValue;
            for (int cPos = 0; cPos < pos.Length; cPos++)
            {
                int oldPos = pos[cPos];

                int count = _graph[oldPos].Count;
                for (int i = 0; i < count; i++)
                {
                    Edge edge = _graph[oldPos][i];

                    if ((keyState & edge.SinkKeyFlag) > 0)
                        continue;

                    if ((keyState & edge.KeysRequiredMask) != edge.KeysRequiredMask)
                        continue;

                    pos[cPos] = edge.SinkIndex;
                    int delta = DynamicHelper4(pos, keyState | edge.SinkKeyFlag, keycount + 1, states);
                    int distance = delta == int.MaxValue ? int.MaxValue : edge.Distance + delta;
                    pos[cPos] = oldPos;
                    if (distance < min)
                        min = distance;
                }
            }
            states[(pos, keyState)] = min;
            return min;
        }

        int _count;
        List<List<Edge>> _graph;
    }

    char[,] _map;
    IntVec2 _bounds;
    IntVec2[] _keys;
    IntVec2 _entry;
    Graph _graph;

    public Day18()
    {
        string[] input = File.ReadAllLines("Inputs/Day18.txt");
        List<IntVec2> keys = new List<IntVec2>(26);
        _map = new char[input[0].Length, input.Length];
        _bounds = new IntVec2(_map.GetLength(0), _map.GetLength(1));
        for (int j = 0; j < _bounds.Y; j++)
        {
            for (int i = 0; i < _bounds.X; i++)
            {
                _map[i, j] = input[j][i];
                if (IsKey(_map[i, j], out int keyIndex))
                {
                    while (keys.Count <= keyIndex)
                        keys.Add(IntVec2.Zero);
                    keys[keyIndex] = new IntVec2(i, j);
                }
                else if (_map[i, j] == '@')
                {
                    _entry = new IntVec2(i, j);
                }
            }
        }
        _keys = keys.ToArray();
    }

    [Fact]
    public void Part1()
    {
        _graph = new Graph(_keys.Length + 1);
        for (int i = 0; i < _keys.Length; i++)
            BfsKeys(_keys[i], i);
        BfsKeys(_entry, 26);

        int answer = _graph.MinDistance();
        Assert.Equal(3764, answer);
    }

    [Fact]
    public void Part2()
    {
        _map[_entry.X, _entry.Y] = '#';
        _map[_entry.X - 1, _entry.Y] = '#';
        _map[_entry.X + 1, _entry.Y] = '#';
        _map[_entry.X, _entry.Y + 1] = '#';
        _map[_entry.X, _entry.Y - 1] = '#';

        _graph = new Graph(_keys.Length + 4);

        for (int i = 0; i < _keys.Length; i++)
            BfsKeys(_keys[i], i);
        BfsKeys(_entry + new IntVec2(1, 1), _keys.Length);
        BfsKeys(_entry + new IntVec2(1, -1), _keys.Length + 1);
        BfsKeys(_entry + new IntVec2(-1, 1), _keys.Length + 2);
        BfsKeys(_entry + new IntVec2(-1, -1), _keys.Length + 3);

        int answer = _graph.MinDistance4();
        Assert.Equal(1738, answer);
    }

    private void BfsKeys(IntVec2 origin, int originIndex)
    {
        (int distance, List<int> keysNeeded)[] distances = new (int distance, List<int> keysNeeded)[_bounds.X * _bounds.Y];
        for (int i = 0; i < distances.Length; i++)
            distances[i].distance = int.MaxValue;
        bool[] visited = new bool[_bounds.X * _bounds.Y];
        Queue<int> toSearch = new Queue<int>();

        int currentIndex = origin.ToIndex(_bounds);
        toSearch.Enqueue(currentIndex);
        distances[currentIndex] = (0, new List<int>());

        do
        {
            currentIndex = toSearch.Dequeue();

            if (visited[currentIndex])
                continue;
            visited[currentIndex] = true;

            IntVec2 current = IntVec2.FromIndex(currentIndex, _bounds);
            int distance = distances[currentIndex].distance;
            int newDistance = distance + 1;
            List<int> newKeys = distances[currentIndex].keysNeeded;

            char c = _map[current.X, current.Y];
            if (IsDoor(c, out int doorIndex))
            {
                newKeys = newKeys.ToList();
                newKeys.Add(doorIndex);
            }
            else if (IsKey(c, out int keyIndex) && keyIndex != origin.ToIndex(_bounds))
            {
                _graph.AddEdge(originIndex, keyIndex, distance, newKeys.ToArray());
            }

            foreach (IntVec2 adjacent in current.Adjacent())
            {
                int adjacentIndex = adjacent.ToIndex(_bounds);
                if (visited[adjacentIndex])
                    continue;

                c = _map[adjacent.X, adjacent.Y];
                if (c == '#')
                {
                    visited[adjacentIndex] = true;
                }
                else
                {
                    distances[adjacentIndex] = (newDistance, newKeys);
                    toSearch.Enqueue(adjacentIndex);
                }
            }
        } while (toSearch.Count > 0);
    }

    private static bool IsKey(char c, out int keyIndex)
    {
        keyIndex = -1;
        if (c >= 'a' && c <= 'z')
        {
            keyIndex = c - 'a';
            return true;
        }
        return false;
    }

    private static bool IsDoor(char c, out int doorIndex)
    {
        doorIndex = -1;
        if (c >= 'A' && c <= 'Z')
        {
            doorIndex = c - 'A';
            return true;
        }
        return false;
    }
}
