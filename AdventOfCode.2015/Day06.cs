using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day06
    {
        const int GridSize = 1000;

        [Fact]
        public void Part1()
        {
            bool[,] grid = new bool[GridSize, GridSize];

            foreach (var tuple in Parse())
            {
                for (int i = tuple.p1.X;  i <= tuple.p2.X; i++)
                {
                    for (int j = tuple.p1.Y; j <= tuple.p2.Y; j++)
                    {
                        grid[i, j] = tuple.op switch
                        {
                            "on" => true,
                            "off" => false,
                            "toggle" => !grid[i, j],
                            _ => throw new InvalidOperationException()
                        };
                    }
                }
            }

            int total = 0;
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize; j++)
                    if (grid[i, j])
                        total++;

            Assert.Equal(377891, total);
        }

        [Fact]
        public void Part2()
        {
            int[,] grid = new int[GridSize, GridSize];

            foreach (var tuple in Parse())
            {
                for (int i = tuple.p1.X; i <= tuple.p2.X; i++)
                {
                    for (int j = tuple.p1.Y; j <= tuple.p2.Y; j++)
                    {
                        ref int val = ref grid[i, j];
                        switch (tuple.op)
                        {
                            case "on": val++; break;
                            case "off": if (val > 0) val--; break;
                            case "toggle": val += 2; break;
                            default: throw new InvalidOperationException();
                        }
                    }
                }
            }

            int total = 0;
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize; j++)
                    total += grid[i, j];

            Assert.Equal(14110788, total);
        }


        private IEnumerable<(IntPoint2 p1, IntPoint2 p2, string op)> Parse()
        {
            return File.ReadAllLines("Inputs/Day06.txt").Select(s =>
            {
                Match match = s_Regex.Match(s);

                IntPoint2 p1 = (
                    int.Parse(match.Groups["x1"].Value),
                    int.Parse(match.Groups["y1"].Value));

                IntPoint2 p2 = (
                    int.Parse(match.Groups["x2"].Value),
                    int.Parse(match.Groups["y2"].Value));

                return (p1, p2, match.Groups["op"].Value);
            });
        }

        private static Regex s_Regex = new Regex(
            @"^(turn )?(?'op'on|off|toggle) (?'x1'\d+),(?'y1'\d+) through (?'x2'\d+),(?'y2'\d+)$",
            RegexOptions.Compiled);
    }
}
