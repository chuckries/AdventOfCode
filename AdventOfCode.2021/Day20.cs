using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    public class Day20
    {
        bool _doToggle;
        bool _toggle;
        bool[] _lookup;
        Dictionary<IntVec2, bool> _current;
        Dictionary<IntVec2, bool> _next;
        (IntVec2 low, IntVec2 hi) _nextBounds;

        public Day20()
        {
            string[] lines = File.ReadAllLines("Inputs/Day20.txt");
            _lookup = lines[0].Select(c => c is '#').ToArray();
            if (_lookup[0] && !_lookup[^1])
                _doToggle = true;
            else if (!_lookup[0])
                throw new InvalidOperationException();

            _current = new();
            _next = new();
            for (int j = 2; j < lines.Length; j++)
                for (int i = 0; i < lines[2].Length; i++)
                    if (lines[j][i] is '#')
                        _current[new(i, j - 2)] = true;

            _nextBounds = (new(-1, -1), new(lines[2].Length, lines.Length - 2));
            _toggle = false;
        }

        [Fact]
        public void Part1()
        {
            Tick();
            Tick();

            int answer = _current.Count;
            Assert.Equal(5663, answer);
        }

        [Fact]
        public void Part2()
        {
            for (int i = 0; i < 50; i++)
                Tick();

            int answer = _current.Count;
            Assert.Equal(19638, answer);
        }

        private void Tick()
        {
            for (int j = _nextBounds.low.Y; j <= _nextBounds.hi.Y; j++)
            { 
                for (int i = _nextBounds.low.X; i <= _nextBounds.hi.X; i++)
                {
                    int val = 0;
                    for (int v = j - 1; v <= j + 1; v++)
                    {
                        for (int u = i - 1; u <= i + 1; u++)
                        {
                            if (Get(u, v))
                                val |= 1;
                            val <<= 1;
                        }
                    }
                    val >>= 1;
                    if (_lookup[val])
                        _next[new(i, j)] = true;
                }
            }

            (_current, _next) = (_next, _current);
            _next.Clear();

            _nextBounds = (_nextBounds.low - 1, _nextBounds.hi + 1);
            _toggle = !_toggle;
        }

        private bool Get(int x, int y)
        {
            if (_doToggle && (x <= _nextBounds.low.X || x >= _nextBounds.hi.X || y <= _nextBounds.low.Y || y >= _nextBounds.hi.Y))
                return _toggle;

            return _current.TryGetValue(new(x, y), out bool val) && val;
        }
    }
}
