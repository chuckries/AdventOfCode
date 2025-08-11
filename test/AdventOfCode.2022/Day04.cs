namespace AdventOfCode._2022;

public class Day04
{
    List<((int x, int y) a, (int x, int y) b)> input;

    public Day04()
    {
        input = File.ReadAllLines("Inputs/Day04.txt")
            .Select(line =>
            {
                var nums = line.Split([',', '-']).Select(int.Parse).ToArray();
                return ((nums[0], nums[1]), (nums[2], nums[3]));
            }).ToList();
    }

    [Fact]
    public void Part1()
    {
        int answer = input.Count(pair =>
        {
            return pair.a.x <= pair.b.x && pair.a.y >= pair.b.y ||
                   pair.b.x <= pair.a.x && pair.b.y >= pair.a.y;
        });

        Assert.Equal(503, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = input.Count(pair =>
        {
            return !(pair.a.x > pair.b.y || pair.a.y < pair.b.x || pair.b.x > pair.b.y || pair.b.y < pair.a.x);
        });

        Assert.Equal(827, answer);
    }
}
