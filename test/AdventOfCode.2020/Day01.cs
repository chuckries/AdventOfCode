using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2020
{
    public class Day01
    {
        int[] _input;

        public Day01()
        {
            _input = File.ReadAllLines("Inputs/Day01.txt").Select(int.Parse).ToArray();
        }

        [Fact]
        public void Part1()
        {
            int answer = 0;

            for (int i = 0; i < _input.Length - 1; i++)
            {
                for (int j = i + 1; j < _input.Length; j++)
                {
                    if (_input[i] + _input[j] == 2020)
                        answer = _input[i] * _input[j];
                }
            }

            Assert.Equal(388075, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = 0;

            for (int i = 0; i < _input.Length - 2; i++)
            {
                for (int j = i + 1; j < _input.Length - 1; j++)
                {
                    for (int k = j + 1; k < _input.Length; k++)
                    {
                        if (_input[i] + _input[j] + _input[k] == 2020)
                            answer = _input[i] * _input[j] * _input[k];
                    }
                }
            }

            Assert.Equal(293450526, answer);
        }
    }
}
