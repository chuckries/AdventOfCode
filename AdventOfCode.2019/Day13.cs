using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day13
    {
        enum Tile : long
        {
            Emtpy = 0,
            Wall = 1,
            Block = 2,
            Paddle = 3,
            Ball = 4
        }

        long[] _program = File.ReadAllText("Inputs/Day13.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            Dictionary<IntPoint2, Tile> canvas = new Dictionary<IntPoint2, Tile>();
            List<long> outputs = new List<long>(3);

            IntCode.OutputWriter writer = output =>
            {
                outputs.Add(output);
                if (outputs.Count == 3)
                {
                    canvas[((int)outputs[0], (int)outputs[1])] = (Tile)outputs[2];
                    outputs.Clear();
                }
            };

            IntCode arcade = new IntCode(_program, null, writer);
            arcade.Run().Wait();

            int answer = canvas.Values.Count(t => t == Tile.Block);
            Assert.Equal(173, answer);
        }
    }
}
