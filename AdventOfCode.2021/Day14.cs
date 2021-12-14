namespace AdventOfCode._2021
{
    public class Day14
    {
        readonly int[] _start;
        readonly int[,] _rules;

        public Day14()
        {
            string[] lines = File.ReadAllLines("Inputs/Day14.txt");

            _start = lines[0].Select(c => c - 'A').ToArray();
            _rules = new int[26, 26];
            foreach (string[] tok in lines.Skip(2).Select(l => l.Split(" -> ")))
            {
                _rules[tok[0][0] - 'A', tok[0][1] - 'A'] = tok[1][0] - 'A';
            }
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
            long[,] pairs = new long[26, 26];
            long[] counts = new long[26];

            for (int i = 0; i < _start.Length - 1; i++)
            {
                counts[_start[i]]++;
                pairs[_start[i], _start[i + 1]]++;
            }
            counts[_start[^1]]++;

            long current;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                long[,] newPairs = new long[26, 26];
                for (int i = 0; i < 26; i++)
                    for (int j = 0; j < 26; j++)
                        if (0 != (current = pairs[i, j]))
                        {
                            int next = _rules[i, j];
                            counts[next] += current;
                            newPairs[i, next] += current;
                            newPairs[next, j] += current;
                        }

                pairs = newPairs;
            }

            long max = long.MinValue;
            long min = long.MaxValue;
            foreach (long count in counts.Where(c => c != 0))
            {
                if (count > max)
                    max = count;
                if (count < min)
                    min = count;
            }

            return max - min;
        }
    }
}
