namespace AdventOfCode._2021
{
    public class Day14
    {
        readonly int[] _start;
        Dictionary<IntVec2, int> _rules;

        public Day14()
        {
            string[] lines = File.ReadAllLines("Inputs/Day14.txt");

            _start = lines[0].Select(c => c - 'A').ToArray();
            _rules = lines[2..].Select(l => l.Split(" -> ")).ToDictionary(tok => new IntVec2(tok[0][0] - 'A', tok[0][1] -'A'), tok => tok[1][0] - 'A');
        }

        [Fact]
        public void Part1()
        {
            long answer = Count(10);
            Assert.Equal(2587, answer);
        }

        [Fact]
        public void Part2()
        {
            long answer = Count(40);
            Assert.Equal(3318837563123, answer);
        }

        public long Count(int iterations)
        {
            Dictionary<IntVec2, long> pairs = new();
            long[] counts = new long[26];

            for (int i = 0; i < _start.Length - 1; i++)
            {
                counts[_start[i]]++;
                AddPairCount(new IntVec2(_start[i], _start[i + 1]), 1);
            }
            counts[_start[^1]]++;

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                var oldPairs = pairs.ToArray();
                pairs.Clear();
                foreach (var kvp in oldPairs)
                {
                    int next = _rules[kvp.Key];
                    counts[next] += kvp.Value;
                    AddPairCount(new IntVec2(kvp.Key.X, next), kvp.Value);
                    AddPairCount(new IntVec2(next, kvp.Key.Y), kvp.Value);
                }
            }

            long max = long.MinValue;
            long min = long.MaxValue;
            foreach (long count in counts)
            {
                if (count == 0)
                    continue;
                if (count > max)
                    max = count;
                if (count < min)
                    min = count;
            }

            return max - min;

            void AddPairCount(IntVec2 pair, long count)
            {
                if (!pairs.ContainsKey(pair))
                    pairs[pair] = count;
                else
                    pairs[pair] += count;
            }
        }
    }
}
