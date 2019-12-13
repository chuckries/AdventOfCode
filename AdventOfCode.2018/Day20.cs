using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2018
{
    public class Day20
    {
        class Node : IEquatable<Node>
        {
            public readonly IntPoint2 Coord;

            public Node North { get; set; }
            public Node South { get; set; }
            public Node West { get; set; }
            public Node East { get; set; }

            public bool Visited { get; set; } = false;
            public int Distance { get; set; } = int.MaxValue;

            public Node(IntPoint2 coord)
            {
                Coord = coord;
            }

            public IEnumerable<Node> Adjacent()
            {
                if (North != null) yield return North;
                if (South != null) yield return South;
                if (West != null) yield return West;
                if (East != null) yield return East;
            }

            public IEnumerable<Node> AdjacentUnivisted()
            {
                foreach (Node n in Adjacent())
                {
                    if (!n.Visited)
                        yield return n;
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Node);
            }

            public bool Equals(Node other)
            {
                return other != null &&
                       Coord.Equals(other.Coord);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Coord);
            }
        }

        public class RegexMap
        {
            public RegexMap(string pattern)
            {
                _pattern = pattern;
            }

            public IEnumerable<T> Traverse<T>(T seed, Func<T, string, T> visit)
            {
                (_, IEnumerable<T> allOptions) = Parse(0, new List<T> { seed }, visit);
                return allOptions;
            }

            private (int index, IEnumerable<T> newHeads) Parse<T>(int index, IEnumerable<T> heads, Func<T, string, T> visit)
            {
                List<T> currentOptionHeads = heads.ToList();
                HashSet<T> newHeads = new HashSet<T>();

                while (index < _pattern.Length)
                {
                    char c = _pattern[index];

                    if (c == '(')
                    {
                        index++;
                        (int newIndex, IEnumerable<T> newOptionHeads) = Parse(index, currentOptionHeads, visit);
                        index = newIndex;
                        currentOptionHeads = newOptionHeads.ToList();
                    }
                    else if (c == '|')
                    {
                        index++;
                        foreach (T n in currentOptionHeads)
                            newHeads.Add(n);

                        currentOptionHeads = heads.ToList();
                    }
                    else if (c == ')')
                    {
                        index++;

                        foreach (T n in currentOptionHeads)
                            newHeads.Add(n);

                        return (index, newHeads);
                    }
                    else if (c == '^')
                    {
                        index++;
                    }
                    else if (c == '$')
                    {
                        index++;
                        return (index, currentOptionHeads);
                    }
                    else
                    {
                        int oldIndex = index;
                        index = _pattern.IndexOfAny(s_tokens, oldIndex);
                        string directions = _pattern.Substring(oldIndex, index - oldIndex);
                        for (int i = 0; i < currentOptionHeads.Count; i++)
                        {
                            currentOptionHeads[i] = visit(currentOptionHeads[i], directions);
                        }
                    }
                }

                throw new InvalidOperationException("reached end without returning?");
            }

            private string _pattern;

            private static char[] s_tokens = new[] { '|', '(', ')', '^', '$' };
        }

        [Fact]
        public void Part1()
        {
            IEnumerable<Node> map = TraverseMap(File.ReadAllText("Inputs/Day20.txt"));
            int answer = map.Max(n => n.Distance);
            Assert.Equal(3739, answer);
        }

        [Fact]
        public void Part2()
        {
            IEnumerable<Node> map = TraverseMap(File.ReadAllText("Inputs/Day20.txt"));
            int answer = map.Count(n => n.Distance >= 1000);
            Assert.Equal(8409, answer);
        }

        private IEnumerable<Node> TraverseMap(string input)
        {
            RegexMap regex = new RegexMap(input);
            Dictionary<IntPoint2, Node> map = new Dictionary<IntPoint2, Node>();
            Node GetNode(IntPoint2 coord)
            {
                if (!map.TryGetValue(coord, out Node n))
                    map.Add(coord, (n = new Node(coord)));
                return n;
            }

            Node origin = GetNode(IntPoint2.Zero);
            Func<Node, string, Node> parseDirections = (Node current, string directions) =>
            {
                foreach (char c in directions)
                {
                    if (c == 'N')
                    {
                        current.North = GetNode(current.Coord + IntPoint2.UnitY);
                        current.North.South = current;
                        current = current.North;
                    }
                    else if (c == 'S')
                    {
                        current.South = GetNode(current.Coord - IntPoint2.UnitY);
                        current.South.North = current;
                        current = current.South;
                    }
                    else if (c == 'W')
                    {
                        current.West = GetNode(current.Coord - IntPoint2.UnitX);
                        current.West.East = current;
                        current = current.West;
                    }
                    else if (c == 'E')
                    {
                        current.East = GetNode(current.Coord + IntPoint2.UnitX);
                        current.East.West = current;
                        current = current.East;
                    }
                    else
                        throw new InvalidOperationException();
                }

                return current;
            };

            regex.Traverse(origin, parseDirections);
            Dijkstra(origin, map.Values);
            return map.Values;
        }

        private void Dijkstra(Node origin, IEnumerable<Node> nodes)
        {
            HashSet<Node> unvisisted = new HashSet<Node>(nodes);
            origin.Distance = 0;
            Node current = origin;

            while (unvisisted.Count > 0)
            {
                int distance = current.Distance + 1;
                foreach(Node adjacent in current.AdjacentUnivisted())
                {
                    if (distance < adjacent.Distance)
                        adjacent.Distance = distance;
                }

                current.Visited = true;
                unvisisted.Remove(current);

                Node minNode = null;
                int minDistance = int.MaxValue;
                foreach (Node n in unvisisted)
                {
                    if (n.Distance < minDistance)
                    {
                        minNode = n;
                        minDistance = n.Distance;
                    }
                }

                current = minNode;
            }
        }
    }
}
