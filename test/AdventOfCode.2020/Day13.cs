using System.Numerics;

namespace AdventOfCode._2020;

public class Day13
{
    private long _target;
    private (long id, int index)[] _busses;

    public Day13()
    {
        string[] input = File.ReadAllLines("Inputs/Day13.txt");
        _target = long.Parse(input[0]);
        string[] strings = input[1].Split(',');
        List<(long, int)> busses = new(strings.Length);
        for (int i = 0; i < strings.Length; i++)
        {
            if (long.TryParse(strings[i], out long id))
                busses.Add((id, i));
        }
        _busses = busses.ToArray();
    }

    [Fact]
    public void Part1()
    {
        long minId = long.MaxValue;
        long minWait = long.MaxValue;

        foreach ((long id, _) in _busses)
        {
            long wait = id - _target % id;
            if (wait < minWait)
            {
                minWait = wait;
                minId = id;
            }
        }

        long answer = minId * minWait;
        Assert.Equal(5257, answer);
    }

    [Fact(Skip = "slower")]
    public void Part2_ChineseRemainderTheorem()
    {
        BigInteger M = 1;
        foreach ((long id, _) in _busses)
            M *= id;

        BigInteger answer = 0;
        foreach ((long id, int index) in _busses)
        {
            BigInteger m = id;
            BigInteger a = m - index;
            BigInteger b = M / m;
            BigInteger bPrime = BigInteger.ModPow(b, m - 2, m);

            answer += a * b * bPrime;
        }

        answer %= M;

        Assert.Equal(new BigInteger(538703333547789), answer);
    }

    [Fact]
    public void Part2_Iterative()
    {
        long inc = _busses[0].id;
        long answer = inc;

        for (int i = 1; i < _busses.Length; i++)
        {
            while ((answer + _busses[i].index) % _busses[i].id != 0)
                answer += inc;
            inc *= _busses[i].id;
        }

        Assert.Equal(538703333547789, answer);
    }
}
