using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AdventOfCode._2018
{
    public class Day22
    {
        const int DEPTH = 10689;
        static readonly IntPair TARGET = (11, 722);

        enum Terrain : int
        {
            Rocky = 0,
            Wet = 1,
            Narrow = 2
        }

        class Region
        {
            public readonly IntPair Coord;
            public readonly int Index;
            public readonly int Erosion;
            public readonly Terrain Terrain;

            public Region(IntPair coord, int index, int erosion, Terrain terrain)
            {
                Coord = coord;
                Index = index;
                Erosion = erosion;
                Terrain = terrain;
            }
        }

        Dictionary<IntPair, Region> _regions = new Dictionary<IntPair, Region>();

        [Fact]
        public void Part1()
        {
            int answer = 0;
            for (int i = 0; i <= TARGET.X; i++)
            {
                for (int j = 0; j <= TARGET.Y; j++)
                {
                    answer += (int)GetRegion(new IntPair(i, j)).Terrain;
                }
            }

            Assert.Equal(8575, answer);
        }

        private Region GetRegion(in IntPair coord)
        {
            if (!_regions.TryGetValue(coord, out Region region))
            {
                int index;
                if (coord.Equals(IntPair.Zero) || coord.Equals(TARGET))
                    index = 0;
                else if (coord.X == 0)
                    index = coord.Y * 48271;
                else if (coord.Y == 0)
                    index = coord.X * 16807;
                else
                    index = GetRegion(coord + IntPair.Left).Erosion * GetRegion(coord + IntPair.Down).Erosion;

                int erosion = (index + DEPTH) % 20183;
                Terrain terrain = (Terrain)(erosion % 3);

                _regions.Add(coord, (region = new Region(coord, index, erosion, terrain)));
            }
            return region;
        }
    }
}
