namespace AdventOfCode._2020;

public class Day09
{
    long[] _input;

    public Day09()
    {
        _input = File.ReadAllLines("Inputs/Day09.txt").Select(long.Parse).ToArray();
    }

    [Fact]
    public void Part1()
    {
        const int window = 25;
        HashSet<long> pool = new HashSet<long>(_input.Take(window));

        long answer = 0;

        int i = window;
        while (i < _input.Length)
        {
            bool found = false;

            long current = _input[i];
            foreach (long candidate in pool)
            {
                if (pool.TryGetValue(current - candidate, out long other) && other != candidate)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                answer = current;
                break;
            }

            pool.Remove(_input[i - window]);
            pool.Add(_input[++i]);
        }

        Assert.Equal(530627549, answer);
    }

    [Fact]
    public void Part2()
    {
        const long target = 530627549;

        int start = 0;
        int end = 1;
        long sum = _input[start] + _input[end];

        while (sum != target)
        {
            if (sum < target)
                sum += _input[++end];
            else
                sum -= _input[start++];
        }

        long min = long.MaxValue;
        long max = 0;

        for (int i = start; i <= end; i++)
        {
            long val = _input[i];
            if (val < min)
                min = val;
            if (val > max)
                max = val; ;
        }

        long answer = min + max;
        Assert.Equal(77730285, answer);
    }
}
