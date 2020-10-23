using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode._2015
{
    public class Day12
    {
        string _input;

        public Day12()
        {
            _input = File.ReadAllText("Inputs/Day12.txt");
        }

        [Fact]
        public void Part1()
        {
            Regex regex = new Regex(@"-?\d+", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(_input);

            int total = matches.Select(m => int.Parse(m.Value)).Sum();

            Assert.Equal(111754, total);
        }

        [Fact]
        public void Part2()
        {

        }

        private char NextTokenIndex(ref int index)
        {
            index = _input.IndexOfAny(s_Tokens);

            if (index == -1)
                throw new InvalidOperationException();

            return _input[index];
        }

        static char[] s_Tokens = { ':', '{', '}', '[', ']' };
    }
}
