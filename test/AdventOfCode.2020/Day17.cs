namespace AdventOfCode._2020;

public class Day17
{
    [Fact]
    public void Part1()
    {
        const int TargetCycles = 6;
        const int Growth = 2 * (TargetCycles + 1);

        string[] input = File.ReadAllLines("Inputs/Day17.txt");
        IntVec2 inputBounds = (input[0].Length, input.Length);

        IntVec3 bounds = (
            inputBounds.X + Growth,
            inputBounds.Y + Growth,
            1 + Growth);

        (IntVec3 low, IntVec3 high) bounding = (
            (TargetCycles, TargetCycles, TargetCycles),
            (inputBounds.X + TargetCycles + 2, inputBounds.Y + TargetCycles + 2, TargetCycles + 2));

        bool[,,] current = new bool[
            bounds.X + 1,
            bounds.Y + 1,
            bounds.Z + 1];
        bool[,,] next = (bool[,,])current.Clone();

        for (int j = 0; j < input.Length; j++)
            for (int i = 0; i < input[j].Length; i++)
                current[i + TargetCycles + 1, j + TargetCycles + 1, TargetCycles + 1] = input[j][i] == '#';

        IntVec3[] dirs = IntVec3.Zero.Surrounding().ToArray();
        for (int cycles = 0; cycles < TargetCycles; cycles++)
        {
            for (int i = bounding.low.X; i <= bounding.high.X; i++)
                for (int j = bounding.low.Y; j <= bounding.high.Y; j++)
                    for (int k = bounding.low.Z; k <= bounding.high.Z; k++)
                    {
                        bool active = current[i, j, k];
                        int adjActive = dirs.Count(adj => current[i + adj.X, j + adj.Y, k + adj.Z]);

                        next[i, j, k] = active ? adjActive is 2 or 3 : adjActive is 3;
                    }

            (current, next) = (next, current);

            bounding = (bounding.low - 1, bounding.high + 1);
        }

        int answer = 0;
        for (int i = 1; i < bounds.X; i++)
            for (int j = 1; j < bounds.Y; j++)
                for (int k = 1; k < bounds.Z; k++)
                    if (current[i, j, k])
                        ++answer;

        Assert.Equal(384, answer);
    }

    [Fact]
    public void Part2()
    {
        const int TargetCycles = 6;
        const int Growth = 2 * (TargetCycles + 1);

        string[] input = File.ReadAllLines("Inputs/Day17.txt");
        IntVec2 inputBounds = (input[0].Length, input.Length);

        IntVec4 bounds = (
            inputBounds.X + Growth,
            inputBounds.Y + Growth,
            1 + Growth,
            1 + Growth);

        (IntVec4 low, IntVec4 high) bounding = (
            (TargetCycles, TargetCycles, TargetCycles, TargetCycles),
            (inputBounds.X + TargetCycles + 2, inputBounds.Y + TargetCycles + 2, TargetCycles + 2, TargetCycles + 2));

        bool[,,,] current = new bool[
            bounds.X + 1,
            bounds.Y + 1,
            bounds.Z + 1,
            bounds.W + 1];
        bool[,,,] next = (bool[,,,])current.Clone();

        for (int j = 0; j < input.Length; j++)
            for (int i = 0; i < input[j].Length; i++)
                current[i + TargetCycles + 1, j + TargetCycles + 1, TargetCycles + 1, TargetCycles + 1] = input[j][i] == '#';

        IntVec4[] dirs = IntVec4.Zero.Surrounding().ToArray();
        for (int cycles = 0; cycles < TargetCycles; cycles++)
        {
            for (int i = bounding.low.X; i <= bounding.high.X; i++)
                for (int j = bounding.low.Y; j <= bounding.high.Y; j++)
                    for (int k = bounding.low.Z; k <= bounding.high.Z; k++)
                        for (int l = bounding.low.W; l <= bounding.high.W; l++)
                        {
                            bool active = current[i, j, k, l];
                            int adjActive = dirs.Count(adj => current[i + adj.X, j + adj.Y, k + adj.Z, l + adj.W]);

                            next[i, j, k, l] = active ? adjActive is 2 or 3 : adjActive is 3;
                        }

            (current, next) = (next, current);

            bounding = (bounding.low - 1, bounding.high + 1);
        }

        int answer = 0;
        for (int i = 1; i < bounds.X; i++)
            for (int j = 1; j < bounds.Y; j++)
                for (int k = 1; k < bounds.Z; k++)
                    for (int l = 1; l < bounds.W; l++)
                        if (current[i, j, k, l])
                            ++answer;

        Assert.Equal(2012, answer);
    }

    [Fact(Skip = "alt")]
    public void Part1_Different()
    {
        int answer = Solve(3);
        Assert.Equal(384, answer);
    }

    [Fact(Skip = "alt")]
    public void Part2_Different()
    {
        int answer = Solve(4);
        Assert.Equal(2012, answer);
    }

