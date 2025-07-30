using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Math;

namespace AdventOfCode._2018;

public class Day23
{
    class Bot
    {
        public readonly IntVec3 P;
        public readonly int R;

        public int Manhattan => Abs(P.X) + Abs(P.Y) + Abs(P.Z);

        public Bot(int x, int y, int z, int r)
        {
            P = (x, y, z);
            R = r;
        }

        public bool InRange(IntVec3 p)
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

        static Regex _regex = new Regex(@"^pos=<(?'X'-?\d+),(?'Y'-?\d+),(?'Z'-?\d+)>, r=(?'R'\d+)$", RegexOptions.Compiled);
    }

    [DebuggerDisplay("({Min}, {Max})")]
    readonly struct BoundingBox
    {
        public readonly IntVec3 Min;
        public readonly IntVec3 Max;
        public readonly IntVec3 Center;
        public readonly bool IsPoint;

        public BoundingBox(in IntVec3 min, in IntVec3 max)
        {
            if (min.X > max.X || min.Y > max.Y || min.Z > max.Z)
                throw new InvalidOperationException("min is greater than max");

            Min = min;
            Max = max;
            Center = Min + ((Max - Min) / 2);
            IsPoint = min.X == max.X && min.Y == max.Y && min.Z == max.Z;
        }

        public IntVec3 Closest(in IntVec3 point)
        {
            int x = Clamp(point.X, Min.X, Max.X);
            int y = Clamp(point.Y, Min.Y, Max.Y);
            int z = Clamp(point.Z, Min.Z, Max.Z);

            return new IntVec3(x, y, z);
        }

        public IEnumerable<BoundingBox> SubBoxes()
        {
            if (!IsPoint)
            {
                bool splitX = Max.X != Center.X;
                bool splitY = Max.Y != Center.Y;
                bool splitZ = Max.Z != Center.Z;

                yield return new BoundingBox((Min.X, Min.Y, Min.Z), (Center.X, Center.Y, Center.Z));
                if (splitX)
                    yield return new BoundingBox((Center.X + 1, Min.Y, Min.Z), (Max.X, Center.Y, Center.Z));
                if (splitY)
                    yield return new BoundingBox((Min.X, Center.Y + 1, Min.Z), (Center.X, Max.Y, Center.Z));
                if (splitZ)
                    yield return new BoundingBox((Min.X, Min.Y, Center.Z + 1), (Center.X, Center.Y, Max.Z));
                if (splitX && splitY)
                    yield return new BoundingBox((Center.X + 1, Center.Y + 1, Min.Z), (Max.X, Max.Y, Center.Z));
                if (splitX && splitZ)
                    yield return new BoundingBox((Center.X + 1, Min.Y, Center.Z + 1), (Max.X, Center.Y, Max.Z));
                if (splitY && splitZ)
                    yield return new BoundingBox((Min.X, Center.Y + 1, Center.Z + 1), (Center.X, Max.Y, Max.Z));
                if (splitX && splitY && splitZ)
                    yield return new BoundingBox((Center.X + 1, Center.Y + 1, Center.Z + 1), (Max.X, Max.Y, Max.Z));
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
        (IntVec3 min, IntVec3 max) = IntVec3.MinMax(_bots.Select(b => b.P));
        BoundingBox initialBox = new BoundingBox(min, max);

        var comparer = Comparer<(BoundingBox box, int botsInRange)>.Create((left, right) =>
        {
            int value = right.botsInRange - left.botsInRange;
            if (value == 0)
            {
                value = left.box.Center.Manhattan - right.box.Center.Manhattan;
            }

            return value;
        });

        var searchSet = new PriorityQueue<BoundingBox, (BoundingBox, int)>(comparer);
        searchSet.Enqueue(initialBox, (initialBox, _bots.Length));

        IntVec3 answer = IntVec3.Zero;
        while (searchSet.Count > 0)
        {
            BoundingBox current = searchSet.Dequeue();

            if (current.IsPoint)
            {
                answer = current.Center;
                break;
            }

            foreach (BoundingBox subBox in current.SubBoxes())
            {
                int botsInRange = _bots.Count(b => b.InRange(subBox.Closest(b.P)));
                searchSet.Enqueue(subBox, (subBox, botsInRange));
            }
        }

        Assert.Equal(106323091, answer.Manhattan);
    }
}
