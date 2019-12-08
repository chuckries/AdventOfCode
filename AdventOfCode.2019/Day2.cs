using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day2
    {
        [Fact]
        public void Part1()
        {
            int[] memory = File.ReadAllText("Inputs/Day2.txt")
                .Split(',')
                .Select(int.Parse)
                .ToArray();

            IntCode intCode = new IntCode(memory);
            intCode[1] = 12;
            intCode[2] = 2;
            intCode.Run().Wait();

            Assert.Equal(3931283, intCode[0]);
        }

        [Fact]
        public void Part2()
        {
            int[] memory = File.ReadAllText("Inputs/Day2.txt")
                .Split(',')
                .Select(int.Parse)
                .ToArray();

            const int target = 19690720;

            int i = 0;
            int j = 0;
            bool stop = false;
            for (i = 0; i < 100; i++)
            {
                for (j = 0; j < 100; j++)
                {
                    IntCode intCode = new IntCode(memory);
                    intCode[1] = i;
                    intCode[2] = j;
                    intCode.Run().Wait();

                    if (intCode[0] == target)
                    {
                        stop = true;
                        break;
                    }
                }
                if (stop)
                    break;
            }

            int answer = 100 * i + j;
            Assert.Equal(6979, answer);
        }
    }
}
