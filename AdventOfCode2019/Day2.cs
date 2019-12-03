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
            int[] program = File.ReadAllText("Inputs/Day2.txt").Split(',').Select(int.Parse).ToArray();
            program[1] = 12;
            program[2] = 2;
            RunIntCode(program);

            Assert.Equal(0, program[0]);
        }

        [Fact]
        public void Part2()
        {
            int[] program = File.ReadAllText("Inputs/Day2.txt").Split(',').Select(int.Parse).ToArray();

            const int target = 19690720;

            int i = 0;
            int j = 0;
            bool stop = false;
            for (i = 0; i < 100; i++)
            {
                for (j = 0; j < 100; j++)
                {
                    int[] testProgram = (int[])program.Clone();
                    testProgram[1] = i;
                    testProgram[2] = j;
                    RunIntCode(testProgram);
                    if (testProgram[0] == target)
                    {
                        stop = true;
                        break;
                    }
                }
                if (stop)
                    break;
            }

            int answer = 100 * i + j;
            Assert.Equal(0, answer);
        }

        private void RunIntCode(int[] program)
        {
            int pc = 0;
            for (; ;)
            {
                int op = program[pc++];
                if (op == 99)
                    break;

                int in1 = program[program[pc++]];
                int in2 = program[program[pc++]];
                ref int @out = ref program[program[pc++]];

                switch (op)
                {
                    case 1: @out = in1 + in2; break;
                    case 2: @out = in1 * in2; break;
                    default: Debug.Fail("invalid op code"); break;
                }
            }
        }
    }
}
