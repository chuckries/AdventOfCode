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
            int countBlocks = 0;
            List<long> outputs = new List<long>(3);
            IntCodeAsync arcade = new IntCodeAsync(
                _program,
                null,
                output =>
                {
                    outputs.Add(output);
                    if (outputs.Count == 3)
                    {
                        if ((Tile)outputs[2] == Tile.Block)
                            countBlocks++;
                        outputs.Clear();
                    }
                });
            arcade.RunAsync().Wait();

            Assert.Equal(173, countBlocks);
        }

        [Fact]
        public void Part2()
        {
            List<long> outputs = new List<long>(3);
            long score = 0;
            long ballX = 0;
            long paddleX = 0;

            IntCodeBase.OutputWriter writer = output =>
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

            IntCode.InputReader reader = () => Math.Sign(ballX - paddleX);

            IntCode arcade = new IntCode(_program, reader, writer);
            arcade[0] = 2;
            arcade.Run();

            Assert.Equal(8942, score);
        }
    }
}
