namespace AdventOfCode._2021
{
    public class Day08
    {
        class Line
        {
            string[] _clues;
            string[] _answers;

            public static Line Parse(string input)
            {
                string[] parts = input.Split('|', StringSplitOptions.TrimEntries);

                Line line = new Line();
                line._clues = parts[0].Split();
                line._answers = parts[1].Split();
                Array.Sort(line._clues, (lhs, rhs) => lhs.Length - rhs.Length);

                return line;
            }

            public int Solve()
            {
                // get segment 'a' by taking the two length clue from the 3 length clue
                char a_seg = _clues[1].First(c => !_clues[0].Contains(c));

                // get segment 'g' by taking segment the three length and and 4 length from all 6 lengths.
                // the one the has a single char remaining is segment g
                char g_seg = _clues[6..9].Select(clue => clue.Where(c => !_clues[1].Contains(c) && !_clues[2].Contains(c)).ToArray())
                    .First(clue => clue.Length == 1)[0];

                // get segment 'e' by again looking at 6 length clues, but now subtracting out our g_seg as well
                char e_seg = _clues[6..9].Select(clue => clue.Where(c => c != g_seg && !_clues[1].Contains(c) && !_clues[2].Contains(c)).ToArray())
                    .First(clue => clue.Length == 1)[0];

                // get segment 'd' by taking the 2 length clue, a_seg, g_seg, and e_seg from all 5 length clues
                char d_seg = _clues[3..6].Select(clue => clue.Where(c => c != a_seg &&  c != g_seg && c != e_seg && !_clues[0].Contains(c)).ToArray())
                    .First(clue => clue.Length == 1)[0];

                // get segment 'b' by doing the same but also taking out d_seg
                char b_seg = _clues[3..6].Select(clue => clue.Where(c => c != a_seg && c != d_seg &&  c != g_seg && c != e_seg && !_clues[0].Contains(c)).ToArray())
                    .First(clue => clue.Length == 1)[0];

                // get segment 'c' by taking a_seg, d_seg, e_seg, and g_seg away from 5 length clues
                char c_seg = _clues[3..6].Select(clue => clue.Where(c => c != a_seg && c != d_seg && c != e_seg && c != g_seg).ToArray())
                    .First(clue => clue.Length == 1)[0];

                // get segment 'f' by taking all known segment from the 7 length clue
                char f_seg = _clues[9].First(c => c != a_seg && c != b_seg && c != c_seg && c != d_seg && c != e_seg && c != g_seg);

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

                Dictionary<string, char> numberMapping = new()
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

                int answer = int.Parse(new string(_answers.Select(ans => numberMapping[new string(ans.Select(c => segmentMapping[c]).OrderBy(c => c).ToArray())]).ToArray()));

                return answer;
            }
        }

        [Fact]
        public void Part1()
        {
            int total = 0;

            foreach (var line in File.ReadAllLines("Inputs/Day08.txt"))
            {
                int index = line.IndexOf('|');
                total += line.Substring(index + 2).Split().Count(s => s.Length == 2 || s.Length == 3 || s.Length == 4 || s.Length == 7);
            }

            Assert.Equal(488, total);
        }

        [Fact]
        public void Part2()
        {
            Line[] lines = File.ReadAllLines("Inputs/Day08.txt").Select(Line.Parse).ToArray();

            int answer = lines.Select(l => l.Solve()).Sum();

            Assert.Equal(1040429, answer);
        }
    }
}