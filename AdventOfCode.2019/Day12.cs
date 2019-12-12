using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day12
    {
        class Moon
        {
            public IntPoint3 Position { get; set; }
            public IntPoint3 Velocity { get; set; }

            public Moon(IntPoint3 position)
            {
                Position = position;
                Velocity = IntPoint3.Zero;
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

            int answer = _moons.Select(m => m.Position.Manhattan * m.Velocity.Manhattan).Sum();

            Assert.Equal(7687, answer);
        }

        [Fact]
        public void Part2()
        {
            IntPoint3[] initialPos = _moons.Select(m => m.Position).ToArray();
            IntPoint3[] initialVel = _moons.Select(m => m.Velocity).ToArray();

            //HashSet<(IntPoint3, IntPoint3)> states = new HashSet<(IntPoint3, IntPoint3)>();
            HashSet<(int, int)> statesX0 = new HashSet<(int, int)>();

            int ticks = 0;
            int lastTicks = 0;
            int deltaTicks = 0;
            for (; ;)
            {
                while (!statesX0.Contains((_moons[0].Position.X, _moons[0].Velocity.X)))
                {
                    statesX0.Add((_moons[0].Position.X, _moons[0].Velocity.X));
                    Tick();
                    ticks++;
                }
                //while (!states.Contains((_moons[0].Position, _moons[0].Velocity)))
                //{
                //    states.Add((_moons[0].Position, _moons[0].Velocity));
                //    Tick();
                //    ticks++;
                //}
                //do
                //{
                //    Tick();
                //    ticks++;
                //} while (!(_moons[0].Position.Equals(initialPos[0]) && _moons[0].Velocity.Equals(initialVel[0])));

                int i = 0;
            }
        }

        private void Tick()
        {
            foreach ((Moon left, Moon right) in _moons.UniquePairs())
            {
                if (left.Position.X < right.Position.X)
                {
                    left.Velocity += IntPoint3.UnitX;
                    right.Velocity -= IntPoint3.UnitX;
                }
                else if (left.Position.X > right.Position.X)
                {
                    left.Velocity -= IntPoint3.UnitX;
                    right.Velocity += IntPoint3.UnitX;
                }

                if (left.Position.Y < right.Position.Y)
                {
                    left.Velocity += IntPoint3.UnitY;
                    right.Velocity -= IntPoint3.UnitY;
                }
                else if (left.Position.Y > right.Position.Y)
                {
                    left.Velocity -= IntPoint3.UnitY;
                    right.Velocity += IntPoint3.UnitY;
                }

                if (left.Position.Z < right.Position.Z)
                {
                    left.Velocity += IntPoint3.UnitZ;
                    right.Velocity -= IntPoint3.UnitZ;
                }
                else if (left.Position.Z > right.Position.Z)
                {
                    left.Velocity -= IntPoint3.UnitZ;
                    right.Velocity += IntPoint3.UnitZ;
                }
            }

            foreach (Moon moon in _moons)
            {
                moon.Position += moon.Velocity;
            }
        }

        private static Regex s_Regex = new Regex(@"^\<x=(?<X>-?\d+), y=(?<Y>-?\d+), z=(?<Z>-?\d+)\>$", RegexOptions.Compiled);
    }
}
