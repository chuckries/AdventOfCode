using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;
using System.IO;

namespace AdventOfCode._2020
{
    public class Day05
    {
        string[] _input;

        public Day05()
        {
            _input = File.ReadAllLines("Inputs/Day05.txt");
        }

        [Fact]
        public void Part1()
        {
            int answer = _input.Select(Calculate).Max();

            Assert.Equal(980, answer);
        }

        [Fact]
        public void Part2()
        {
            List<int> seats = _input.Select(Calculate).ToList();
            seats.Sort();

            int answer = 0;
            for (int i = 0; i < seats.Count - 1; i++)
            {
                if (seats[i] == seats[i + 1] - 2)
                {
                    answer = seats[i] + 1;
                    break;
                }
            }

            Assert.Equal(607, answer);
        }

        [Fact]
        public void Sample()
        {
            int answer = Calculate("FBFBBFFRLR");
            Assert.Equal(357, answer);
        }

        private int Calculate(string input)
        {
            int front = 0;
            int back = 127;

            int left = 0;
            int right = 7;

            foreach (char c in input.AsSpan(0, 7))
            {
                if (c == 'F')
                    back = front + (back - front) / 2;
                else if (c == 'B')
                    front = front + (back - front + 1) / 2;
                else 
                    throw new InvalidOperationException();
            }

            foreach (char c in input.AsSpan(7))
            {
                if (c == 'L')
                    right = left + (right - left) / 2;
                else if (c == 'R')
                    left = left + (right - left + 1) / 2;
                else
                    throw new InvalidOperationException();
            }

            return front * 8 + left;
        }
    }
}
