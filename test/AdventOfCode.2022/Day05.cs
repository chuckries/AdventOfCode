namespace AdventOfCode._2022;

public class Day05
{
    List<Stack<char>> _stacks = new();
    List<(int count, int from, int to)> _moves = new();

    public Day05()
    {
        var lines = File.ReadAllLines("Inputs/Day05.txt");
        int emptyLine = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == string.Empty)
            {
                emptyLine = i;
                break;
            }
        }

        string idxLine = lines[emptyLine - 1];
        for (int i = 0; i < idxLine.Length; i++)
        {
            if (idxLine[i] != ' ')
            {
                Stack<char> stack = new();
                int y = emptyLine - 1;
                while (y >= 0)
                {
                    if (lines[y][i] != ' ')
                    {
                        stack.Push(lines[y][i]); 
                    }
                    else
                    {
                        break;
                    }
                    y--;
                }
                _stacks.Add(stack);
            }
        }

        for (int i = emptyLine + 1; i < lines.Length; i++)
        {
            var split = lines[i].Split(' ');
            int count = int.Parse(split[1]);
            int from = int.Parse(split[3]);
            int to = int.Parse(split[5]);
            _moves.Add((count, from - 1, to - 1));
        }
    }

    [Fact]
    public void Part1()
    {
        foreach ((int count, int from, int to) in _moves)
        {
            for (int i = 0; i < count; i++)
            {
                _stacks[to].Push(_stacks[from].Pop());
            }
        }

        string answer = new(_stacks.Select(s => s.Peek()).ToArray());
        Assert.Equal("VQZNJMWTR", answer);
    }

    [Fact]
    public void Part2()
    {
        Stack<char> temp = new();
        foreach ((int count, int from, int to) in _moves)
        {
            for (int i = 0; i < count; i++)
            {
                temp.Push(_stacks[from].Pop());
            }

            for (int i = 0; i < count; i++)
            {
                _stacks[to].Push(temp.Pop());
            }
        }

        string answer = new(_stacks.Select(s => s.Peek()).ToArray());
        Assert.Equal("NLCDCLVMQ", answer);
    }
}
