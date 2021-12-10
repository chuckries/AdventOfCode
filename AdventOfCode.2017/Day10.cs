using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2017
{
    public class Day10
    {
        private class KnotHash
        {
            private int[] _list;
            private int _current;
            private int _skip;

            public KnotHash(int size)
            {
                _list = Enumerable.Range(0, size).ToArray();
                _current = 0;
                _skip = 0;
            }

            public void Apply(int length)
            {
                int begin = _current;
                int end = begin + length - 1;

                for (int i = 0; i < length / 2; i++)
                {
                    int tmp = _list[Index(begin)];
                    _list[Index(begin)] = _list[Index(end)];
                    _list[Index(end)] = tmp;

                    begin++;
                    end--;
                }

                Advance(length);
            }

            public IEnumerable<int> Enumerate() => _list;

            private void Advance(int length)
            {
                _current = Index(_current + length + _skip++);
            }

            private int Index(int i) => i % _list.Length;
        }

        [Fact]
        public void Part1()
        {
            int[] lengths = File.ReadAllText("Inputs/Day10.txt").Split(',').Select(int.Parse).ToArray();

            KnotHash hash = new KnotHash(256);
            foreach (int length in lengths)
                hash.Apply(length);

            int answer = hash.Enumerate().Take(2).Aggregate((x, y) => x * y);

            Assert.Equal(23715, answer);
        }

        [Fact]
        public void Part2()
        {
            int[] lengths = File.ReadAllText("Inputs/Day10.txt").Select(c => (int)c).Concat(new[] { 17, 31, 73, 47, 23 }).ToArray();
            KnotHash hash = new KnotHash(256);

            for (int i = 0; i < 64; i++)
                foreach (int length in lengths)
                    hash.Apply(length);

            string answer = hash.Enumerate()
                .Chunk(16).Select(chunk => chunk.Aggregate((x, y) => x ^ y))
                .Select(n => string.Format("{0:x2}", n))
                .Aggregate(new StringBuilder(), (sb, s) => sb.Append(s), sb => sb.ToString());

            Assert.Equal("541dc3180fd4b72881e39cf925a50253", answer);
        }
    }
}
