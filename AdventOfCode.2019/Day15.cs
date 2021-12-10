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

        Dictionary<IntVec2, Status> _map = new Dictionary<IntVec2, Status>();
        IntVec2 _oxygen = IntVec2.Zero;

        public Day15()
        {
            AsyncQueue<long> programInputs = new AsyncQueue<long>();
            AsyncQueue<long> programOutputs = new AsyncQueue<long>();
            IntCodeAsync intCode = new IntCodeAsync(_program, programInputs.Dequeue, programOutputs.Enqueue);
            Task vm = Task.Run(intCode.RunAsync);

            Task<long> sendCommand(Direction d)
            {
                programInputs.Enqueue((long)d + 1);
                return programOutputs.Dequeue();
            }

            async Task backtrackHelper(IntVec2 position)
            {
                foreach (Direction d in s_directions)
                {
                    IntVec2 candidatePosition = position + s_deltas[(int)d];
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

            Task.Run(() => backtrackHelper(IntVec2.Zero)).Wait();
        }

        [Fact]
        public void Part1()
        {
            PriorityQueue<(IntVec2 position, int distance)> toExplore = 
                new PriorityQueue<(IntVec2, int)>(Comparer<(IntVec2 position, int distance)>.Create((left, right) =>
                {
                    return (left.position.ManhattanDistanceFrom(_oxygen) + left.distance) - 
                           (right.position.ManhattanDistanceFrom(_oxygen) + right.distance);
                }));
            HashSet<IntVec2> visisted = new HashSet<IntVec2>(_map.Count);
            toExplore.Enqueue((IntVec2.Zero, 0));

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
                foreach (IntVec2 adjacent in current.position.Adjacent())
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
            HashSet<IntVec2> filled = new HashSet<IntVec2>();
            List<IntVec2> toFill = new List<IntVec2>() { _oxygen };

            int iterations = -1;
            do
            {
                iterations++;
                IntVec2[] currentNodes = toFill.ToArray();
                toFill.Clear();

                foreach (IntVec2 current in currentNodes)
                    filled.Add(current);

                foreach (IntVec2 current in currentNodes)
                {
                    foreach (IntVec2 adjacent in current.Adjacent())
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
        static IntVec2[] s_deltas = new[] { IntVec2.UnitY, -IntVec2.UnitY, -IntVec2.UnitX, IntVec2.UnitX };
    }
}
