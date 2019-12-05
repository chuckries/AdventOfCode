using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2019
{
    public class Day5
    {
        int[] _memory = File.ReadAllText("Inputs/Day5.txt").Split(',').Select(int.Parse).ToArray();

        [Fact]
        public void Part1()
        {
            List<int> outputs = new List<int>();
            IntCode intCode = new IntCode(
                _memory,
                () => 1,
                (int value) => outputs.Add(value)
                );

            intCode.Run();

            Assert.True(outputs.SkipLast(1).All(0.Equals));
            Assert.Equal(8332629, outputs.Last());
        }

        [Fact]
        public void Part2()
        {
            int output = 0;
            IntCode intCode = new IntCode(
                _memory,
                () => 5,
                (int value) => output = value
                );

            intCode.Run();

            Assert.Equal(8805067, output);
        }
    }
}
