using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day02
    {
        private readonly string[] _input;

        public Day02()
        {
            _input = File.ReadAllLines("Inputs/Day02.txt");
        }

        [Fact]
        public void Part1()
        {
            string answer = GetCode(
                (1, 1),
                (pos, delta) => (pos + delta).Transform(i => Math.Clamp(i, 0, 2)),
                p => p switch
                {
                    (0, 0) => '1',
                    (1, 0) => '2',
                    (2, 0) => '3',
                    (0, 1) => '4',
                    (1, 1) => '5',
                    (2, 1) => '6',
                    (0, 2) => '7',
                    (1, 2) => '8',
                    (2, 2) => '9',
                    _ => throw new InvalidOperationException()
                });

            Assert.Equal("12578", answer);
        }

        [Fact]
        public void Part2()
        {
            string answer = GetCode(
                (0, 2),
                (pos, delta) =>
                {
                    IntPoint2 candidate = pos + delta;

                    if (candidate.X < 0 || candidate.X > 4 || candidate.Y < 0 || candidate.Y > 4)
                        return pos;

                    if ((candidate.X == 0 || candidate.X == 4) && candidate.Y != 2)
                        return pos;

                    if ((candidate.X == 1 || candidate.X == 3) && (candidate.Y == 0 || candidate.Y == 4))
                        return pos;

                    return candidate;
                },
                p => p switch
                {
                    (2, 0) => '1',
                    (1, 1) => '2',
                    (2, 1) => '3',
                    (3, 1) => '4',
                    (0, 2) => '5',
                    (1, 2) => '6',
                    (2, 2) => '7',
                    (3, 2) => '8',
                    (4, 2) => '9',
                    (1, 3) => 'A',
                    (2, 3) => 'B',
                    (3, 3) => 'C',
                    (2, 4) => 'D',
                    _ => throw new InvalidOperationException()
                });

            Assert.Equal("516DD", answer);
        }

        private string GetCode(IntPoint2 initialPosition, Func<IntPoint2, IntPoint2, IntPoint2> addDeltaAndBound, Func<IntPoint2, char> getPositionCode)
        {
            IntPoint2 position = initialPosition;
            StringBuilder sb = new StringBuilder();

            foreach (string input in _input)
            {
                foreach (char c in input)
                {
                    IntPoint2 delta = c switch
                    {
                        'U' => -IntPoint2.UnitY,
                        'D' => IntPoint2.UnitY,
                        'L' => -IntPoint2.UnitX,
                        'R' => IntPoint2.UnitX,
                        _ => throw new InvalidOperationException()
                    };

                    position = addDeltaAndBound(position, delta);
                }

                sb.Append(getPositionCode(position));
            }

            return sb.ToString();
        }
    }
}
