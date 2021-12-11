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
            public record struct Vec3(int X, int Y, int Z);

            private Vec3 _p;
            private Vec3 _v;
            private Vec3 _a;

            public Vec3 P => _p;

            public bool Dead { get; set; } = false;

            public Particle(int px, int py, int pz, int vx, int vy, int vz, int ax, int ay, int az)
            {
                _p = new(); _v = new(); _a = new();
                _p.X = px; _p.Y = py; _p.Z = pz;
                _v.X = vx; _v.Y = vy; _v.Z = vz;
                _a.X = ax; _a.Y = ay; _a.Z = az;
            }

            public void Tick()
            {
                _v.X += _a.X;
                _v.Y += _a.Y;
                _v.Z += _a.Z;
                _p.X += _v.X;
                _p.Y += _v.Y;
                _p.Z += _v.Z;
            }

            public int Manhattan => Abs(_p.X) + Abs(_p.Y) + Abs(_p.Z);
        }

        private readonly Particle[] _particles;

        public Day20()
        {
            string[] lines = File.ReadAllLines("Inputs/Day20.txt");
            _particles = lines.Select(s =>
            {
                GroupCollection g = s_Regex.Match(s).Groups;
                return new Particle(
                    int.Parse(g["px"].Value), int.Parse(g["py"].Value), int.Parse(g["pz"].Value),
                    int.Parse(g["vx"].Value), int.Parse(g["vy"].Value), int.Parse(g["vz"].Value),
                    int.Parse(g["ax"].Value), int.Parse(g["ay"].Value), int.Parse(g["az"].Value)
                );
            }).ToArray();
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

        private static Regex s_Regex = new Regex(
            @"^p=<(?'px'-?[0-9]+),(?'py'-?[0-9]+),(?'pz'-?[0-9]+)>, v=<(?'vx'-?[0-9]+),(?'vy'-?[0-9]+),(?'vz'-?[0-9]+)>, a=<(?'ax'-?[0-9]+),(?'ay'-?[0-9]+),(?'az'-?[0-9]+)>$",
            RegexOptions.Compiled);
    }
}
