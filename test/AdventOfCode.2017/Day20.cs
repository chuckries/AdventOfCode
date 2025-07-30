using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

using static System.Math;

namespace AdventOfCode._2017
{
    public class Day20
    {
        class Particle
        {
            private IntVec3 _p;
            private IntVec3 _v;
            private IntVec3 _a;

            public IntVec3 P => _p;

            public bool Dead { get; set; } = false;

            public int Manhattan => Abs(_p.X) + Abs(_p.Y) + Abs(_p.Z);

            public Particle(IntVec3 p, IntVec3 v, IntVec3 a)
            {
                _p = p;
                _v = v;
                _a = a;
            }

            public static Particle Parse(string str)
            {
                GroupCollection g = s_Regex.Match(str).Groups;
                return new Particle(
                    new IntVec3(int.Parse(g["px"].Value), int.Parse(g["py"].Value), int.Parse(g["pz"].Value)),
                    new IntVec3(int.Parse(g["vx"].Value), int.Parse(g["vy"].Value), int.Parse(g["vz"].Value)),
                    new IntVec3(int.Parse(g["ax"].Value), int.Parse(g["ay"].Value), int.Parse(g["az"].Value)));
            }

            public void Tick()
            {
                _v += _a;
                _p += _v;
            }

            private static Regex s_Regex = new Regex(
                @"^p=<(?'px'-?[0-9]+),(?'py'-?[0-9]+),(?'pz'-?[0-9]+)>, v=<(?'vx'-?[0-9]+),(?'vy'-?[0-9]+),(?'vz'-?[0-9]+)>, a=<(?'ax'-?[0-9]+),(?'ay'-?[0-9]+),(?'az'-?[0-9]+)>$",
                RegexOptions.Compiled);
        }

        private readonly Particle[] _particles;

        public Day20()
        {
            _particles = File.ReadAllLines("Inputs/Day20.txt").Select(Particle.Parse).ToArray();
        }

        [Fact]
        public void Part1()
        {
            for (int i = 0; i < 350; i++)
                foreach (Particle part in _particles)
                    part.Tick();

            int minDist = int.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < _particles.Length; i++)
            {
                int dist = _particles[i].Manhattan;
                if (dist < minDist)
                {
                    minDist = dist;
                    minIndex = i;
                }
            }

            Assert.Equal(308, minIndex);
        }

        [Fact]
        public void Part2()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < _particles.Length - 1; j++)
                    if (!_particles[j].Dead)
                        for (int k = j + 1; k < _particles.Length; k++)
                        {
                            if (!_particles[k].Dead && _particles[j].P == _particles[k].P)
                            {
                                _particles[j].Dead = true;
                                _particles[k].Dead = true;
                            }
                        }

                foreach (Particle part in _particles)
                    if (!part.Dead)
                        part.Tick();
            }

            int answer = _particles.Count(p => !p.Dead);
            Assert.Equal(504, answer);
        }
    }
}
