namespace AdventOfCode._2016;

public class Day15
{
    [Fact]
    public void Part1()
    {
        Func<int, int>[] discs = Parse().ToArray();

        int t = 0;
        while (!discs.All(d => d(t) == 0))
            t++;

        Assert.Equal(16824, t);
    }

    [Fact]
    public void Part2()
    {
        List<Func<int, int>> discs = Parse().ToList();
        int index = discs.Count;
        discs.Add(t => (t + index + 1) % 11);

        int t = 0;
        while (!discs.All(d => d(t) == 0))
            t++;

        Assert.Equal(3543984, t);
    }

    private IEnumerable<Func<int, int>> Parse()
    {
        string[] input = File.ReadAllLines("Inputs/Day15.txt");
        for (int i = 0; i < input.Length; i++)
        {
            string[] t = input[i].Split(' ');
            int positions = int.Parse(t[3]);
            int start = int.Parse(t[11].TrimEnd('.'));
            int index = i;
            yield return t => (t + start + 1 + index) % positions;
        }
    }
}
