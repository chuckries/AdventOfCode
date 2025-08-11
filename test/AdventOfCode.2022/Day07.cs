using System.Net.Security;
using System.Text;

namespace AdventOfCode._2022;

public class Day07
{
    private abstract class Node
    {
        public required Dir? Parent { get; init; }

        public required string Name { get; init; }
    }

    private class Dir : Node
    {
        public Dictionary<string, Node> Children { get; } = new();
    }

    private class File : Node
    {
        public required long Size { get; init; }
    }

    Dir _root;

    public Day07()
    {
        _root = new Dir()
        {
            Name = "/",
            Parent = null
        };

        List<string[]> input = System.IO.File.ReadAllLines("Inputs/Day07.txt").Select(s => s.Split(' ')).ToList();

        int i = 0;
        Dir current = _root;
        while (i < input.Count)
        {
            string[] line = input[i];

            if (line[0] == "$")
            {
                if (line[1] == "cd")
                {
                    var target = line[2];

                    current = target switch
                    {
                        ".." => current.Parent ?? throw new InvalidOperationException(),
                        "/" => _root,
                        _ => (Dir)current.Children[target]
                    };

                    i++;
                }
                else if (line[1] == "ls")
                {
                    while (true)
                    {
                        i++;
                        if(i >= input.Count)
                            break;
                        line = input[i];
                        if (line[0] == "$")
                            break;

                        string name = line[1];

                        if (line[0] == "dir")
                        {
                            current.Children.Add(name, new Dir()
                            {
                                Name = name,
                                Parent = current
                            });
                        }
                        else
                        {
                            long size = long.Parse(line[0]);
                            current.Children.Add(name, new File()
                            {
                                Name = name,
                                Size = size,
                                Parent = current,
                            });
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }

    long SumVisit(Node n, Action<long> visitor)
    {
        switch (n)
        {
            case File file:
                return file.Size;
            case Dir dir:
                long sum = dir.Children.Values.Sum(n => SumVisit(n, visitor));
                visitor(sum);
                return sum;
            default:
                throw new InvalidOperationException();
        }
    }

    [Fact]
    public void Part1()
    {
        long answer = 0;
        SumVisit(_root, total =>
        {
            if (total <= 100000)
                answer += total;
        });

        Assert.Equal(1723892, answer);
    }

    [Fact]
    public void Part2()
    {
        long total = SumVisit(_root, _ => { });
        long min_required = 30000000 - (70000000 - total);

        long min_dir = long.MaxValue;
        SumVisit(_root, size =>
        {
            if (size >= min_required && size < min_dir)
            {
                min_dir = size;
            }
        });

        Assert.Equal(8474158, min_dir);
    }
}
