using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day22
    {
        class Deck
        {
            public long Current => _deck[_head];

            public Deck(long size)
            {
                _size = size;
                _deck = new long[_size];
                for (long i = 0; i < _size; i++)
                    _deck[i] = i;
                _temp = new long[_size];
                _head = 0;
                _dir = 1;
            }

            public IEnumerable<long> Cards()
            {
                long current = _head;
                for (long i = 0; i < _size; i++)
                {
                    yield return _deck[current];
                    current = Bounds(current + _dir);
                }
            }

            public void NewStack()
            {
                _dir *= -1;
                _head = Bounds(_head + _dir);
            }

            public void Cut(int amount)
            {
                _head = Bounds(_head + amount * _dir);
            }

            public void Deal(long number)
            {
                long index = 0;
                foreach (long card in Cards())
                {
                    _temp[index] = card;
                    index = Bounds(index + number);
                }
                var temp = _deck;
                _deck = _temp;
                _temp = temp;
                _head = 0;
                _dir = 1;
            }

            private long Bounds(long current)
            {
                if (current >= _size)
                    return current %= _size;
                else
                {
                    while (current < 0)
                        current += _size;
                    return current;
                }
            }

            long _size;
            long[] _deck;
            long[] _temp;
            long _head;
            long _dir;
        }

        [Fact]
        public void Part1()
        {
            Deck deck = new Deck(10007);
            Parse(File.ReadAllText("Inputs/Day22.txt"), deck);

            long i = 0;
            foreach (long card in deck.Cards())
            {
                if (card == 2019)
                    break;
                i++;
            }

            Assert.Equal(4096, i);
        }

        [Fact]
        public void DeckTest()
        {
            Deck deck = new Deck(10);

            deck.Cut(3);
            Assert.Equal(3, deck.Current);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 3, 4, 5, 6, 7, 8, 9, 0, 1, 2 }));

            deck.Cut(-4);
            Assert.Equal(9, deck.Current);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 9, 0, 1, 2, 3, 4, 5, 6, 7, 8 }));

            deck.NewStack();
            Assert.Equal(8, deck.Current);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 8, 7, 6, 5, 4, 3, 2, 1, 0, 9 }));

            deck.Cut(1);
            Assert.Equal(7, deck.Current);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 7, 6, 5, 4, 3, 2, 1, 0, 9, 8 }));

            deck.NewStack();
            Assert.Equal(8, deck.Current);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 8, 9, 0, 1, 2, 3, 4, 5, 6, 7 }));
        }

        [Fact]
        public void Example()
        {
            string input = @"deal with increment 7
deal into new stack
deal into new stack";
            Deck deck = new Deck(10);
            Parse(input, deck);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 0, 3, 6, 9, 2, 5, 8, 1, 4, 7 }));

            input = @"cut 6
deal with increment 7
deal into new stack";
            deck = new Deck(10);
            Parse(input, deck);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 3, 0, 7, 4, 1, 8, 5, 2, 9, 6 }));

            input = @"deal with increment 7
deal with increment 9
cut -2";
            deck = new Deck(10);
            Parse(input, deck);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 6, 3, 0, 7, 4, 1, 8, 5, 2, 9 }));

            input = @"deal into new stack
cut -2
deal with increment 7
cut 8
cut -4
deal with increment 7
cut 3
deal with increment 9
deal with increment 3
cut -1";
            deck = new Deck(10);
            Parse(input, deck);
            Assert.True(Enumerable.SequenceEqual(deck.Cards(), new long[] { 9, 2, 5, 8, 1, 4, 7, 0, 3, 6 }));
        }

        private void Parse(string shuffle, Deck deck)
        {
            foreach (string line in shuffle.Split(Environment.NewLine))
            {
                if (line.StartsWith("deal"))
                {
                    if (line.EndsWith("stack"))
                        deck.NewStack();
                    else
                    {
                        int number = int.Parse(line.Split()[^1]);
                        deck.Deal(number);
                    }
                }
                else if (line.StartsWith("cut"))
                {
                    int number = int.Parse(line.Split()[^1]);
                    deck.Cut(number);
                }
                else
                    throw new InvalidOperationException();
            }
        }
    }
}
