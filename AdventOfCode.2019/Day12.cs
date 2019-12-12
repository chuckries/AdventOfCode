using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day12
    {
        class Moon
        {
            public IntPoint3 P { get; set; }
            public IntPoint3 V { get; set; }

            public Moon(IntPoint3 position)
            {
                P = position;
                V = IntPoint3.Zero;
            }
        }

        Moon[] _moons;

        public Day12()
        {
            List<Moon> moons = new List<Moon>();
            foreach (string line in File.ReadAllLines("Inputs/Day12.txt"))
            {
                Match match = s_Regex.Match(line);
                moons.Add(new Moon(new IntPoint3(
                    int.Parse(match.Groups["X"].Value),
                    int.Parse(match.Groups["Y"].Value),
                    int.Parse(match.Groups["Z"].Value))));
            }
            _moons = moons.ToArray();
        }

        [Fact]
        public void Part1()
        {
            for (int i = 0; i < 1000; i++)
                Tick();

            int answer = _moons.Select(m => m.P.Manhattan * m.V.Manhattan).Sum();

            Assert.Equal(7687, answer);
        }

        [Fact]
        public void Part2()
        {
            IntPoint3[] initialPositions = _moons.Select(m => m.P).ToArray();

            long? xPeriod = null;
            long? yPeriod = null;
            long? zPeriod = null;

            void CheckMatch(
                ref long? period,
                int ticks,
                Func<Moon, int> getPosition, 
                Func<Moon, int> getVelocity, 
                Func<int, int> getInitialPosition
                )
            {
                bool isMatch = true;
                for (int i = 0; i < _moons.Length; i++)
                {
                    if (getPosition(_moons[i]) != getInitialPosition(i) || getVelocity(_moons[i]) != 0)
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                    period = ticks;
            }

            int ticks = 0;
            do
            {
                Tick();
                ticks++;

                if (!xPeriod.HasValue)
                    CheckMatch(ref xPeriod, ticks, moon => moon.P.X, moon => moon.V.X, i => initialPositions[i].X);

                if (!yPeriod.HasValue)
                    CheckMatch(ref yPeriod, ticks, moon => moon.P.Y, moon => moon.V.Y, i => initialPositions[i].Y);

                if (!zPeriod.HasValue)
                    CheckMatch(ref zPeriod, ticks, moon => moon.P.Z, moon => moon.V.Z, i => initialPositions[i].Z);
            }
            while (!(xPeriod.HasValue && yPeriod.HasValue && zPeriod.HasValue));

            long answer = MathUtils.LeastCommonMultiple(xPeriod.Value, MathUtils.LeastCommonMultiple(yPeriod.Value, zPeriod.Value));

            Assert.Equal(334945516288044, answer);
        }

        private void Tick()
        {
            foreach ((Moon left, Moon right) in _moons.UniquePairs())
            {
                IntPoint3 delta = IntPoint3.Zero;

                delta += left.P.X < right.P.X ? IntPoint3.UnitX : left.P.X > right.P.X ? -IntPoint3.UnitX : IntPoint3.Zero;
                delta += left.P.Y < right.P.Y ? IntPoint3.UnitY : left.P.Y > right.P.Y ? -IntPoint3.UnitY : IntPoint3.Zero;
                delta += left.P.Z < right.P.Z ? IntPoint3.UnitZ : left.P.Z > right.P.Z ? -IntPoint3.UnitZ : IntPoint3.Zero;

                left.V += delta;
                right.V -= delta;
            }

            foreach (Moon moon in _moons)
            {
                moon.P += moon.V;
            }
        }

        private static Regex s_Regex = new Regex(@"^\<x=(?<X>-?\d+), y=(?<Y>-?\d+), z=(?<Z>-?\d+)\>$", RegexOptions.Compiled);
    }
}
