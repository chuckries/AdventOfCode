using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2017
{
    public class Day15
    {
        private class Generator
        {
            private const uint Divider = 2147483647;

            readonly uint _factor;
            uint _current;

            public uint Current => _current;
            public ushort Checksum => (ushort)_current;

            public Generator(uint factor, uint seed)
            {
                _factor = factor;
                _current = seed;
            }

            public void Tick()
            {
                ulong tmp = (ulong)_current * _factor;
                _current = (uint)(tmp % Divider);
            }

            public void TickForMultiples(int multipleOf)
            {
                do
                {
                    Tick();
                } while (_current % multipleOf != 0);
            }

            public static IEnumerable<ushort> Generate(uint factor, uint seed)
            {
                uint current = seed;
                while (true)
                {
                    current = (uint)(((ulong)current * factor) % Divider);
                    yield return (ushort)current;
                }
            }
        }

        private Generator _genA = new Generator(16807, 722);
        private Generator _genB = new Generator(48271, 354);

        [Fact]
        public void Part1()
        {
            int count = Generator.Generate(16807, 722)
                .Zip(Generator.Generate(48271, 354))
                .Take(40_000_000)
                .Count(pair => pair.First == pair.Second);

            Assert.Equal(612, count);
        }

        [Fact]
        public void Part2()
        {
            int count = 0;
            for (int i = 0; i < 5_000_000; i++)
            {
                _genA.TickForMultiples(4);
                _genB.TickForMultiples(8);

                if (_genA.Checksum == _genB.Checksum)
                    count++;
            }

            Assert.Equal(285, count);
        }
    }
}
