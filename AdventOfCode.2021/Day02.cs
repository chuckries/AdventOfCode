using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2021
{
    public class Day02
    {
        IEnumerable<(string, int)> _commands;

        public Day02()
        {
            _commands = File.ReadAllLines("Inputs/Day02.txt")
                .Select(s => s.Split())
                .Select(tok => (tok[0], int.Parse(tok[1])));
        }

        [Fact]
        public void Part1()
        {
            (int x, int y) pos = (0, 0);
            foreach (var command in _commands)
            {
                switch (command.Item1)
                {
                    case "forward": pos.x += command.Item2; break;
                    case "down": pos.y += command.Item2; break;
                    case "up": pos.y -= command.Item2; break;
                    default: throw new InvalidOperationException();
                }
            }

            int answer = pos.x * pos.y;
            Assert.Equal(2102357, answer);
        }

        [Fact]
        public void Part2()
        {
            (int x, int y, int aim) sub = (0, 0, 0);
            foreach (var command in _commands)
            {
                switch (command.Item1)
                {
                    case "forward":
                        sub.x += command.Item2;
                        sub.y += command.Item2 * sub.aim;
                        break;
                    case "down": sub.aim += command.Item2; break;
                    case "up": sub.aim -= command.Item2; break;
                    default: throw new InvalidOperationException();
                }
            }

            int answer = sub.x * sub.y;
            Assert.Equal(2101031224, answer);
        }
    }
}
