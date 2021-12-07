using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2021
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
            int count = 0;
            for (int i = 1; i < _input.Length; i++)
            {
                if (_input[i] > _input[i - 1])
                    count++;
            }

            count = _input.Skip(1).Zip(_input).Count(pair => pair.First > pair.Second);

            Assert.Equal(1342, count);
        }

        [Fact]
        public void Part2()
        {
            int count = 0;
            for (int i = 3; i < _input.Length; i++)
            {
                if (_input[i] > _input[i - 3])
                    count++;
            }

            count = _input.Skip(3).Zip(_input).Count(pair => pair.First > pair.Second);

            Assert.Equal(1378, count);
        }

    }
}
