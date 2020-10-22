using System;
using System.IO;
using System.Linq;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day1
    {

        string _str;

        public Day1()
        {
            _str = File.ReadAllText("Inputs/Day1.txt");
        }

        [Fact]
        public void Part1()
        {
            int answer = _str.
                Select(c => c switch
                {
                    '(' => 1,
                    ')' => -1,
                    _ => throw new InvalidOperationException()
                })
                .Sum();

            Assert.Equal(138, answer);
        }

        [Fact]
        public void Part2()
        {
            int index = 0;
            int total = 0;
            while (true)
            {
                char c = _str[index++];
                total += c switch
                {
                    '(' => 1,
                    ')' => -1,
                    _ => throw new OperationCanceledException()
                };

                if (total == -1)
                    break;
            }

            Assert.Equal(1771, index);
        }
    }
}
