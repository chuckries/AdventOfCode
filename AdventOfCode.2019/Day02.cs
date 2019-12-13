using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day02
    {
        long[] _program = File.ReadAllText("Inputs/Day02.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            IntCode intCode = new IntCode(_program);
            intCode[1] = 12;
            intCode[2] = 2;
            intCode.Run().Wait();

            Assert.Equal(3931283, intCode[0]);
        }

        [Fact]
        public void Part2()
        {
            const int target = 19690720;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    IntCode intCode = new IntCode(_program);
                    intCode[1] = i;
                    intCode[2] = j;
                    intCode.Run().Wait();

                    if (intCode[0] == target)
                    {
                        int answer = 100 * i + j;
                        Assert.Equal(6979, answer);
                        return;
                    }
                }
            }
        }
    }
}
