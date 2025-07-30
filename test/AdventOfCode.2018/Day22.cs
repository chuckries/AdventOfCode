using System.Diagnostics;

namespace AdventOfCode._2018;

public class Day22
{
    const int Depth = 10689;
    const int TargetX = 11;
    const int TargetY = 722;
    static readonly IntVec2 TargetCoord = new IntVec2(TargetX, TargetY);

    enum Terrain : int
    {
        Rocky = 0,
        Wet = 1,
        Narrow = 2,
        Invalid = int.MaxValue
    }

    enum Tool
    {
        None = 0,
        Climbing = 1,
        Torch = 2,
        Invalid = int.MaxValue
    }

    [DebuggerDisplay("{Coord}, {Terrain}")]
    private readonly record struct Region(
        IntVec2 Coord,
        int Erosion,
        Terrain Terrain);

    private readonly record struct SearchCoord(
        IntVec2 Coord,
        Tool Tool);

    [DebuggerDisplay("{Tool}, {Region}, {Time}")]
    struct Node
    {
        public readonly Region Region;
        public readonly Tool Tool;
        public readonly int Time;
        public readonly SearchCoord SearchCoord;

        public Node(Region region, Tool tool, int time)
        {
            Region = region;
            Tool = tool;
            Time = time;
            SearchCoord = new SearchCoord(Region.Coord, Tool);
        }
    }

    Dictionary<IntVec2, Region> _regions = new Dictionary<IntVec2, Region>();

    [Fact]
    public void Part1()
    {
        int answer = 0;
        for (int i = 0; i <= TargetCoord.X; i++)
            for (int j = 0; j <= TargetCoord.Y; j++)
                answer += (int)GetRegion(new IntVec2(i, j)).Terrain;

        Assert.Equal(8575, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = Search();
        Assert.Equal(999, answer);
    }

    private int Search()
    {
        IEnumerable<Node> GetAdjacentNodes(Node current)
        {
            Tool newTool = OtherValidTool[(int)current.Region.Terrain, (int)current.Tool];
            yield return new Node(current.Region, newTool, current.Time + 7);

            foreach (IntVec2 adjacent in current.Region.Coord.Adjacent())
            {
                if (adjacent.X >= 0 && adjacent.Y >= 0)
                {
                    Region region = GetRegion(adjacent);
                    if (ValidToolRegions[(int)current.Tool, (int)region.Terrain])
                        yield return new Node(region, current.Tool, current.Time + 1);
                }
            }
        }

        PriorityQueue<Node, int> searchSet = new PriorityQueue<Node, int>(TargetCoord.X * TargetCoord.Y);
        Dictionary<SearchCoord, int> dists = new(TargetCoord.X * TargetCoord.Y * 2);

        Node start = new Node(GetRegion(IntVec2.Zero), Tool.Torch, 0);
        searchSet.Enqueue(start, 0);
        dists.Add(start.SearchCoord, 0);

        while (searchSet.Count > 0)
        {
            Node current = searchSet.Dequeue();

            if (current.Region.Coord == TargetCoord && current.Tool == Tool.Torch)
                return current.Time;

            if (dists[current.SearchCoord] == current.Time)
                foreach (Node adjacent in GetAdjacentNodes(current))
                    if (!dists.TryGetValue(adjacent.SearchCoord, out int adjDist) || adjacent.Time < adjDist)
                    {
                        dists[adjacent.SearchCoord] = adjacent.Time;
                        searchSet.Enqueue(adjacent, adjacent.Time + adjacent.Region.Coord.ManhattanDistanceFrom(TargetCoord));
                    }
        }

        throw new InvalidOperationException();
    }

    private Region GetRegion(in IntVec2 coord)
    {
        if (!_regions.TryGetValue(coord, out Region region))
        {
            int index = coord switch
            {
                (0, 0) or (TargetX, TargetY) => 0,
                (int X, 0) => X * 16807,
                (0, int Y) => Y * 48271,
                _ => GetRegion(coord - IntVec2.UnitX).Erosion * GetRegion(coord - IntVec2.UnitY).Erosion
            };

            int erosion = (index + Depth) % 20183;
            Terrain terrain = (Terrain)(erosion % 3);

            _regions.Add(coord, region = new Region(coord, erosion, terrain));
        }
        return region;
    }

    private static Tool[,] OtherValidTool =
    {
            { Tool.Invalid, Tool.Torch, Tool.Climbing },
            { Tool.Climbing, Tool.None, Tool.Invalid },
            { Tool.Torch, Tool.Invalid, Tool.None }
        };

    private static bool[,] ValidToolRegions =
    {
            { false, true, true },
            { true, true, false },
            { true, false, true },
        };
}
