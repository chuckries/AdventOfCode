namespace AdventOfCode._2016;

public class Day13
{
    const int TargetX = 31;
    const int TargetY = 39;
    const int Input = 1364;

    private Dictionary<IntVec2, bool> _cells = new Dictionary<IntVec2, bool>();

    [Fact]
    public void Part1()
    {
        int answer = MinSteps((1, 1), (TargetX, TargetY));
        Assert.Equal(86, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = MaxSpaces((1, 1), 50);
        Assert.Equal(127, answer);
    }

    private int MinSteps(in IntVec2 initial, in IntVec2 target)
    {
        (IntVec2 p, int distance) current = (initial, 0);
        HashSet<IntVec2> visited = new HashSet<IntVec2>();
        Queue<(IntVec2, int)> queue = new Queue<(IntVec2, int)>();
        queue.Enqueue(current);

        while (queue.Count > 0)
        {
            current = queue.Dequeue();

            if (visited.Contains(current.p))
                continue;
            visited.Add(current.p);

            if (current.p.Equals(target))
                return current.distance;
            else
            {
                foreach (IntVec2 adj in AdjacentSpaces(current.p).Where(p => !visited.Contains(p)))
                {
                    queue.Enqueue((adj, current.distance + 1));
                }
            }
        }

        throw new InvalidOperationException();
    }

    private int MaxSpaces(in IntVec2 initial, int maxSteps)
    {
        (IntVec2 p, int distance) current = (initial, 0);
        HashSet<IntVec2> visited = new HashSet<IntVec2>();
        Queue<(IntVec2, int)> queue = new Queue<(IntVec2, int)>();
        queue.Enqueue(current);

        while (queue.Count > 0)
        {
            current = queue.Dequeue();

            if (visited.Contains(current.p))
                continue;
            visited.Add(current.p);

            if (current.distance == maxSteps)
                continue;

            foreach (IntVec2 adj in AdjacentSpaces(current.p).Where(p => !visited.Contains(p)))
            {
                queue.Enqueue((adj, current.distance + 1));
            }
        }

        return visited.Count;
    }

    private IEnumerable<IntVec2> AdjacentSpaces(in IntVec2 pos) =>
        pos.Adjacent().Where(p => p.X >= 0 && p.Y >= 0 && !IsWall(p));

    private bool IsWall(in IntVec2 p)
    {
        if (!_cells.TryGetValue(p, out bool isWall))
        {
            long key = p.X * p.X + 3 * p.X + 2 * p.X * p.Y + p.Y + p.Y * p.Y;
            key += Input;
            isWall = key.BitCount() % 2 == 1;
            _cells[p] = isWall;
        }
        return isWall;
    }
}
