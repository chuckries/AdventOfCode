﻿using AdventOfCode.Common;
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
            public IntVec3 P { get; set; }
            public IntVec3 V { get; set; }

            public Moon(IntVec3 position)
            {
                P = position;
                V = IntVec3.Zero;
            }
        }

        Moon[] _moons;

        public Day12()
        {
            List<Moon> moons = new List<Moon>();
            foreach (string line in File.ReadAllLines("Inputs/Day12.txt"))
            {
                Match match = s_Regex.Match(line);
                moons.Add(new Moon(new IntVec3(
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
            IntVec3[] initialPositions = _moons.Select(m => m.P).ToArray();

            long xPeriod = 0;
            long yPeriod = 0;
            long zPeriod = 0;

            bool IsMatch(
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
                return isMatch;
            }

            int ticks = 0;
            do
            {
                Tick();
                ticks++;

                if (xPeriod == 0 && IsMatch(moon => moon.P.X, moon => moon.V.X, i => initialPositions[i].X))
                    xPeriod = ticks;

                if (yPeriod == 0 && IsMatch(moon => moon.P.Y, moon => moon.V.Y, i => initialPositions[i].Y))
                    yPeriod = ticks;

                if (zPeriod == 0 && IsMatch(moon => moon.P.Z, moon => moon.V.Z, i => initialPositions[i].Z))
                    zPeriod = ticks;
            }
            while (xPeriod == 0 || yPeriod == 0 || zPeriod == 0);

            long answer = MathUtils.LeastCommonMultiple(xPeriod, yPeriod, zPeriod);

            Assert.Equal(334945516288044, answer);
        }

        private void Tick()
        {
            foreach ((Moon left, Moon right) in _moons.UniquePairs())
            {
                IntVec3 delta = new IntVec3(
                    Math.Sign(right.P.X - left.P.X),
                    Math.Sign(right.P.Y - left.P.Y),
                    Math.Sign(right.P.Z - left.P.Z)
                    );

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
