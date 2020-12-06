using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;

namespace AdventOfCode._2020
{
    public class Day06
    {
        [Fact]
        public void Part1()
        {
            int answer = Reduce((x, y) => x | y);

            Assert.Equal(6310, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Reduce((x, y) => x & y);

            Assert.Equal(3193, answer);
        }

        public int Reduce(Func<int, int, int> op)
        {
            return Parse().Sum(r => r.Aggregate(op).BitCount());
        }

        private IEnumerable<IEnumerable<int>> Parse()
        {
            using (StreamReader sr = new StreamReader("Inputs/Day06.txt"))
            {
                while (!sr.EndOfStream)
                    yield return InnerParse(sr);
            }

            static IEnumerable<int> InnerParse(StreamReader sr)
            {
                string line;
                while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                    yield return line.Aggregate(0, (t, c) => t | 1 << (c - 'a'));
            }
        }
    }
}
