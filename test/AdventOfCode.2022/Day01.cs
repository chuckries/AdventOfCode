namespace AdventOfCode._2022;

public class Day01
{

    List<List<int>> input;

    public Day01()
    {
        this.input = new();
        List<int> current = new();
        foreach (string line in File.ReadAllLines("Inputs/Day01.txt"))
        {
            if (line == string.Empty)
            {
                this.input.Add(current);
                current = new();
            }
            else
            {
                current.Add(int.Parse(line));
            }
        }
    }

    [Fact]
    public void Part1()
    {
        int answer = this.input.Select(g => g.Sum()).Max();
        Assert.Equal(66616, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = this.input.Select(g => g.Sum()).OrderByDescending(i => i).Take(3).Sum();
        Assert.Equal(199172, answer);
    }
}