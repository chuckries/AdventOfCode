using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2017
{
    public class Day24
    {
        private readonly HashSet<IntVec2> _initial;
        private readonly HashSet<IntVec2> _available;
        private readonly int _initialAvailablePort;

        public Day24()
        {
            IntVec2[] input = File.ReadAllLines("Inputs/Day24.txt")
                .Select(line => line.Split('/'))
                .Select(tok => new IntVec2(tok[0], tok[1]))
                .ToArray();

            IntVec2 start = input.Single(p => p.X == 0 || p.Y == 0);
            _initial = new();
            _initial.Add(start);

            _available = new HashSet<IntVec2>(input);
            _available.Remove(start);

            _initialAvailablePort = start.X != 0 ? start.X : start.Y;
        }

        [Fact]
        public void Part1()
        {
            int max = int.MinValue;
            BackTrack(_initial, _initialAvailablePort, _available, terminal =>
            {
                int strength = terminal.Sum(p => p.X + p.Y);
                if (strength > max)
                    max = strength;
            });

            Assert.Equal(1695, max);
        }

        [Fact]
        public void Part2()
        {
            int maxLegth = int.MinValue;
            int maxStrength = int.MinValue;
            BackTrack(_initial, _initialAvailablePort, _available, terminal =>
            {
                if (terminal.Count >= maxLegth)
                {
                    maxLegth = terminal.Count;
                    int strength = terminal.Sum(p => p.X + p.Y);
                    if (strength > maxStrength)
                        maxStrength = strength;
                }
            });

            Assert.Equal(1673, maxStrength);
        }

        static void BackTrack(HashSet<IntVec2> bridge, int currentPort, HashSet<IntVec2> available, Action<HashSet<IntVec2>> terminal)
        {
            IntVec2[] candidates = available.Where(a => a.X == currentPort || a.Y == currentPort).ToArray();
            if (candidates.Length == 0)
                terminal(bridge);
            else
                foreach (IntVec2 cand in candidates)
                {
                    bridge.Add(cand);
                    available.Remove(cand);

                    int newCurrentPort = currentPort == cand.X ? cand.Y : cand.X;

                    BackTrack(bridge, newCurrentPort, available, terminal);

                    bridge.Remove(cand);
                    available.Add(cand);
                }
        }
    }
}
