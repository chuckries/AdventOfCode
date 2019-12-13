using AdventOfCode.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2018
{
    public class Day22
    {
        const int DEPTH = 10689;
        static readonly IntPoint2 TARGET = (11, 722);

        enum Terrain : int
        {
            Rocky = 0,
            Wet = 1,
            Narrow = 2
        }

        enum Tool
        {
            None,
            Climbing,
            Torch
        }

        [DebuggerDisplay("{Coord}, {Terrain}")]
        struct Region : IEquatable<Region>
        {
            public readonly IntPoint2 Coord;
            public readonly int Index;
            public readonly int Erosion;
            public readonly Terrain Terrain;

            public Region(IntPoint2 coord, int index, int erosion, Terrain terrain)
            {
                Coord = coord;
                Index = index;
                Erosion = erosion;
                Terrain = terrain;
            }

            public override bool Equals(object obj)
            {
                return obj is Region region && Equals(region);
            }

            public bool Equals(Region other)
            {
                return Coord.Equals(other.Coord);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Coord);
            }
        }

        [DebuggerDisplay("{Tool}, {Region}, {Time}")]
        struct Node : IEquatable<Node>
        {
            public readonly Region Region;
            public readonly Tool Tool;
            public readonly int Time;
            public readonly int Distance;

            public Node(Region region, Tool tool, int time)
            {
                Region = region;
                Tool = tool;
                Time = time;
                Distance = region.Coord.Distance(TARGET);
            }

            public override bool Equals(object obj)
            {
                return obj is Node node && Equals(node);
            }

            public bool Equals(Node other)
            {
                return Region.Equals(other.Region) &&
                       Tool == other.Tool;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Region, Tool);
            }
        }

        Dictionary<IntPoint2, Region> _regions = new Dictionary<IntPoint2, Region>();

        [Fact]
        public void Part1()
        {
            int answer = 0;
            for (int i = 0; i <= TARGET.X; i++)
            {
                for (int j = 0; j <= TARGET.Y; j++)
                {
                    answer += (int)GetRegion(new IntPoint2(i, j)).Terrain;
                }
            }

            Assert.Equal(8575, answer);
        }

        [Fact]
        public void Part2()
        {
            IEnumerable<Node> GetAdjacentNodes(Node current)
            {
                Tool newTool = current.Region.Terrain switch
                {
                    Terrain.Rocky => current.Tool switch
                    {
                        Tool.Climbing => Tool.Torch,
                        Tool.Torch => Tool.Climbing,
                        _ => throw new InvalidOperationException("invalid tool")
                    },
                    Terrain.Wet => current.Tool switch
                    {
                        Tool.None => Tool.Climbing,
                        Tool.Climbing => Tool.None,
                        _ => throw new InvalidOperationException("invalid tool")
                    },
                    Terrain.Narrow => current.Tool switch
                    {
                        Tool.None => Tool.Torch,
                        Tool.Torch => Tool.None,
                        _ => throw new InvalidOperationException("invalid tool")
                    },
                    _ => throw new InvalidOperationException("invalid region")
                };
                yield return new Node(current.Region, newTool, current.Time + 7);

                foreach (IntPoint2 adjacent in current.Region.Coord.Adjacent())
                {
                    if (adjacent.X >= 0 && adjacent.Y >= 0)
                    {
                        Region region = GetRegion(adjacent);
                        if ((current.Tool == Tool.None &&
                                (region.Terrain == Terrain.Wet || region.Terrain == Terrain.Narrow)) ||
                            (current.Tool == Tool.Torch &&
                                (region.Terrain == Terrain.Rocky || region.Terrain == Terrain.Narrow)) ||
                            (current.Tool == Tool.Climbing &&
                                (region.Terrain == Terrain.Rocky || region.Terrain == Terrain.Wet)))
                        {
                            yield return new Node(region, current.Tool, current.Time + 1);
                        }
                    }
                }
            }

            IComparer<Node> comparer = Comparer<Node>.Create((left, right) =>
            {
                return (left.Time + left.Distance) -
                       (right.Time + right.Distance);
            });

            PriorityQueue<Node> searchSet = new PriorityQueue<Node>(comparer);
            Dictionary<Node, int> visited = new Dictionary<Node, int>();

            Node start = new Node(GetRegion(IntPoint2.Zero), Tool.Torch, 0);
            searchSet.Enqueue(start);
            visited.Add(start, 0);

            int answer = 0;
            while (searchSet.Count > 0)
            {
                Node current = searchSet.Dequeue();
                if (current.Region.Coord.Equals(TARGET) && current.Tool == Tool.Torch)
                {
                    answer = current.Time;
                    break;
                }
                else
                {
                    foreach (Node adjacent in GetAdjacentNodes(current))
                    {
                        if (!visited.TryGetValue(adjacent, out int time))
                        {
                            visited.Add(adjacent, adjacent.Time);
                            searchSet.Enqueue(adjacent);
                        }
                        else if (adjacent.Time < time)
                        {
                            visited[adjacent] = adjacent.Time;
                            searchSet.Enqueue(adjacent);
                        }
                    }
                }
            }

            Assert.Equal(999, answer);
        }

        private Region GetRegion(in IntPoint2 coord)
        {
            if (!_regions.TryGetValue(coord, out Region region))
            {
                int index;
                if (coord.Equals(IntPoint2.Zero) || coord.Equals(TARGET))
                    index = 0;
                else if (coord.X == 0)
                    index = coord.Y * 48271;
                else if (coord.Y == 0)
                    index = coord.X * 16807;
                else
                    index = GetRegion(coord - IntPoint2.UnitX).Erosion * GetRegion(coord - IntPoint2.UnitY).Erosion;

                int erosion = (index + DEPTH) % 20183;
                Terrain terrain = (Terrain)(erosion % 3);

                _regions.Add(coord, (region = new Region(coord, index, erosion, terrain)));
            }
            return region;
        }
    }
}
