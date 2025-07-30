using System.Collections;

namespace AdventOfCode._2015;

public class Day17
{
    private class Recipe : IEquatable<Recipe>
    {
        public readonly int Size;
        BitArray _containers;

        public Recipe()
        {
            Size = 0;
            _containers = new BitArray(_sizes.Length);
        }

        protected Recipe(int size, BitArray containers)
        {
            Size = size;
            _containers = containers;
        }

        public IEnumerable<Recipe> NextCandidates()
        {
            for (int i = 0; i < _sizes.Length; i++)
            {
                if (!_containers[i])
                {
                    BitArray candidateContainers = new BitArray(_containers);
                    candidateContainers[i] = true;
                    yield return new Recipe(Size + _sizes[i], candidateContainers);
                }
            }
        }

        public int CountContainers()
        {
            int total = 0;
            for (int i = 0; i < _containers.Count; i++)
                if (_containers[i])
                    total++;

            return total;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Recipe);
        }

        public bool Equals(Recipe? other)
        {
            if (other is null)
                return false;

            for (int i = 0; i < _containers.Count; i++)
                if (_containers[i] != other._containers[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < _containers.Count; i++)
                if (_containers[i])
                    hash ^= 1 << (i % 32);

            return hash;
        }
    }

    static int[] _sizes;

    static Day17()
    {
        _sizes = File.ReadAllLines("Inputs/Day17.txt").Select(int.Parse).ToArray();
    }

    [Fact]
    public void Part1()
    {
        int answer = Backtrack().Count();
        Assert.Equal(654, answer);
    }

    [Fact]
    public void Part2()
    {
        int minCount = 0;
        int minContainers = int.MaxValue;

        foreach (Recipe r in Backtrack())
        {
            int countContainers = r.CountContainers();
            if (countContainers < minContainers)
            {
                minCount = 1;
                minContainers = countContainers;
            }
            else if (countContainers == minContainers)
                minCount++;
        }

        Assert.Equal(57, minCount);
    }

    private IEnumerable<Recipe> Backtrack()
    {
        const int Target = 150;

        Recipe current = new Recipe();
        HashSet<Recipe> visited = new HashSet<Recipe>();
        Stack<Recipe> stack = new Stack<Recipe>();
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Pop();
            if (visited.Contains(current))
                continue;
            visited.Add(current);

            if (current.Size == Target)
                yield return current;
            else
            {
                foreach (Recipe candidate in current.NextCandidates())
                    if (candidate.Size <= Target && !visited.Contains(candidate))
                        stack.Push(candidate);
            }
        }
    }
}
