namespace AdventOfCode._2019;

public class Day03
{
    struct Step
    {
        public readonly IntVec2 Delta;
        public readonly int Count;

        public Step(IntVec2 delta, int count)
        {
            Delta = delta;
            Count = count;
        }

        public static Step Parse(string str)
        {
            IntVec2 delta = str[0] switch
            {
                'U' => IntVec2.UnitY,
                'D' => -IntVec2.UnitY,
                'L' => -IntVec2.UnitX,
                'R' => IntVec2.UnitX,
                _ => throw new InvalidOperationException("invalid direction")
            };

            int count = int.Parse(str.Substring(1));

            return new Step(delta, count);
        }
    }

    private Dictionary<IntVec2, (int mask, int totalSteps)> _map;

    public Day03()
    {
        Step[][] steps = File.ReadAllLines("Inputs/Day03.txt")
            .Select(l => l.Split(',')
                .Select(Step.Parse)
                .ToArray())
            .ToArray();

        _map = new Dictionary<IntVec2, (int, int)>();

        for (int i = 0; i < steps.Length; i++)
        {
            DoSteps(_map, steps[i], i);
        }
    }

    [Fact]
    public void Part1()
    {
        int answer = _map.Where(kvp => kvp.Value.mask == 3)
            .Min(kvp => kvp.Key.Manhattan);

        Assert.Equal(248, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = _map.Values.Where(v => v.mask == 3)
            .Min(v => v.totalSteps);

        Assert.Equal(28580, answer);
    }

    private void DoSteps(Dictionary<IntVec2, (int mask, int totalSteps)> map, Step[] steps, int id)
    {
        IntVec2 current = new IntVec2(0, 0);
        int totalSteps = 0;
        foreach (Step step in steps)
        {
            for (int i = 0; i < step.Count; i++)
            {
                current += step.Delta;
                totalSteps++;

                (int mask, int totalStesp) state;
                if (!map.TryGetValue(current, out state))
                {
                    map.Add(current, (1 << id, totalSteps));
                }
                else
                {
                    if ((map[current].mask & (1 << id)) == 0)
                    {
                        map[current] = (map[current].mask | (1 << id), map[current].totalSteps + totalSteps);
                    }
                }

            }
        }
    }
}
