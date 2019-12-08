using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using static System.Math;

namespace AdventOfCode._2018
{
    public class Day23
    {
        class Bot
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Z;
            public readonly int R;

            public int Manhattan => Abs(X) + Abs(Y) + Abs(Z);

            public Bot(int x, int y, int z, int r)
            {
                X = x;
                Y = y;
                Z = z;
                R = r;
            }

            public bool InRange(int x, int y, int z)
            {
                return Abs(X - x) + Abs(Y - y) + Abs(Z - z) <= R;
            }

            public static Bot Parse(string str)
            {
                Match match = _regex.Match(str);
                return new Bot(
                    int.Parse(match.Groups["X"].Value),
                    int.Parse(match.Groups["Y"].Value),
                    int.Parse(match.Groups["Z"].Value),
                    int.Parse(match.Groups["R"].Value)
                    );
            }

            static Regex _regex = new Regex(@"^pos=\<(?<X>-?[0-9]+),(?<Y>-?[0-9]+),(?<Z>-?[0-9]+)\>, r=(?<R>[0-9]+)$", RegexOptions.Compiled);
        }

        Bot[] _bots = File.ReadAllLines("Inputs/Day23.txt")
            .Select(Bot.Parse)
            .ToArray();

        [Fact]
        public void Part1()
        {
            int maxRange = 0;
            Bot maxBot = null;
            for (int i = 0; i < _bots.Length; i++)
                if (_bots[i].R > maxRange)
                {
                    maxBot = _bots[i];
                    maxRange = maxBot.R;
                }

            int total = 0;
            foreach (Bot bot in _bots)
                if (maxBot.InRange(bot.X, bot.Y, bot.Z))
                    total++;

            Assert.Equal(270, total);
        }
    }
}
