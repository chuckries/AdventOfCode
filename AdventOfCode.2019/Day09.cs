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
            IntCode intCode = new IntCode(
                _program,
                () => Task.FromResult(1L),
                answer.Add
                );

            intCode.Run().Wait();

            Assert.Collection(answer, item => Assert.Equal(2518058886, item));
        }

        [Fact]
        public void Part2()
        {
            List<long> answer = new List<long>();
            IntCode intCode = new IntCode(
                _program,
                () => Task.FromResult(2L),
                answer.Add
                );

            intCode.Run().Wait();

            Assert.Collection(answer, item => Assert.Equal(44292, item));
        }

        [Fact]
        public void Part1Example()
        {
            long[] program = { 109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 };

            List<long> output = new List<long>(program.Length);
            IntCode intCode = new IntCode(
                program,
                null,
                output.Add
                );
            intCode.Run().Wait();

            Assert.Collection(output,
                program.Select((long p) => new Action<long>((long item) => Assert.Equal(p, item))).ToArray());
        }
    }
}
