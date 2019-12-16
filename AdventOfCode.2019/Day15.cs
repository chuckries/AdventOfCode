using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day15
    {
        enum Direction : long
        {
            N = 0,
            S = 1,
            W = 2,
            E = 3
        }

        enum Status : long
        {
            Wall = 0,
            Success = 1,
            Oxygen = 2
        }

        long[] _program = File.ReadAllText("Inputs/Day15.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        Dictionary<IntPoint2, Status> _map = new Dictionary<IntPoint2, Status>();
        IntPoint2 _oxygen = IntPoint2.Zero;

        public Day15()
        {
            AsyncQueue<long> programInputs = new AsyncQueue<long>();
            AsyncQueue<long> programOutputs = new AsyncQueue<long>();
            IntCode intCode = new IntCode(_program, programInputs.Dequeue, programOutputs.Enqueue);
            Task vm = Task.Run(intCode.Run);

            Task<long> sendCommand(Direction d)
            {
                programInputs.Enqueue((long)d + 1);
                return programOutputs.Dequeue();
            }

            async Task backtrackHelper(IntPoint2 position)
            {
                foreach (Direction d in s_directions)
                {
                    IntPoint2 candidatePosition = position + s_deltas[(int)d];
                    if (!_map.ContainsKey(candidatePosition))
                    {
                        Status status = (Status)await sendCommand(d);
                        if (status != Status.Wall)
                        {
                            _map.Add(candidatePosition, status);
                            if (status == Status.Oxygen)
                                _oxygen = candidatePosition;

                            await backtrackHelper(candidatePosition);
                            await sendCommand(s_opposites[(int)d]);
                        }
                    }
                }
            }

            Task.Run(() => backtrackHelper(IntPoint2.Zero)).Wait();
        }

        [Fact]
        public void Part1()
        {
            PriorityQueue<(IntPoint2 position, int distance)> toExplore = 
                new PriorityQueue<(IntPoint2, int)>(Comparer<(IntPoint2 position, int distance)>.Create((left, right) =>
                {
                    return (left.position.Distance(_oxygen) + left.distance) - 
                           (right.position.Distance(_oxygen) + right.distance);
                }));
            HashSet<IntPoint2> visisted = new HashSet<IntPoint2>(_map.Count);
            toExplore.Enqueue((IntPoint2.Zero, 0));

            int answer = 0;
            do
            {
                var current = toExplore.Dequeue();

                if (current.position.Equals(_oxygen))
                {
                    answer = current.distance;
                    break;
                }

                int newDistance = current.distance + 1;
                foreach (IntPoint2 adjacent in current.position.Adjacent())
                {
                    if (_map.TryGetValue(adjacent, out Status status) && status != Status.Wall && !visisted.Contains(adjacent))
                    {
                        toExplore.Enqueue((adjacent, newDistance));
                    }
                }

                visisted.Add(current.position);
            } while (toExplore.Count > 0);

            Assert.Equal(380, answer);
        }

        [Fact]
        public void Part2()
        {
            HashSet<IntPoint2> filled = new HashSet<IntPoint2>();
            List<IntPoint2> toFill = new List<IntPoint2>() { _oxygen };

            int iterations = -1;
            do
            {
                iterations++;
                IntPoint2[] currentNodes = toFill.ToArray();
                toFill.Clear();

                foreach (IntPoint2 current in currentNodes)
                    filled.Add(current);

                foreach (IntPoint2 current in currentNodes)
                {
                    foreach (IntPoint2 adjacent in current.Adjacent())
                    {
                        if (_map.TryGetValue(adjacent, out Status status) && status != Status.Wall && !filled.Contains(adjacent))
                        {
                            toFill.Add(adjacent);
                        }
                    }
                }
            } while (toFill.Count > 0);

            Assert.Equal(410, iterations);
        }

        static Direction[] s_directions = new[] { Direction.N, Direction.S, Direction.W, Direction.E };
        static Direction[] s_opposites = new[] { Direction.S, Direction.N, Direction.E, Direction.W };
        static IntPoint2[] s_deltas = new[] { IntPoint2.UnitY, -IntPoint2.UnitY, -IntPoint2.UnitX, IntPoint2.UnitX };
    }
}
