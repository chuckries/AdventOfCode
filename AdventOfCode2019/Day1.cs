using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2019
{
    public class Day1
    {
        [Fact]
        public void Part1()
        {
            int sum = File.ReadAllLines("Inputs/Day1.txt")
                .Select(int.Parse)
                .Select(i => (i / 3) - 2)
                .Sum();

            Assert.Equal(3363929, sum);
        }

        [Fact]
        public void Part2()
        {
            var modules = File.ReadAllLines("Inputs/Day1.txt").Select(int.Parse);

            int total = 0;

            foreach(int module in modules)
            {
                int newWeight = module;
                while (newWeight > 0)
                {
                    newWeight = (newWeight / 3) - 2;
                    if (newWeight > 0)
                    {
                        total += newWeight;
                    }
                }
            }

            Assert.Equal(5043026, total);
        }
    }
}
