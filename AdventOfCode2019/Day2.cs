using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2019
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

            memory[1] = 12;
            memory[2] = 2;

            IntCode intCode = new IntCode(memory, null, null);
            intCode.Run();

            Assert.Equal(3931283, intCode.Memory[0]);
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
                    int[] testMemory = (int[])memory.Clone();
                    testMemory[1] = i;
                    testMemory[2] = j;

                    IntCode intCode = new IntCode(testMemory, null, null);
                    intCode.Run();

                    if (intCode.Memory[0] == target)
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
