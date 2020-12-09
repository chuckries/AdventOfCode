using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;

namespace AdventOfCode._2020
{
    public class Day09
    {
        long[] _input;
        Dictionary<long, long> _valueToIndexMap;

        public Day09()
        {
            string[] input = File.ReadAllLines("Inputs/Day09.txt");

            _input = new long[input.Length];
            _valueToIndexMap = new Dictionary<long, long>(input.Length);
            for (long i = 0; i < input.Length; i++)
            {
                _input[i] = long.Parse(input[i]);
                _valueToIndexMap[_input[i]] = i;
            }
        }

        [Fact]
        public void Part1()
        {
            const int window = 25;
            long i = window;
            long answer = 0;
            while (i < _input.Length)
            {
                long candidate = _input[i];
                long start = i - window;
                bool found = false;

                for (long j = start; j < i; j++)
                {
                    if (_valueToIndexMap.TryGetValue(candidate - _input[j], out long other) 
                        && other >= start && other < i)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    answer = candidate;
                    break;
                }

                i++;
            }

            Assert.Equal(530627549, answer);
        }

        [Fact]
        public void Part2()
        {
            const long target = 530627549;

            int start = 0;
            int end = 1;
            long sum = _input[start] + _input[end];

            while (sum != target)
            {
                if (sum < target)
                    sum += _input[++end];
                else
                    sum -= _input[start++];
            }

            long min = long.MaxValue;
            long max = 0;

            for (int i = start; i <= end; i++)
            {
                long val = _input[i];
                if (val < min)
                    min = val;
                if (val > max)
                    max = val; ;
            }

            long answer = min + max;
            Assert.Equal(77730285, answer);
        }
    }
}
