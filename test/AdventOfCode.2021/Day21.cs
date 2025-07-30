using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day21
    {
        const int StartA = 8;
        const int StartB = 5;

        private class Dice
        {
            private IEnumerator<int> _enumerator;

            public Dice()
            {
                _enumerator = GetEnumerator();
            }

            public int Roll()
            {
                _enumerator.MoveNext();
                return _enumerator.Current;
            }

            private IEnumerator<int> GetEnumerator()
            {
                while (true)
                    for (int i = 1; i <= 100; i++)
                        yield return i;
            }
        }

        private class Player
        {
            private int _position;

            public int Position => _position + 1;
            public int Score { get; private set; }

            public Player(int initialPosition)
            {
                _position = initialPosition == 0 ? 9 : initialPosition - 1;
            }

            public void DoTurn(Dice dice)
            {
                _position += dice.Roll() + dice.Roll() + dice.Roll();
                _position %= 10;

                Score += Position;
            }
        }

        Dice _dice;
        Player _a;
        Player _b;

        public Day21()
        {
            _dice = new Dice();
            _a = new Player(StartA);
            _b = new Player(StartB);
        }

        [Fact]
        public void Part1()
        {
            int answer = Play();
            Assert.Equal(597600, answer);
        }

        [Fact]
        public void Part2()
        {
            var positions = new long[21, 21, 10, 10, 2];
            positions[0, 0, StartA - 1, StartB - 1, 0] = 1;

            long aWins = 0;
            long bWins = 0;
            for (int scoreA = 0; scoreA < 21; scoreA++)
            {
                for (int scoreB = 0; scoreB < 21; scoreB++)
                {
                    for (int posA = 0; posA < 10; posA++)
                    {
                        for (int posB = 0; posB < 10; posB++)
                        {
                            for (int turn = 0; turn < 2; turn++)
                            {
                                for (int i = 1; i <= 3; i++)
                                {
                                    for (int j = 1; j <= 3; j++)
                                    {
                                        for (int k = 1; k <= 3; k++)
                                        {
                                            int dice = i + j + k;

                                            int pos;
                                            int score;
                                            if (turn == 0)
                                            {
                                                pos = posA;
                                                score = scoreA;
                                            }
                                            else
                                            {
                                                pos = posB;
                                                score = scoreB;
                                            }

                                            int newPos = pos + dice;
                                            newPos %= 10;
                                            int newScore = score + newPos + 1;

                                            if (newScore >= 21)
                                            {
                                                if (turn is 0)
                                                    aWins += positions[scoreA, scoreB, posA, posB, 0];
                                                else
                                                    bWins += positions[scoreA, scoreB, posA, posB, 1];
                                                continue;
                                            }

                                            if (turn is 0)
                                            {
                                                positions[newScore, scoreB, newPos, posB, 1] +=
                                                    positions[scoreA, scoreB, posA, posB, 0];
                                            }
                                            else
                                            {
                                                positions[scoreA, newScore, posA, newPos, 0] +=
                                                    positions[scoreA, scoreB, posA, posB, 1];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            long answer = aWins > bWins ? aWins : bWins;
            Assert.Equal(634769613696613, answer);
        }

        private int Play()
        {
            Player current = _a;
            Player next = _b;
            int turns = 0;

            while (true)
            {
                current.DoTurn(_dice);
                turns++;
                if (current.Score >= 1000)
                {
                    return turns * 3 * next.Score;
                }

                (current, next) = (next, current);
            }
        }
    }
}
