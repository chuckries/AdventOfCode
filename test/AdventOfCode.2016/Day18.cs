namespace AdventOfCode._2016;

public class Day18
{
    private char[] _current;
    private char[] _next;

    public Day18()
    {
        _current = File.ReadAllText("Inputs/Day18.txt").ToCharArray();
        _next = (char[])_current.Clone();
    }

    [Fact]
    public void Part1()
    {
        int answer = SumSafe(40);
        Assert.Equal(1987, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = SumSafe(400_000);
        Assert.Equal(19984714, answer);
    }

    private int SumSafe(int ticks)
    {
        int total = 0;
        for (int i = 0; i < ticks; i++)
        {
            for (int j = 0; j < _current.Length; j++)
                if (_current[j] == '.')
                    total += 1;
            Tick();
        }
        return total;
    }

    private void Tick()
    {
        for (int i = 0; i < _current.Length; i++)
        {
            bool left = i > 0 && _current[i - 1] == '^';
            bool center = _current[i] == '^';
            bool right = i < _current.Length - 1 && _current[i + 1] == '^';

            _next[i] = ((left && center && !right) ||
                      (center && right && !left) ||
                      (left && !center && !right) ||
                      (!left && !center && right)) ?
                      '^' : '.';
        }

        (_current, _next) = (_next, _current);
    }
}
