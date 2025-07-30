namespace AdventOfCode._2019;

public class Day01
{
    [Fact]
    public void Part1()
    {
        int sum = File.ReadAllLines("Inputs/Day01.txt")
            .Select(int.Parse)
            .Select(i => i / 3 - 2)
            .Sum();

        Assert.Equal(3363929, sum);
    }

    [Fact]
    public void Part2()
    {
        var modules = File.ReadAllLines("Inputs/Day01.txt")
            .Select(int.Parse);

        int total = 0;
        foreach (int module in modules)
        {
            int weight = module / 3 - 2;
            while (weight > 0)
            {
                total += weight;
                weight = weight / 3 - 2;
            }
        }

        Assert.Equal(5043026, total);
    }
}
