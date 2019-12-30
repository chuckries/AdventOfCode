using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day05
    {
        long[] _program = File.ReadAllText("Inputs/Day05.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            List<long> outputs = new List<long>();
            IntCodeAsync intCode = new IntCodeAsync(
                _program,
                () => Task.FromResult(1L),
                outputs.Add
                );

            intCode.RunAsync().Wait();

            Assert.True(outputs.SkipLast(1).All(0L.Equals));
            Assert.Equal(8332629, outputs.Last());
        }

        [Fact]
        public void Part2()
        {
            long output = 0;
            IntCodeAsync intCode = new IntCodeAsync(
                _program,
                () => Task.FromResult(5L),
                value => output = value
                );

            intCode.RunAsync().Wait();

            Assert.Equal(8805067, output);
        }
    }
}
