using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2020
{
    public class Day23
    {
        const string Input = "137826495";

        private class Game2
        {
            private int[] _list;
            private int _current;
            private int _max;

            public Game2(int[] seed, int size)
            {
                _list = new int[size + 1];
                _max = size;

                _current = seed[0];
                int current = _current;

                int i = 1;
                for (; i < seed.Length; i++)
                    current = _list[current] = seed[i];

                i++;
                for (; i <= size; i++)
                    current = _list[current] = i;

                _list[current] = _current;
            }

            public unsafe void Tick(int count)
            {
                int a, b, c, destination;
                fixed (int* list = _list)
                {
                    for (int i = 0; i < count; i++)
                    {
                        a = list[_current];
                        b = list[a];
                        c = list[b];

                        list[_current] = list[c];

                        destination = _current;
                        do
                            if (--destination == 0)
                                destination = _max;
                        while (destination == a || destination == b || destination == c);

                        list[c] = list[destination];
                        list[destination] = a;

                        _current = list[_current];
                    }
                }
            }

            public IEnumerable<int> AnswerOrder()
            {
                int n = _list[1];
                do
                {
                    yield return n;
                    n = _list[n];
                } while (n != 1);
            }
        }

        int[] _input;

        public Day23()
        {
            _input = Input.Select(c => c - '0').ToArray();
        }

        [Fact]
        public void Part1()
        {
            Game2 game = new(_input, _input.Length);
            game.Tick(100);

            string answer = new string(game.AnswerOrder().Select(i => (char)(i + '0')).ToArray());
            Assert.Equal("59374826", answer);
        }

        [Fact]
        public void Part2()
        {
            Game2 game = new(_input, 1_000_000);
            game.Tick(10_000_000);

            long answer = game.AnswerOrder().Take(2).Aggregate(1L, (x, y) => x * y);
            Assert.Equal(66878091588, answer);
        }
    }
}
