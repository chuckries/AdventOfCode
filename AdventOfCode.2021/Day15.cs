using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day15
    {
        private readonly int[,] _map;
        private readonly IntVec2 _bounds;

        public Day15()
        {
            string[] lines = File.ReadAllLines("Inputs/Day15.txt");
            _bounds = new(lines[0].Length, lines.Length);

            _map = new int[_bounds.X, _bounds.Y];
            for (int i = 0; i < _bounds.X; i++)
                for (int j = 0; j < _bounds.Y; j++)
                    _map[i, j] = lines[j][i] - '0';
        }

        [Fact]
        public void Part1()
        {
            int answer = MinDistance(IntVec2.Zero, _bounds - 1, _map, _bounds);
            Assert.Equal(595, answer);
        }

        [Fact]
        public void Part2()
        {
            const int multi = 5;
            IntVec2 bounds = _bounds * multi;
            int[,] map = new int[bounds.X, bounds.Y];

            for (int i = 0; i < bounds.X; i++)
                for (int j = 0; j < bounds.Y; j++)
                {
                    int add = i / _bounds.X + j / _bounds.Y;
                    int val = _map[i % _bounds.X, j % _bounds.Y];
                    for (int adds = 0; adds < add; adds++)
                    {
                        val++;
                        if (val == 10)
                            val = 1;
                    }

                    map[i, j] = val;
                }

            int answer = MinDistance(IntVec2.Zero, bounds - 1, map, bounds);
            Assert.Equal(2914, answer);
        }

        private readonly struct Node
        {
            public readonly IntVec2 p;
            public readonly int distance;

            public Node(IntVec2 p, int distance)
            {
                this.p = p;
                this.distance = distance;
            }
        }

        private static int MinDistance(IntVec2 start, IntVec2 end, int[,] map, IntVec2 bounds)
        {
            int CompareNodes(Node lhs, Node rhs) =>
                lhs.distance - rhs.distance;

            PriorityQueue<Node> toSearch = new(Comparer<Node>.Create(CompareNodes));
            bool[,] visited = new bool[bounds.X, bounds.Y];

            toSearch.Enqueue(new Node(start, 0));
            while (toSearch.Count > 0)
            {
                Node current = toSearch.Dequeue();

                if (visited[current.p.X, current.p.Y])
                    continue;

                if (current.p == end)
                    return current.distance;

                visited[current.p.X, current.p.Y] = true;

                foreach (IntVec2 adj in current.p.Adjacent(bounds))
                    if (!visited[adj.X, adj.Y])
                        toSearch.Enqueue(new Node(adj, current.distance + map[adj.X, adj.Y]));
            }

            throw new InvalidOperationException();
        }
    }
}