    private int Solve(int dimension)
    {
        const int TargetCycles = 6;
        const int Growth = 2 * (TargetCycles + 1);

        string[] input = File.ReadAllLines("Inputs/Day17.txt");
        int m = input[0].Length;
        int n = input.Length;

        int[] boundsArr = new int[dimension];
        boundsArr[0] = m + Growth + 1;
        boundsArr[1] = n + Growth + 1;
        for (int i = 2; i < dimension; i++)
            boundsArr[i] = 2 + Growth;
        TupleVec bounds = new TupleVec(boundsArr);

        int[] boundingArrMax = new int[dimension];
        boundingArrMax[0] = m + TargetCycles + 2;
        boundingArrMax[1] = n + TargetCycles + 2;
        for (int i = 2; i < boundingArrMax.Length; i++)
            boundingArrMax[i] = TargetCycles + 2;

        (TupleVec min, TupleVec max) bounding = (TupleVec.FromRepeat(TargetCycles, dimension), new TupleVec(boundingArrMax));

        Array current = Array.CreateInstance(typeof(object), boundsArr);
        Array next = (Array)current.Clone();

        int[] indices = new int[dimension];
        for (int i = 0; i < indices.Length; i++)
            indices[i] = TargetCycles + 1;

        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
            {
                indices[0] += i;
                indices[1] += j;
                current.SetValue(input[j][i] == '#' ? s_Sentinel : null, indices);
                indices[0] -= i;
                indices[1] -= j;
            }

        TupleVec zero = TupleVec.FromRepeat(0, dimension);
        TupleVec one = TupleVec.FromRepeat(1, dimension);

        TupleVec[] dirs = TupleVec.EnumerateSpace(-one, one).Where(dir => !dir.Equals(zero)).ToArray();

        for (int cycles = 0; cycles < TargetCycles; cycles++)
        {
            foreach (TupleVec p in TupleVec.EnumerateSpace(bounding.min, bounding.max))
            {
                bool active = p.GetValue(current) is not null;
                int adjActive = dirs.Count(dir => (p + dir).GetValue(current) is not null);

                p.SetValue(next, (active ? adjActive is 2 or 3 : adjActive is 3) ? s_Sentinel : null);
            }

            (current, next) = (next, current);

            bounding = (bounding.min - one, bounding.max + one);
        }

        int total = 0;
        foreach (TupleVec p in TupleVec.EnumerateSpace(zero, bounds - one))
            if (p.GetValue(current) is not null)
                ++total;

        return total;
    }

    static object s_Sentinel = new object();

    private readonly struct TupleVec : IEquatable<TupleVec>
    {
        private readonly int[] _values;

        public int Dimension { get; }

        public TupleVec(params int[] values)
        {
            _values = (int[])values.Clone();
            Dimension = _values.Length;
        }

        public TupleVec(TupleVec other)
            : this(other._values)
        {
        }

        public object GetValue(Array arr)
        {
            return arr.GetValue(_values);
        }

        public void SetValue(Array arr, object o)
        {
            arr.SetValue(o, _values);
        }

        public static TupleVec FromRepeat(int value, int dimension)
        {
            return new TupleVec(Enumerable.Repeat(value, dimension).ToArray());
        }

        public static IEnumerable<TupleVec> EnumerateSpace(TupleVec min, TupleVec max)
        {
            if (min.Dimension != max.Dimension)
                throw new InvalidOperationException();

            for (int i = 0; i < min.Dimension; i++)
            {
                if (max._values[i] < min._values[i])
                    throw new InvalidOperationException();
            }

            int[] next = (int[])min._values.Clone();
            while (true)
            {
                yield return new TupleVec(next);
                int index = 0;
                while (index < next.Length)
                {
                    if (next[index] == max._values[index])
                    {
                        next[index] = min._values[index];
                        index++;
                    }
                    else
                    {
                        ++next[index];
                        break;
                    }
                }
                if (index == next.Length)
                    break;
            }
        }

        public static TupleVec operator -(TupleVec a)
        {
            TupleVec ret = new TupleVec(a);
            for (int i = 0; i < ret.Dimension; i++)
                ret._values[i] = -ret._values[i];
            return ret;
        }

        public static TupleVec operator +(TupleVec a, TupleVec b)
        {
            if (a.Dimension != b.Dimension)
                throw new InvalidOperationException();

            TupleVec ret = new TupleVec(a);
            for (int i = 0; i < a.Dimension; i++)
                ret._values[i] += b._values[i];
            return ret;
        }

        public static TupleVec operator -(TupleVec a, TupleVec b)
        {
            if (a.Dimension != b.Dimension)
                throw new InvalidOperationException();

            TupleVec ret = new TupleVec(a);
            for (int i = 0; i < a.Dimension; i++)
                ret._values[i] -= b._values[i];
            return ret;
        }

        public bool Equals(TupleVec other)
        {
            if (Dimension != other.Dimension)
                return false;

            for (int i = 0; i < Dimension; i++)
                if (_values[i] != other._values[i])
                    return false;

            return true;
        }
    }
}
