using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;
using System.Numerics;
using System.Diagnostics;

namespace AdventOfCode._2020
{
    public class Day13
    {
        private long _target;
        private (long id, int index)[] _busses;

        public Day13()
        {
            string[] input = File.ReadAllLines("Inputs/Day13.txt");
            _target = long.Parse(input[0]);
            string[] strings = input[1].Split(',');
            List<(long, int)> busses = new (strings.Length);
            for (int i = 0; i < strings.Length; i++)
            {
                if (long.TryParse(strings[i], out long id))
                    busses.Add((id, i));
            }
            _busses = busses.ToArray();
        }

        [Fact]
        public void Part1()
        {
            long minId = long.MaxValue;
            long minWait = long.MaxValue;

            foreach ((long id, _) in _busses)
            {
                long wait = id - _target % id;
                if (wait < minWait)
                {
                    minWait = wait;
                    minId = id;
                }
            }

            long answer = minId * minWait;
            Assert.Equal(5257, answer);
        }

        [Fact]
        public void Part2()
        {
            BigInteger M = 1;
            foreach ((long id, _) in _busses)
                M *= id;

            BigInteger answer = 0;
            foreach ((long id, int index) in _busses)
            {
                BigInteger m = id;
                BigInteger a = m - index;
                BigInteger b = M / m;
                BigInteger bPrime = BigInteger.ModPow(b, m - 2, m);

                answer += a * b * bPrime;
            }

            answer %= M;
            Assert.Equal(new BigInteger(538703333547789), answer);
        }

        [Fact]
        public void Part2_Different()
        {
            long inc = _busses[0].id;
            long answer = inc;

            LinkedList<(long id, int index)> available = new(_busses[1..]);

            while (available.Count > 0)
            {
                LinkedListNode<(long id, int index)> n = available.First;
                do
                {
                    if ((answer + n.Value.index) % n.Value.id == 0)
                        break;
                    n = n.Next;
                } while (n != null);

                if (n != null)
                {
                    inc *= n.Value.id;
                    available.Remove(n);
                }
                else
                {
                    answer += inc;
                }
            }
            Assert.Equal(538703333547789, answer);
        }
    }
}
