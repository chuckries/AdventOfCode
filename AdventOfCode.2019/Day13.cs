using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Fact]
        public void Part2()
        {
            List<long> outputs = new List<long>(3);
            long score = 0;
            long ballX = 0;
            long paddleX = 0;

            IntCode.OutputWriter writer = output =>
            {
                outputs.Add(output);
                if (outputs.Count == 3)
                {
                    if (outputs[0] == -1 && outputs[1] == 0)
                        score = outputs[2];
                    else
                    {
                        Tile tile = (Tile)outputs[2];
                        long x = outputs[0];
                        if (tile == Tile.Ball) ballX = x;
                        if (tile == Tile.Paddle) paddleX = x;
                    }
                    outputs.Clear();
                }
            };

            IntCode.InputReader reader = () => 
                Task.FromResult<long>(Math.Sign(ballX - paddleX));

            IntCode arcade = new IntCode(_program, reader, writer);
            arcade[0] = 2;
            arcade.Run().Wait();

            Assert.Equal(8942, score);
        }
    }
}
