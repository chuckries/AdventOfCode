using System.Text.RegularExpressions;

namespace AdventOfCode._2015;

public class Day12
{
    string _input;

    public Day12()
    {
        _input = File.ReadAllText("Inputs/Day12.txt");
    }

    [Fact]
    public void Part1()
    {
        Regex regex = new Regex(@"-?\d+", RegexOptions.Compiled);
        MatchCollection matches = regex.Matches(_input);

        int total = matches.Select(m => int.Parse(m.Value)).Sum();

        Assert.Equal(111754, total);
    }

    [Fact]
    public void Part2()
    {
        int index = 0;
        int currentLevel = 0;
        int currentTotal = 0;
        Stack<int> objectTotals = new Stack<int>();

        while (index < _input.Length)
        {
            char c = _input[index];

            if (c == '{')
            {
                objectTotals.Push(currentTotal);
                currentTotal = 0;
                currentLevel++;
            }
            else if (c == '}')
            {
                currentTotal += objectTotals.Pop();
                currentLevel--;
            }
            else if (char.IsNumber(c) || c == '-')
            {
                int numStart = index;
                while (char.IsNumber(_input[index + 1]))
                    index++;
                currentTotal += int.Parse(_input.AsSpan(numStart, index - numStart + 1));
            }
            else if (c == ':' && _input[index + 1] == '"')
            {
                index += 2;
                int endIndex = _input.IndexOf('"', index);
                if (endIndex - index == 3 &&
                    string.Compare(_input, index, "red", 0, endIndex - index) == 0)
                {
                    currentTotal = objectTotals.Pop();
                    int targetLevel = currentLevel - 1;
                    while (currentLevel != targetLevel)
                    {
                        index = _input.IndexOfAny(s_Braces, index + 1);
                        c = _input[index];
                        if (c == '{') currentLevel++;
                        else if (c == '}') currentLevel--;
                    }
                }
                else
                {
                    index = endIndex;
                }
            }

            index++;
        }

        Assert.Equal(65402, currentTotal);
    }

    static char[] s_Braces = new[] { '{', '}' };
}
