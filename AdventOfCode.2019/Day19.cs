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
    public class Day19
    {
        long[] _program = File.ReadAllText("Inputs/Day19.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();
        IntCode _bot;
        Queue<long> _input = new Queue<long>();

        public Day19()
        {
            _bot = new IntCode(
                _program,
                () => Task.FromResult(_input.Dequeue()),
                null);
        }

        [Fact]
        public void Part1()
        {
            int total = 0;
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (QueryBot(i, j) == 1)
                        total++;
                }
            }

            Assert.Equal(112, total);
        }

        private long QueryBot(int x, int y)
        {
            long output = 0;
            _input.Enqueue(x);
            _input.Enqueue(y);
            _bot.Reset();
            _bot.Writer = value => output = value;
            _bot.Run().Wait();
            return output;
        }
    }
}
