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
        const int Depth = 10689;
        const int TargetX = 11;
        const int TargetY = 722;
        static readonly IntVec2 TargetCoord = (TargetX, TargetY);

        enum Terrain : int
        {
            Rocky = 0,
            Wet = 1,
            Narrow = 2,
            Invalid = int.MaxValue
        }

        enum Tool
        {
            None = 0,
            Climbing = 1,
            Torch = 2,
            Invalid = int.MaxValue
        }

        [DebuggerDisplay("{Coord}, {Terrain}")]
        private readonly struct Region
        {
            public readonly IntVec2 Coord;
            public readonly int Index;
            public readonly int Erosion;
            public readonly Terrain Terrain;

            public Region(IntVec2 coord, int index, int erosion, Terrain terrain)
            {
                Coord = coord;
                Index = index;
                Erosion = erosion;
                Terrain = terrain;
            }
        }

        private readonly struct SearchCoord : IEquatable<SearchCoord>
        {
            public readonly IntVec2 Coord;
            public readonly Tool Tool;

            public SearchCoord(IntVec2 coord, Tool tool)
            {
                Coord = coord;
                Tool = tool;
            }

            public bool Equals(SearchCoord other)
            {
                return Coord.Equals(other.Coord) && Tool == other.Tool;
            }

            public override bool Equals(object obj)
            {
                return obj is SearchCoord other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Coord.X, Coord.Y, Tool);
            }
        }

        [DebuggerDisplay("{Tool}, {Region}, {Time}")]
        struct Node
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
                Distance = region.Coord.DistanceFrom(TargetCoord);
            }

            public SearchCoord SearchCoord => new SearchCoord(Region.Coord, Tool);
        }

        Dictionary<IntVec2, Region> _regions = new Dictionary<IntVec2, Region>();

        [Fact]
        public void Part1()
        {
            int answer = 0;
            for (int i = 0; i <= TargetCoord.X; i++)
            {
                for (int j = 0; j <= TargetCoord.Y; j++)
                {
                    answer += (int)GetRegion(new IntVec2(i, j)).Terrain;
                }
            }

            Assert.Equal(8575, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Search();
            Assert.Equal(999, answer);
        }

        private int Search()
        {
            Tool[,] otherValidTool =
            {
                { Tool.Invalid, Tool.Torch, Tool.Climbing },
                { Tool.Climbing, Tool.None, Tool.Invalid },
                { Tool.Torch, Tool.Invalid, Tool.None }
            };

            bool[,] validToolRegions =
            {
                { false, true, true },
                { true, true, false },
                { true, false, true },
            };

            IEnumerable<Node> GetAdjacentNodes(Node current)
            {
                Tool newTool = otherValidTool[(int)current.Region.Terrain, (int)current.Tool];
                yield return new Node(current.Region, newTool, current.Time + 7);

                foreach (IntVec2 adjacent in current.Region.Coord.Adjacent())
                {
                    if (adjacent.X >= 0 && adjacent.Y >= 0)
                    {
                        Region region = GetRegion(adjacent);
                        if (validToolRegions[(int)current.Tool, (int)region.Terrain])
                            yield return new Node(region, current.Tool, current.Time + 1);
                    }
                }
            }

            IComparer<Node> comparer = Comparer<Node>.Create((left, right) =>
            {
                return (left.Time + left.Distance) -
                       (right.Time + right.Distance);
            });

            Node current = new Node(GetRegion(IntVec2.Zero), Tool.Torch, 0);
            PriorityQueue<Node> searchSet = new PriorityQueue<Node>(comparer);
            Dictionary<SearchCoord, int> times = new Dictionary<SearchCoord, int>();

            searchSet.Enqueue(current);
            times.Add(current.SearchCoord, 0);

            while (searchSet.Count > 0)
            {
                current = searchSet.Dequeue();

                if (current.Region.Coord.Equals(TargetCoord) && current.Tool == Tool.Torch)
                {
                    return current.Time;
                }

                foreach (Node adjacent in GetAdjacentNodes(current))
                {
                    if (!times.TryGetValue(adjacent.SearchCoord, out int existingTime) ||
                        adjacent.Time < existingTime)
                    {
                        times[adjacent.SearchCoord] = adjacent.Time;
                        searchSet.Enqueue(adjacent);
                    }
                }
            }

            throw new InvalidOperationException();
        }

        private Region GetRegion(in IntVec2 coord)
        {
            if (!_regions.TryGetValue(coord, out Region region))
            {
                int index = coord switch
                { 
                    (0, 0) or (TargetX, TargetY) => 0,
                    (int X, 0) => X * 16807,
                    (0, int Y) => Y * 48271,
                    _ => GetRegion(coord - IntVec2.UnitX).Erosion * GetRegion(coord - IntVec2.UnitY).Erosion
                };

                int erosion = (index + Depth) % 20183;
                Terrain terrain = (Terrain)(erosion % 3);

                _regions.Add(coord, region = new Region(coord, index, erosion, terrain));
            }
            return region;
        }
    }
}
