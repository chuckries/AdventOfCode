namespace AdventOfCode._2019;

public class Day24
{
    class Space
    {
        class Grid
        {
            public const int Size = 5;
            public const int Middle = Size / 2;

            private bool[,] _grid;

            public readonly Space Space;
            public readonly int Level;

            public bool this[int x, int y]
            {
                get
                {
                    if (x == Middle && y == Middle)
                        throw new InvalidOperationException();
                    return _grid[x, y];
                }
                set
                {
                    if (x == Middle && y == Middle)
                        throw new InvalidOperationException();
                    _grid[x, y] = value;
                }
            }

            public Grid(Space space, int level)
            {
                Space = space;
                Level = level;
                _grid = new bool[Size, Size];
            }

            public void ApplyTo(Grid next)
            {
                if (this == next)
                    throw new InvalidOperationException();

                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (i == Middle && j == Middle)
                            continue;

                        int count = CountAdjacentOns(new IntVec2(i, j));
                        if (_grid[i, j] && count != 1)
                            next._grid[i, j] = false;
                        else if (!_grid[i, j] && count is 1 or 2)
                            next._grid[i, j] = true;
                        else next._grid[i, j] = _grid[i, j];
                    }
                }
            }

            public int CountOns()
            {
                int total = 0;
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                    {
                        if (i == Middle && j == Middle)
                            continue;

                        if (_grid[i, j])
                            total++;
                    }

                return total;
            }

            private int CountLeft()
            {
                int total = 0;
                for (int i = 0; i < Size; i++)
                    if (_grid[0, i])
                        total++;
                return total;
            }

            private int CountRight()
            {
                int total = 0;
                for (int i = 0; i < Size; i++)
                    if (_grid[Size - 1, i])
                        total++;
                return total;
            }

            private int CountTop()
            {
                int total = 0;
                for (int i = 0; i < Size; i++)
                    if (_grid[i, 0])
                        total++;
                return total;
            }

            private int CountBottom()
            {
                int total = 0;
                for (int i = 0; i < Size; i++)
                    if (_grid[i, Size - 1])
                        total++;
                return total;
            }

            private int CountAdjacentOns(IntVec2 p)
            {
                int total = 0;
                foreach (IntVec2 adj in p.Adjacent())
                {
                    total += CountAdjacentOnsFrom(p, adj);
                }

                return total;
            }

            private int CountAdjacentOnsFrom(IntVec2 p, IntVec2 adj)
            {
                if (adj.X < 0)
                    return CountFromSpace(Level - 1, g => g._grid[Middle - 1, Middle] ? 1 : 0);

                if (adj.X >= Size)
                    return CountFromSpace(Level - 1, g => g._grid[Middle + 1, Middle] ? 1 : 0);

                if (adj.Y < 0)
                    return CountFromSpace(Level - 1, g => g._grid[Middle, Middle - 1] ? 1 : 0);

                if (adj.Y >= Size)
                    return CountFromSpace(Level - 1, g => g._grid[Middle, Middle + 1] ? 1 : 0);

                if (adj == new IntVec2(Middle, Middle))
                {
                    if (p.X == Middle - 1)
                        return CountFromSpace(Level + 1, g => g.CountLeft());

                    if (p.X == Middle + 1)
                        return CountFromSpace(Level + 1, g => g.CountRight());

                    if (p.Y == Middle - 1)
                        return CountFromSpace(Level + 1, g => g.CountTop());

                    if (p.Y == Middle + 1)
                        return CountFromSpace(Level + 1, g => g.CountBottom());

                    throw new InvalidOperationException();
                }

                return _grid[adj.X, adj.Y] ? 1 : 0;
            }

            private int CountFromSpace(int level, Func<Grid, int> func)
            {
                if (!Space.TryGetGrid(level, out Grid grid))
                    return 0;

                return func(grid);
            }
        }

        private Dictionary<int, Grid> _grids = new();

        public int Size { get; private set; } = 0;

        private Space()
        {
        }

        public static Space GetEmptySpace()
        {
            return new Space();
        }

        public static Space GetStartingSpace(bool[,] start)
        {
            Space space = new Space();
            Grid grid = space.GetGrid(0);

            for (int i = 0; i < Grid.Size; i++)
                for (int j = 0; j < Grid.Size; j++)
                {
                    if (i == Grid.Middle && j == Grid.Middle)
                        continue;
                    grid[i, j] = start[i, j];
                }

            return space;
        }

        public void ApplyTo(Space next)
        {
            if (this == next)
                throw new InvalidOperationException();

            Size++;
            for (int i = -Size; i <= Size; i++)
            {
                Grid current = GetGrid(i);
                Grid nextGrid = next.GetGrid(i);
                current.ApplyTo(nextGrid);
            }
            next.Size = Size;
        }

        public int CountOns()
        {
            return _grids.Values.Sum(g => g.CountOns());
        }

        private bool TryGetGrid(int level, out Grid grid)
        {
            return _grids.TryGetValue(level, out grid);
        }

        private Grid GetGrid(int level)
        {
            if (!TryGetGrid(level, out Grid grid))
            {
                grid = new Grid(this, level);
                _grids.Add(level, grid);
            }
            return grid;
        }
    }

    string[] _input;

    public Day24()
    {
        _input = File.ReadAllLines("Inputs/Day24.txt");
    }

    [Fact]
    public void Part1()
    {

    }

    [Fact]
    public void Part2()
    {
        int answer = DoIterations(200, _input);
        Assert.Equal(1916, answer);
    }

    private int DoIterations(int iterations, string[] seed)
    {
        bool[,] start = new bool[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                start[i, j] = seed[j][i] == '#';

        Space current = Space.GetStartingSpace(start);
        Space next = Space.GetEmptySpace();

        for (int i = 0; i < iterations; i++)
        {
            current.ApplyTo(next);
            (current, next) = (next, current);
        }

        return current.CountOns();
    }
}
