namespace AdventOfCode._2015;

public class Day08
{
    [Fact]
    public void Part1()
    {
        int total = 0;
        foreach (string s in Parse())
        {
            total += s.Length;

            int i = 1;
            while (i < s.Length - 1)
            {
                char c = s[i++];
                total--;

                if (c == '\\')
                {
                    c = s[i++];

                    if (c == 'x')
                        i += 2;
                    else if (!(c == '\\' || c == '"'))
                        throw new InvalidOperationException();
                }
            }
        }

        Assert.Equal(1371, total);
    }

    [Fact]
    public void Part2()
    {
        int total = 0;
        foreach (string s in Parse())
        {
            total -= s.Length;

            total += 2; // new outer quotes

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                total++;

                if (c == '"' || c == '\\')
                    total++;
            }
        }

        Assert.Equal(2117, total);
    }

    private IEnumerable<string> Parse()
    {
        return File.ReadAllLines("Inputs/Day08.txt");
    }
}
