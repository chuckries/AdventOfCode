using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using static System.Math;

namespace AdventOfCode._2017
{
    public class Day11
    {
        string[] _input;

        public Day11()
        {
            _input = File.ReadAllText("Inputs/Day11.txt").Split(',').ToArray();
        }

        [Fact]
        public void Part1()
        {
            (int q, int r, int s) p = (0, 0, 0);

            foreach (string dir in _input)
                Move(dir, ref p);

            int answer = Distance(p);
            Assert.Equal(670, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Steps().Select(Distance).Max();
            Assert.Equal(1426, answer);

            IEnumerable<(int, int, int)> Steps()
            {
                (int q, int r, int s) p = (0, 0, 0);
                foreach (string dir in _input)
                {
                    Move(dir, ref p);
                    yield return p;
                }
            }
        }

        private static void Move(string dir, ref (int q, int r, int s) p)
        {
            switch (dir)
            {
                case "n":   p.r--; p.s++; break;
                case "ne":  p.q++; p.r--; break;
                case "se":  p.q++; p.s--; break;
                case "s":   p.r++; p.s--; break;
                case "sw":  p.q--; p.r++; break;
                case "nw":  p.q--; p.s++; break;
                default: throw new InvalidOperationException();
            }
        }

        private static int Distance((int q, int r, int s) p) =>
            (Abs(p.q) + Abs(p.r) + Abs(p.s)) / 2;
    }
}
