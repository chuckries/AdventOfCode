namespace AdventOfCode._2017;

public class Day15
{
    private class Generator
    {
        private const uint Divider = 2147483647;

        readonly uint _factor;
        uint _current;
        int _multiplesOfMask;

        public uint Current => _current;
        public ushort Checksum => (ushort)_current;

        public Generator(uint factor, uint seed, int multiplesOf)
        {
            _factor = factor;
            _current = seed;
            // hack, works for powers of 2
            _multiplesOfMask = multiplesOf - 1;
        }

        public void Tick()
        {
            ulong tmp = (ulong)_current * _factor;
            _current = (uint)(tmp % Divider);
        }

        public void TickForMultiples()
        {
            do
            {
                Tick();
            } while ((_current & _multiplesOfMask) != 0);
        }
    }

    private Generator _genA = new Generator(16807, 722, 4);
    private Generator _genB = new Generator(48271, 354, 8);

    [Fact]
    public void Part1()
    {
        int count = 0;
        for (int i = 0; i < 40_000_000; i++)
        {
            _genA.Tick();
            _genB.Tick();

            if (_genA.Checksum == _genB.Checksum)
                count++;
        }

        Assert.Equal(612, count);
    }

    [Fact]
    public void Part2()
    {
        int count = 0;
        for (int i = 0; i < 5_000_000; i++)
        {
            _genA.TickForMultiples();
            _genB.TickForMultiples();

            if (_genA.Checksum == _genB.Checksum)
                count++;
        }

        Assert.Equal(285, count);
    }
}
