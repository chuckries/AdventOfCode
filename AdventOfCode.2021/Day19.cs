using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day19
    {
        List<List<IntVec3>> _scanners;

        public Day19()
        {
            _scanners = new();

            using StreamReader sr = new StreamReader(new FileStream("Inputs/Day19.txt", FileMode.Open, FileAccess.Read));
            _ = sr.ReadLine();

            List<IntVec3> current = new();
            string? line;
            while (true)
            {
                line = sr.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    _scanners.Add(current);
                    if (line is null)
                        break;

                    current = new();
                    _ = sr.ReadLine();
                }
                else
                {
                    string[] tok = line.Split(',');
                    current.Add(new IntVec3(tok[0], tok[1], tok[2]));
                }
            }
        }

        [Fact]
        public void Part1()
        {
            (var points, _) = Solve();
            int answer = points.Count;

            Assert.Equal(440, answer);
        }

        [Fact]
        public void Part2()
        {
            (_, int max) = Solve();
            Assert.Equal(13382, max);
        }

        private (HashSet<IntVec3>, int) Solve()
        {
            HashSet<IntVec3> points = new(_scanners[0]);
            List<List<IntVec3>> unknownScanners = _scanners.Skip(1).ToList();
            List<IntVec3> offsets = new();

            while (unknownScanners.Count > 0)
            {
                List<IntVec3>? foundScanner = null;
                List<IntVec3>? resolvedScanner = null;
                foreach (var candScanner in unknownScanners)
                    for (int i = 0; i < 24; i++)
                    {
                        List<IntVec3> rotation = candScanner.Select(p => RotatePoint(p, i)).ToList();
                        if (TryMatchScanners(points, rotation, out IntVec3 offset))
                        {
                            offsets.Add(offset);
                            foundScanner = candScanner;
                            resolvedScanner = rotation.Select(p => p + offset).ToList();
                            goto found;
                        }
                    }

                throw new InvalidOperationException();

            found:
                unknownScanners.Remove(foundScanner!);
                foreach (IntVec3 p in resolvedScanner!)
                    points.Add(p);
            }

            int maxDistance = int.MinValue;
            foreach (IntVec3 a in offsets)
                foreach (IntVec3 b in offsets)
                {
                    int dist = Abs(a.X - b.X) + Abs(a.Y - b.Y) + Abs(a.Z - b.Z);
                    if (dist > maxDistance)
                        maxDistance = dist;
                }

            return (points, maxDistance);
        }

        private bool TryMatchScanners(ICollection<IntVec3> a, List<IntVec3> b, out IntVec3 scannerOffset)
        {
            scannerOffset = IntVec3.Zero;
            int offsetCount;
            Dictionary<IntVec3, int> offsetCounts = new(a.Count * b.Count);
            foreach (IntVec3 pa in a)
                foreach (IntVec3 pb in b)
                {
                    IntVec3 offset = pa - pb;
                    if (!offsetCounts.TryGetValue(offset, out offsetCount))
                        offsetCounts[offset] = 1;
                    else if(offsetCount == 11)
                    {
                        scannerOffset = offset;
                        return true;
                    }
                    else
                    {
                        offsetCounts[offset] = offsetCount + 1;
                    }
                }

            return false;
        }

        private IntVec3 RotatePoint(IntVec3 p, int rot)
        {
            return rot switch
            {
                0 => new IntVec3(p.X, p.Y, p.Z),

                // y axis
                1 => new IntVec3(-p.Z, p.Y, p.X),
                2 => new IntVec3(-p.X, p.Y, -p.Z),
                3 => new IntVec3(p.Z, p.Y, -p.X),

                // x axis
                4 => new IntVec3(p.X, p.Z, -p.Y),
                5 => new IntVec3(p.X, -p.Y, -p.Z),
                6 => new IntVec3(p.X, -p.Z, p.Y),

                // z axis
                7 => new IntVec3(p.Y, -p.X, p.Z),
                8 => new IntVec3(-p.X, -p.Y, p.Z),
                9 => new IntVec3(-p.Y, p.X, p.Z),

                // lazy... maybe do later?

                10 => RotatePoint(RotatePoint(p, 4), 7),
                11 => RotatePoint(RotatePoint(p, 4), 8),
                12 => RotatePoint(RotatePoint(p, 4), 9),

                13 => RotatePoint(RotatePoint(p, 7), 4),
                14 => RotatePoint(RotatePoint(p, 7), 5),
                15 => RotatePoint(RotatePoint(p, 7), 6),

                16 => RotatePoint(RotatePoint(p, 6), 7),
                17 => RotatePoint(RotatePoint(p, 6), 8),
                18 => RotatePoint(RotatePoint(p, 6), 9),

                19 => RotatePoint(RotatePoint(p, 9), 4),
                20 => RotatePoint(RotatePoint(p, 9), 5),
                21 => RotatePoint(RotatePoint(p, 9), 6),

                22 => RotatePoint(RotatePoint(p, 5), 1),
                23 => RotatePoint(RotatePoint(p, 8), 1),

                _ => throw new InvalidOperationException()
            };
        }
    }
}
