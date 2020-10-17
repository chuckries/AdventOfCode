using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day09
    {
        long[] _program = File.ReadAllText("Inputs/Day09.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            List<long> answer = new List<long>();
            IntCodeAsync intCode = new IntCodeAsync(
                _program,
                _ => Task.FromResult(1L),
                answer.Add
                );

            intCode.RunAsync().Wait();

            Assert.Collection(answer, item => Assert.Equal(2518058886, item));
        }

        [Fact]
        public void Part2()
        {
            List<long> answer = new List<long>();
            IntCodeAsync intCode = new IntCodeAsync(
                _program,
                _ => Task.FromResult(2L),
                answer.Add
                );

            intCode.RunAsync().Wait();

            Assert.Collection(answer, item => Assert.Equal(44292, item));
        }
    }
}
