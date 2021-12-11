using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2017
{
    public class Day17
    {
        private const int Input = 355;

        [Fact]
        public void Part1()
        {
            int[] buffer = new int[2018];

            int current = 0;
            for (int i = 1; i <= 2017; i++)
            {
                for (int j = 0; j < Input; j++)
                    current = buffer[current];

                int tmp = buffer[current];
                buffer[current] = i;
                buffer[i] = tmp;

                current = i;
            }

            int answer = buffer[current];

            Assert.Equal(1912, answer);
        }

        [Fact]
        public void Part2()
        {
            int current = 0;
            int answer = 0;
            for (int i = 1; i <= 50_000_000; i++)
                if (1 == (current = (current + Input) % i + 1))
                    answer = i;

            Assert.Equal(21066990, answer);
        }
    }
}
