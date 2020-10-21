using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day17
    {
        long[] _program = File.ReadAllText("Inputs/Day17.txt")
            .Split(',')
            .Select(long.Parse)
            .ToArray();

        string[] _map;

        public Day17()
        {
            StringBuilder sb = new StringBuilder();
            IntCode intCode = new IntCode(
                _program,
                null,
                value =>
                {
                    if (value == 10)
                        sb.AppendLine();
                    else
                        sb.Append((char)value);
                });

            intCode.Run();

            _map = sb.ToString().Split().Where(s => s != string.Empty).ToArray();
        }

        [Fact]
        public void Part1()
        {
            int total = 0;
            for (int j = 0; j < _map.Length; j++)
            {
                for (int i = 0; i < _map[j].Length; i++)
                {
                    if (_map[j][i] == '#')
                    {
                        if (j - 1 >= 0 && _map[j - 1][i] == '#' &&
                            j + 1 < _map.Length && _map[j + 1][i] == '#' &&
                            i - 1 >= 0 && _map[j][i - 1] == '#' &&
                            i + 1 < _map[j].Length && _map[j][i + 1] == '#')
                            total += i * j;
                    }
                }
            }

            Assert.Equal(3888, total);
        }

        [Fact]
        public void Part2()
        {
            string str = GetString();

            // do magic

            string[] functions = new string[]
            {
                "A,B,A,C,B,C,B,C,A,C",
                "L,10,R,12,R,12",
                "R,6,R,10,L,10",
                "R,10,L,10,L,12,R,6"
            };

            string functionString = string.Join('\n', functions) + '\n' + 'n' + '\n';
            int current = 0;
            long answer = 0;
            IntCode intCode = new IntCode(
                _program,
                () => functionString[current++],
                val => answer = val
                );
            intCode[0] = 2;
            intCode.Run();

            Assert.Equal(927809, answer);
        }

        private string GetString()
        {
            List<string> parts = new List<string>();

            IntPoint2 current = GetStart();
            IntPoint2 dir = -IntPoint2.UnitY;

            while (true)
            {
                char? letter = GetTurn(current, ref dir);
                if (letter == null)
                    break;

                parts.Add(new string(new[] { letter.Value }));

                parts.Add(MoveForward(ref current, dir).ToString());
            }

            return string.Join(',', parts);
        }

        private IntPoint2 GetStart()
        {
            for (int j = 0; j < _map.Length; j++)
            {
                for (int i = 0; i < _map[0].Length; i++)
                {
                    if (_map[j][i] == '^')
                        return (i, j);
                }
            }

            throw new InvalidOperationException();
        }

        private int MoveForward(ref IntPoint2 current, IntPoint2 direction)
        {
            int count = 0;

            while (InBounds(current + direction) &&
                _map[current.Y + direction.Y][current.X + direction.X] == '#')
            {
                current += direction;
                count++;
            }

            return count;
        }

        private char? GetTurn(IntPoint2 current, ref IntPoint2 direction)
        {
            IntPoint2 leftPoint = current + -direction.TurnLeft();
            IntPoint2 rightPoint = current + -direction.TurnRight();

            if (InBounds(leftPoint) && _map[leftPoint.Y][leftPoint.X] == '#')
            {
                direction = -direction.TurnLeft();
                return 'L';
            }
            else if (InBounds(rightPoint) && _map[rightPoint.Y][rightPoint.X] == '#')
            {
                direction = -direction.TurnRight();
                return 'R';
            }

            return null;
        }

        private bool InBounds(IntPoint2 point)
        {
            return point.X >= 0 && point.X < _map[0].Length &&
                   point.Y >= 0 && point.Y < _map.Length;
        }
    }
}
