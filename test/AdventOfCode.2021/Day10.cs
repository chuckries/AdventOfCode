namespace AdventOfCode._2021
{
    public class Day10
    {
        string[] _lines;

        public Day10()
        {
            _lines = File.ReadAllLines("Inputs/Day10.txt");
        }

        [Fact]
        public void Part1()
        {
            int total = 0;
            Action<char> corrupted = c =>
            {
                total += c switch
                {
                    ')' => 3,
                    ']' => 57,
                    '}' => 1197,
                    '>' => 25137,
                    _ => throw new InvalidOperationException()
                };
            };

            foreach (string line in _lines)
            {
                ParseLine(line, corrupted, null);
            }

            Assert.Equal(343863, total);
        }

        [Fact]
        public void Part2()
        {
            List<long> totals = new();
            long total = 0;

            Action<Stack<char>> incomplete = s =>
            {
                foreach (char c in s)
                {
                    total = total * 5 + c switch
                    {
                        '(' => 1,
                        '[' => 2,
                        '{' => 3,
                        '<' => 4,
                        _ => throw new InvalidOperationException()
                    };
                }
            };

            foreach (string line in _lines)
            {
                total = 0;
                if (ParseLine(line, null, incomplete))
                    totals.Add(total);
            }

            totals.Sort();
            long answer = totals[totals.Count / 2];

            Assert.Equal(2924734236, answer);
        }

        static bool ParseLine(string line, Action<char>? corrupted, Action<Stack<char>>? incomplete)
        {
            Stack<char> stack = new();
            foreach (char c in line)
            {
                if (c is '(' or '[' or '{' or '<')
                    stack.Push(c);
                else if (c != s_Pairs[stack.Pop()])
                {
                    corrupted?.Invoke(c);
                    return false;
                }
            }
            incomplete?.Invoke(stack);
            return true;
        }

        private static Dictionary<char, char> s_Pairs = new()
        {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '<', '>' }
        };
    }
}
