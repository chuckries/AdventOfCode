using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day9
    {
        long[] _program = File.ReadAllText("Inputs/Day9.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

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

            Assert.True(program.SequenceEqual(output));
        }
    }
}
