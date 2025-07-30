namespace AdventOfCode._2015;

public class Day05
{
    [Fact]
    public void Part1()
    {
        int answer = Parse()
            .Where(s =>
            {
                int vowels = 0;
                int doubles = 0;
                bool valid = true;

                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];

                    if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u')
                        vowels++;

                    if (i < s.Length - 1)
                    {
                        char cNext = s[i + 1];

                        if (c == cNext)
                            doubles++;

                        if ((c == 'a' && cNext == 'b') ||
                            (c == 'c' && cNext == 'd') ||
                            (c == 'p' && cNext == 'q') ||
                            (c == 'x' && cNext == 'y'))
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                return valid && vowels >= 3 && doubles > 0;
            })
            .Count();

        Assert.Equal(255, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = Parse()
            .Where(s =>
            {
                bool hasDouble = false;
                bool hasRepeat = false;
                for (int i = 0; i < s.Length - 2; i++)
                {
                    char c1 = s[i];
                    if (!hasDouble && s[i + 2] == c1)
                        hasDouble = true;

                    if (!hasRepeat)
                    {
                        char c2 = s[i + 1];
                        for (int j = i + 2; j < s.Length - 1; j++)
                        {
                            char c3 = s[j];
                            char c4 = s[j + 1];

                            if (c1 == c3 && c2 == c4)
                                hasRepeat = true;
                        }
                    }
                }

                return hasDouble && hasRepeat;
            })
            .Count();

        Assert.Equal(55, answer);
    }

    private IEnumerable<string> Parse()
    {
        return File.ReadAllLines("Inputs/Day05.txt");
    }
}
