using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
            public readonly IntPoint3 P;
            public readonly int R;

            public int Manhattan => Abs(P.X) + Abs(P.Y) + Abs(P.Z);

            public Bot(int x, int y, int z, int r)
            {
                P = new IntPoint3(x, y, z);
                R = r;
            }

            public bool InRange(in IntPoint3 p)
            {
                return P.Distance(p) <= R;
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

        [DebuggerDisplay("({Min}, {Max})")]
        class BoundingBox
        {
            public readonly IntPoint3 Min;
            public readonly IntPoint3 Max;
            public readonly IntPoint3 Center;
            public readonly bool IsPoint;
            public readonly long Volume;

            public BoundingBox(IntPoint3 min, IntPoint3 max)
            {
                if (min.X > max.X || min.Y > max.Y || min.Z > max.Z)
                    throw new InvalidOperationException("min is greater than max");

                Min = min;
                Max = max;
                Center = Min + ((Max - Min) / 2);
                IsPoint = Min.X == Max.X && Min.Y == Max.Y && Min.Z == Max.Z;
                Volume = (Max.X - Min.X) * (Max.Y - Min.Y) * (Max.Z - Min.Z);
            }

            public IntPoint3 Closest(in IntPoint3 point)
            {
                int x = Clamp(point.X, Min.X, Max.X);
                int y = Clamp(point.Y, Min.Y, Max.Y);
                int z = Clamp(point.Z, Min.Z, Max.Z);

                return new IntPoint3(x, y, z);
            }

            public IEnumerable<BoundingBox> SubBoxes()
            {
                if (!IsPoint)
                {
                    bool splitX = Max.X != Center.X;
                    bool splitY = Max.Y != Center.Y;
                    bool splitZ = Max.Z != Center.Z;

                    yield return new BoundingBox(new IntPoint3(Min.X, Min.Y, Min.Z), new IntPoint3(Center.X, Center.Y, Center.Z));
                    if (splitX)
                        yield return new BoundingBox(new IntPoint3(Center.X + 1, Min.Y, Min.Z), new IntPoint3(Max.X, Center.Y, Center.Z));
                    if (splitY)
                        yield return new BoundingBox(new IntPoint3(Min.X, Center.Y + 1, Min.Z), new IntPoint3(Center.X, Max.Y, Center.Z));
                    if (splitZ)
                        yield return new BoundingBox(new IntPoint3(Min.X, Min.Y, Center.Z + 1), new IntPoint3(Center.X, Center.Y, Max.Z));
                    if (splitX && splitY)
                        yield return new BoundingBox(new IntPoint3(Center.X + 1, Center.Y + 1, Min.Z), new IntPoint3(Max.X, Max.Y, Center.Z));
                    if (splitX && splitZ)
                        yield return new BoundingBox(new IntPoint3(Center.X + 1, Min.Y, Center.Z + 1), new IntPoint3(Max.X, Center.Y, Max.Z));
                    if (splitY && splitZ)
                        yield return new BoundingBox(new IntPoint3(Min.X, Center.Y + 1, Center.Z + 1), new IntPoint3(Center.X, Max.Y, Max.Z));
                    if (splitX && splitY && splitZ)
                        yield return new BoundingBox(new IntPoint3(Center.X + 1, Center.Y + 1, Center.Z + 1), new IntPoint3(Max.X, Max.Y, Max.Z));
                }
            }
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
                if (maxBot.InRange(bot.P))
                    total++;

            Assert.Equal(270, total);
        }

        [Fact]
        public void Part2()
        {
            int minX, maxX, minY, maxY, minZ, maxZ;
            minX = minY = minZ = int.MaxValue;
            maxX = maxY = maxZ = int.MinValue;
            foreach (Bot bot in _bots)
            {
                if (bot.P.X < minX) minX = bot.P.X;
                if (bot.P.X > maxX) maxX = bot.P.X;
                if (bot.P.Y < minY) minY = bot.P.Y;
                if (bot.P.Y > maxY) maxY = bot.P.Y;
                if (bot.P.Z < minZ) minZ = bot.P.Z;
                if (bot.P.Z > maxZ) maxZ = bot.P.Z;
            }

            BoundingBox initialBox = new BoundingBox(new IntPoint3(minX, minY, minZ), new IntPoint3(maxX, maxY, maxZ));

            var SearchSet = new List<(BoundingBox box, int botsInRage)> { (initialBox, _bots.Length) };

            IntPoint3 answer = IntPoint3.Zero;
            for (; ;)
            {
                (BoundingBox current, _) = SearchSet[SearchSet.Count - 1];
                SearchSet.RemoveAt(SearchSet.Count - 1);

                if (current.IsPoint)
                {
                    answer = current.Center;
                    break;
                }
                else
                {
                    foreach (BoundingBox subBox in current.SubBoxes())
                    {
                        int botsInRange = 0;
                        foreach (Bot bot in _bots)
                            if (bot.InRange(subBox.Closest(bot.P)))
                                botsInRange++;

                        SearchSet.Add((subBox, botsInRange));
                    }

                    SearchSet.Sort((left, right) =>
                    {
                        int value = left.botsInRage - right.botsInRage;
                        if (value == 0)
                        {
                            value = (int)(right.box.Volume - left.box.Volume);
                            if (value == 0)
                            {
                                value = right.box.Center.Manhattan - left.box.Center.Manhattan;
                            }
                        }

                        return value;
                    });
                }
            }

            Assert.Equal(0, answer.Manhattan);
        }

        [Fact]
        public void BoundingBox_PointBox_CenterIsPoint()
        {
            BoundingBox box = new BoundingBox(new IntPoint3(1, 1, 1), new IntPoint3(1, 1, 1));
            Assert.True(box.IsPoint);
            Assert.Equal(new IntPoint3(1, 1, 1), box.Center);
            Assert.Empty(box.SubBoxes().ToArray());
        }

        [Fact]
        public void BoundBox_8PointBox_DividesCorrectly()
        {
            BoundingBox box = new BoundingBox(new IntPoint3(1, 1, 1), new IntPoint3(2, 2, 2));
            BoundingBox[] subs = box.SubBoxes().ToArray();

            Assert.Equal(8, subs.Length);
        }
    }
}
