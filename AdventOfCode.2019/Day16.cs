using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day16
    {
        int[] _current = File.ReadAllText("Inputs/Day16.txt")
            .Select(c => c - '0')
            .ToArray();
        int[] _next;
        IEnumerator<int>[] _generators;

        static int[] s_pattern = new[] { 0, 1, 0, -1 };

        public Day16()
        {
            _next = (int[])_current.Clone();
            _generators = new IEnumerator<int>[_current.Length];
        }

        [Fact]
        public void Part1()
        {
            for (int i = 0; i < 100; i++)
                Tick();

            string answer = new string(_current.Take(8).Select(i => (char)(i + '0')).ToArray());

            Assert.Equal("84487724", answer);
        }

        private void Tick()
        {
            GetEnumerators(_generators);
            for (int i = 0; i < _current.Length; i++)
            {
                _next[i] = 0;
                for (int j = 0; j < _current.Length; j++)
                {
                    _next[i] += _current[j] * _generators[i].Current;
                    _generators[i].MoveNext();
                }
                _next[i] = Math.Abs(_next[i]) % 10;
            }

            var temp = _current;
            _current = _next;
            _next = temp;
        }

        private void GetEnumerators(IEnumerator<int>[] generators)
        {
            for (int i = 0; i < generators.Length; i++)
            {
                generators[i] = GetPattern(i + 1);
                generators[i].MoveNext();
                generators[i].MoveNext();
            }
        }

        private IEnumerator<int> GetPattern(int repeat)
        {
            for(; ;)
            {
                foreach (int number in s_pattern)
                    for (int i = 0; i < repeat; i++)
                        yield return number;
            }
        }
    }
}
