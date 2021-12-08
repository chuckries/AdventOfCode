namespace AdventOfCode._2021
{
    public class Day08
    {
        class Line
        {
            private readonly string[] _clues;
            private readonly string[] _answers;

            private Line(string[] clues, string[] answers)
            {
                _clues = clues;
                _answers = answers;
            }

            public static Line Parse(string input)
            {
                string[] parts = input.Split('|', StringSplitOptions.TrimEntries);

                string[] clues = parts[0].Split();
                Array.Sort(clues, (lhs, rhs) => lhs.Length - rhs.Length);
                string[] answers = parts[1].Split();

                return new Line(clues, answers);
            }

            public int CountUniques() => _answers.Count(ans => ans.Length is 2 or 3 or 4 or 7);

            public int Solve()
            {
                // get segment 'a' by taking the 2 length clue from the 3 length clue
                char a_seg = _clues[1].Except(_clues[0]).First();

                // get segment 'g' by taking the 3 length and and 4 length from all 6 lengths.
                // the one that has a single char remaining is segment g
                char g_seg = _clues[6..9].Select(clue => clue.Except(_clues[1]).Except(_clues[2])).First(clue => clue.Count() == 1).First();

                // get segment 'e' by again looking at 6 length clues, but now subtracting out our g_seg as well
                char e_seg = _clues[6..9].Select(clue => clue.Except(new[] { g_seg }).Except(_clues[1]).Except(_clues[2])).First(clue => clue.Count() == 1).First();

                // get segment 'd' by taking the 2 length clue, a_seg, g_seg, and e_seg from all 5 length clues
                char d_seg = _clues[3..6].Select(clue => clue.Except(new[] { a_seg, e_seg, g_seg }).Except(_clues[0])).First(clue => clue.Count() == 1).First();

                // get segment 'b' by doing the same but also taking out d_seg
                char b_seg = _clues[3..6].Select(clue => clue.Except(new[] { a_seg, d_seg, e_seg, g_seg }).Except(_clues[0])).First(clue => clue.Count() == 1).First();

                // get segment 'c' by taking a_seg, d_seg, e_seg, and g_seg away from 5 length clues
                char c_seg = _clues[3..6].Select(clue => clue.Except(new[] { a_seg, d_seg, e_seg, g_seg })).First(clue => clue.Count() == 1).First();

                // get segment 'f' by taking all known segment from the 7 length clue
                char f_seg = _clues[9].Except(new[] { a_seg, b_seg, c_seg, d_seg, e_seg, g_seg }).First();

                Dictionary<char, char> segmentMapping = new()
                {
                    { a_seg, 'a' },
                    { b_seg, 'b' },
                    { c_seg, 'c' },
                    { d_seg, 'd' },
                    { e_seg, 'e' },
                    { f_seg, 'f' },
                    { g_seg, 'g' }
                };

                int answer = int.Parse(new string(_answers.Select(ans => s_NumberMapping[new string(ans.Select(c => segmentMapping[c]).OrderBy(c => c).ToArray())]).ToArray()));

                return answer;
            }

            private static Dictionary<string, char> s_NumberMapping = new()
            {
                { "abcefg", '0' },
                { "cf", '1' },
                { "acdeg", '2' },
                { "acdfg", '3' },
                { "bcdf", '4' },
                { "abdfg", '5' },
                { "abdefg", '6' },
                { "acf", '7' },
                { "abcdefg", '8' },
                { "abcdfg", '9' }
            };
        }

        private Line[] _lines;

        public Day08()
        {
            _lines = File.ReadAllLines("Inputs/Day08.txt").Select(Line.Parse).ToArray();
        }


        [Fact]
        public void Part1()
        {
            int answer = _lines.Sum(l => l.CountUniques());
            Assert.Equal(488, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = _lines.Sum(l => l.Solve());

            Assert.Equal(1040429, answer);
        }
    }
}