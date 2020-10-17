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
        class Bot
        {
            public Bot(long[] program)
            {
                _bot = new IntCodeAsync(
                    program,
                    _ => Task.FromResult(_input.Dequeue()),
                    value => _output = value);
            }

            public long Query(int x, int y)
            {
                _input.Enqueue(x);
                _input.Enqueue(y);
                _bot.Reset();
                _bot.RunAsync().Wait();
                return _output;
            }

            IntCodeAsync _bot;
            Queue<long> _input = new Queue<long>();
            long _output = 0;
        }

        Bot _bot;

        public Day19()
        {
            _bot = new Bot(File.ReadAllText("Inputs/Day19.txt").Split(',').Select(long.Parse).ToArray());
        }

        [Fact]
        public void Part1()
        {
            int total = 0;
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (_bot.Query(i, j) == 1)
                        total++;
                }
            }

            Assert.Equal(112, total);
        }
    }
}
