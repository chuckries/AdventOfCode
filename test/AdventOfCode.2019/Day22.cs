using System.Numerics;

namespace AdventOfCode._2019;

public class Day22
{
    class Deck
    {
        public readonly long Size;
        public long Offset { get; private set; }
        public long Increment { get; private set; }

        public Deck(long size)
            : this(0, 1, size)
        {
        }

        public Deck(long offset, long increment, long size)
        {
            Offset = offset;
            Increment = increment;
            Size = size;
        }

        public long Get(long n)
        {
            return Mod(Offset + n * Increment);
        }

        public void NewStack()
        {
            Increment = Mod(Increment * -1);
            Offset = Mod(Offset + Increment);
        }

        public void Cut(long n)
        {
            Offset = Mod(Offset + Increment * n);
        }

        public void Deal(long n)
        {
            Increment = Mod(new BigInteger(Increment) * (long)BigInteger.ModPow(n, Size - 2, Size));
        }

        private long Mod(BigInteger n)
        {
            return (long)((n % Size + Size) % Size);
        }
    }

    [Fact]
    public void Part1()
    {
        const long Target = 2019;
        Deck deck = new Deck(10_007);
        Parse(File.ReadAllText("Inputs/Day22.txt"), deck);

        int pos = 0;
        while (true)
        {
            if (deck.Get(pos) == Target)
                break;

            pos++;
        }

        Assert.Equal(4096, pos);
    }

    [Fact]
    public void Part2()
    {
        // I never figured this out, all credit goes to 
        // https://www.reddit.com/r/adventofcode/comments/ee0rqi/2019_day_22_solutions/fbnkaju/?utm_source=reddit&utm_medium=web2x&context=3
        // whose explanation was helpful but barely understood
        // and whose code I essentially copied

        const long TargetPosition = 2020;
        const long Iterations = 101_741_582_076_661;
        const long Size = 119_315_717_514_047;
        Deck deck = new Deck(Size);
        Parse(File.ReadAllText("Inputs/Day22.txt"), deck);

        long finalIncrement = (long)BigInteger.ModPow(deck.Increment, Iterations, Size);
        BigInteger finalOffset =
            new BigInteger(deck.Offset) *
            new BigInteger(1 - finalIncrement) *
            BigInteger.ModPow(((1 - deck.Increment) % Size + Size) % Size, Size - 2, Size);
        finalOffset = (finalOffset % Size + Size) % Size;

        deck = new Deck((long)finalOffset, finalIncrement, Size);

        long answer = deck.Get(TargetPosition);

        Assert.Equal(78613970589919, answer);
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
