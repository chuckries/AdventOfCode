using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day17
    {
        long[] _program = File.ReadAllText("Inputs/Day17.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            StringBuilder sb = new StringBuilder();
            IntCodeAsync intCode = new IntCodeAsync(
                _program,
                null,
                value =>
                {
                    if (value == 10)
                        sb.AppendLine();
                    else
                        sb.Append((char)value);
                });

            intCode.RunAsync().Wait();

            string[] map = sb.ToString().Split().Where(s => s != string.Empty).ToArray();

            int total = 0;
            for (int j = 0; j < map.Length; j++)
            {
                for (int i = 0; i < map[j].Length; i++)
                {
                    if (map[j][i] == '#')
                    {
                        if (j - 1 >= 0 && map[j - 1][i] == '#' &&
                            j + 1 < map.Length && map[j + 1][i] == '#' &&
                            i - 1 >= 0 && map[j][i - 1] == '#' &&
                            i + 1 < map[j].Length && map[j][i + 1] == '#')
                            total += i * j;
                    }
                }
            }

            Assert.Equal(3888, total);
        }
    }
}
