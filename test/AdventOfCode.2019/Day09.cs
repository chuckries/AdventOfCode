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
            long answer = 0;
            IntCode intCode = new IntCode(
                _program,
                () => 1L,
                val => answer = val
                );

            intCode.Run();

            Assert.Equal(2518058886, answer);
        }

        [Fact]
        public void Part2()
        {
            long answer = 0;
            IntCode intCode = new IntCode(
                _program,
                () => 2L,
                val => answer = val
                ); ;

            intCode.Run();

            Assert.Equal(44292, answer);
        }
    }
}
