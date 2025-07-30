namespace AdventOfCode._2016;

public class Day07
{
    string[] _input;

    public Day07()
    {
        _input = File.ReadAllLines("Inputs/Day07.txt");
    }

    [Fact]
    public void Part1()
    {
        int answer = _input.Where(s =>
        {
            bool inBrackets = false;
            bool hasQuad = false;
            for (int i = 0; i < s.Length - 3; i++)
            {
                char c = s[i];

                if (c == '[')
                    inBrackets = true;
                else if (c == ']')
                    inBrackets = false;
                else if (!hasQuad || inBrackets)
                {
                    char c1 = s[i + 1];
                    char c2 = s[i + 2];
                    char c3 = s[i + 3];

                    if (c != c1 && c1 == c2 && c == c3)
                    {
                        if (inBrackets)
                            return false;

                        hasQuad = true;
                    }
                }
            }

            return hasQuad;
        }).Count();

        Assert.Equal(105, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = _input.Where(s =>
        {
            HashSet<string> tris = new HashSet<string>();
            bool inBrackets = false;
            for (int i = 0; i < s.Length - 2; i++)
            {
                char c = s[i];

                if (c == '[')
                    inBrackets = true;
                else if (c == ']')
                    inBrackets = false;
                else if (!inBrackets &&
                    c == s[i + 2] && c != s[i + 1] && s[i + 1] != '[')
                {
                    tris.Add(new string(new[] { s[i + 1], c, s[i + 1] }));
                }
            }

            inBrackets = false;
            for (int i = 0; i < s.Length - 2; i++)
            {
                char c = s[i];

                if (c == '[')
                    inBrackets = true;
                else if (c == ']')
                    inBrackets = false;
                else if (inBrackets)
                {
                    foreach (string tri in tris)
                        if (string.Compare(s, i, tri, 0, 3) == 0)
                            return true;
                }
            }

            return false;
        }).Count();

        Assert.Equal(258, answer);
    }
}
