using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day02
    {
        [Fact]
        public void Part1()
        {
            int answer = Parse().
                Select(p =>
                {
                    int i = p.X * p.Y;
                    int j = p.X * p.Z;
                    int k = p.Y * p.Z;

                    int min = Math.Min(Math.Min(i, j), k);

                    return 2 * i + 2 * j + 2 * k + min;
                })
                .Sum();

            Assert.Equal(1588178, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Parse()
                .Select(p =>
                {
                    int i = p.X + p.Y;
                    int j = p.X + p.Z;
                    int k = p.Y + p.Z;

                    int min = Math.Min(Math.Min(i, j), k);

                    return 2 * min + p.X * p.Y *p.Z;
                })
                .Sum();

            Assert.Equal(3783758, answer);
        }

        private IEnumerable<IntPoint3> Parse()
        {
            return File.ReadAllLines("Inputs/Day02.txt").Select(Parse);
        }

        private IntPoint3 Parse(string line)
        {
            Match match = s_Regex.Match(line);
            return new IntPoint3(
                int.Parse(match.Groups["X"].Value),
                int.Parse(match.Groups["Y"].Value),
                int.Parse(match.Groups["Z"].Value));
        }

        static Regex s_Regex = new Regex(@"^(?'X'\d+)x(?'Y'\d+)x(?'Z'\d+)$", RegexOptions.Compiled);
    }
}
