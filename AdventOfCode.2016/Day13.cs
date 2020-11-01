using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day13
    {
        const int TargetX = 31;
        const int TargetY = 39;
        const int Input = 1364;

        private Dictionary<IntPoint2, bool> _cells = new Dictionary<IntPoint2, bool>();

        [Fact]
        public void Part1()
        {
            int answer = MinSteps((1, 1), (TargetX, TargetY));
            Assert.Equal(86, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = MaxSpaces((1, 1), 50);
            Assert.Equal(127, answer);
        }

        private int MinSteps(in IntPoint2 initial, in IntPoint2 target)
        {
            (IntPoint2 p, int distance) current = (initial, 0);
            HashSet<IntPoint2> visited = new HashSet<IntPoint2>();
            Queue<(IntPoint2, int)> queue = new Queue<(IntPoint2, int)>();
            queue.Enqueue(current);

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                if (visited.Contains(current.p))
                    continue;
                visited.Add(current.p);

                if (current.p.Equals(target))
                    return current.distance;
                else
                {
                    foreach (IntPoint2 adj in AdjacentSpaces(current.p).Where(p => !visited.Contains(p)))
                    {
                        queue.Enqueue((adj, current.distance + 1));
                    }
                }
            }

            throw new InvalidOperationException();
        }

        private int MaxSpaces(in IntPoint2 initial, int maxSteps)
        {
            (IntPoint2 p, int distance) current = (initial, 0);
            HashSet<IntPoint2> visited = new HashSet<IntPoint2>();
            Queue<(IntPoint2, int)> queue = new Queue<(IntPoint2, int)>();
            queue.Enqueue(current);

            while (queue.Count > 0)
            {
                current = queue.Dequeue();

                if (visited.Contains(current.p))
                    continue;
                visited.Add(current.p);

                if (current.distance == maxSteps)
                    continue;

                foreach (IntPoint2 adj in AdjacentSpaces(current.p).Where(p => !visited.Contains(p)))
                {
                    queue.Enqueue((adj, current.distance + 1));
                }
            }

            return visited.Count;
        }

        private IEnumerable<IntPoint2> AdjacentSpaces(in IntPoint2 pos) =>
            pos.Adjacent().Where(p => p.X >= 0 && p.Y >= 0 && !IsWall(p));

        private bool IsWall(in IntPoint2 p)
        {
            if (!_cells.TryGetValue(p, out bool isWall))
            {
                long key = p.X * p.X + 3 * p.X + 2 * p.X * p.Y + p.Y + p.Y * p.Y;
                key += Input;
                int bitCount = BitCount(key);
                isWall = bitCount % 2 == 1;
                _cells[p] = isWall;
            }
            return isWall;
        }

        private static int BitCount(long i)
        {
            //https://stackoverflow.com/questions/2709430/count-number-of-bits-in-a-64-bit-long-big-integer
            i = i - ((i >> 1) & 0x5555555555555555);
            i = (i & 0x3333333333333333) + ((i >> 2) & 0x3333333333333333);
            return (int)((((i + (i >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
        }
    }
}
