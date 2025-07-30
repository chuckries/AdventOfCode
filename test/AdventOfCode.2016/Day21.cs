namespace AdventOfCode._2016;

public class Day21
{
    private class Scrambler
    {
        private interface IScrambleStep
        {
            void Scramble(char[] s);
            void Unscramble(char[] s);
        }

        private class SwapPositionStep : IScrambleStep
        {
            private int _x;
            private int _y;

            public SwapPositionStep(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public void Scramble(char[] s) => SwapPosition(_x, _y, s);

            public void Unscramble(char[] s) => SwapPosition(_x, _y, s);
        }

        private class SwapLettersStep : IScrambleStep
        {
            private char _x;
            private char _y;

            public SwapLettersStep(char x, char y)
            {
                _x = x;
                _y = y;
            }

            public void Scramble(char[] s) => SwapLetters(_x, _y, s);

            public void Unscramble(char[] s) => SwapLetters(_x, _y, s);
        }

        private class RotateStep : IScrambleStep
        {
            private int _x;

            public RotateStep(int x)
            {
                _x = x;
            }

            public void Scramble(char[] s) => Rotate(_x, s);

            public void Unscramble(char[] s) => Rotate(-_x, s);
        }

        private class RotateLetterPositionStep : IScrambleStep
        {
            private char _x;

            public RotateLetterPositionStep(char x)
            {
                _x = x;
            }

            public void Scramble(char[] s) => RotateLetterPosition(_x, s);

            public void Unscramble(char[] s)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    char[] candidate = (char[])s.Clone();
                    Rotate(i, candidate);

                    char[] rotated = (char[])candidate.Clone();
                    RotateLetterPosition(_x, rotated);

                    if (rotated.SequenceEqual(s))
                    {
                        for (int j = 0; j < s.Length; j++)
                            s[j] = candidate[j];
                        return;
                    }
                }

                throw new InvalidOperationException();
            }
        }

        private class ReverseStep : IScrambleStep
        {
            private int _x;
            private int _y;

            public ReverseStep(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public void Scramble(char[] s) => Reverse(_x, _y, s);

            public void Unscramble(char[] s) => Reverse(_x, _y, s);
        }

        private class MoveStep : IScrambleStep
        {
            private int _x;
            private int _y;

            public MoveStep(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public void Scramble(char[] s) => Move(_x, _y, s);

            public void Unscramble(char[] s) => Move(_y, _x, s);
        }

        IScrambleStep[] _scramble;

        public Scrambler(string[] scramble)
        {
            _scramble = Parse(scramble).ToArray();

            static IEnumerable<IScrambleStep> Parse(string[] scramble)
            {
                foreach (string str in scramble)
                {
                    string[] t = str.Split(' ');
                    if (t[0] == "swap")
                    {
                        if (t[1] == "position")
                        {
                            int x = int.Parse(t[2]);
                            int y = int.Parse(t[5]);
                            yield return new SwapPositionStep(x, y);
                        }
                        else if (t[1] == "letter")
                        {
                            char x = t[2][0];
                            char y = t[5][0];
                            yield return new SwapLettersStep(x, y);
                        }
                        else throw new InvalidOperationException();
                    }
                    else if (t[0] == "rotate")
                    {
                        if (t[1] == "left" || t[1] == "right")
                        {
                            int x = int.Parse(t[2]);
                            if (t[1] == "right")
                                x *= -1;
                            yield return new RotateStep(x);
                        }
                        else if (t[1] == "based")
                        {
                            char x = t[6][0];
                            yield return new RotateLetterPositionStep(x);
                        }
                        else throw new InvalidOperationException();
                    }
                    else if (t[0] == "reverse")
                    {
                        int x = int.Parse(t[2]);
                        int y = int.Parse(t[4]);
                        yield return new ReverseStep(x, y);
                    }
                    else if (t[0] == "move")
                    {
                        int x = int.Parse(t[2]);
                        int y = int.Parse(t[5]);
                        yield return new MoveStep(x, y);
                    }
                    else throw new InvalidOperationException();
                }
            }
        }

        public string Scramble(string input)
        {
            char[] s = input.ToCharArray();
            for (int i = 0; i < _scramble.Length; i++)
                _scramble[i].Scramble(s);
            return new string(s);
        }

        public string Unscramble(string input)
        {
            char[] s = input.ToCharArray();
            for (int i = _scramble.Length - 1; i >= 0; i--)
                _scramble[i].Unscramble(s);
            return new string(s);
        }

        private static void SwapPosition(int x, int y, char[] s)
        {
            char tmp = s[x];
            s[x] = s[y];
            s[y] = tmp;
        }

        private static void SwapLetters(char x, char y, char[] s)
        {
            int ix = 0;
            int iy = 0;

            int i = 0;
            while (true)
            {
                if (s[i] == x)
                {
                    ix = i;
                    break;
                }
                else if (s[i] == y)
                {
                    iy = i;
                    break;
                }
                i++;
            }

            i++;
            while (true)
            {
                if (s[i] == x)
                {
                    ix = i;
                    break;
                }
                else if (s[i] == y)
                {
                    iy = i;
                    break;
                }
                i++;
            }

            SwapPosition(ix, iy, s);
        }

        private static void Rotate(int x, char[] s)
        {
            x = (x % s.Length + s.Length) % s.Length;
            char[] tmp = (char[])s.Clone();
            for (int i = 0; i < s.Length; i++)
                s[i] = tmp[(i + x) % s.Length];
        }

        private static void RotateLetterPosition(char x, char[] s)
        {
            int i = 0;
            for (; i < s.Length; i++)
                if (s[i] == x)
                    break;

            int r = 1 + i;
            if (i >= 4)
                r++;

            Rotate(-r, s);
        }

        private static void Reverse(int x, int y, char[] s)
        {
            int l = y - x + 1;
            char[] tmp = new char[l];
            for (int i = 0; i < l; i++)
                tmp[i] = s[i + x];
            for (int i = 0; i < l; i++)
                s[i + x] = tmp[l - i - 1];
        }

        private static void Move(int x, int y, char[] s)
        {
            char c = s[x];

            if (y < x)
            {
                for (int i = x; i > y; i--)
                    s[i] = s[i - 1];
                s[y] = c;
            }
            else if (y > x)
            {
                for (int i = x; i < y; i++)
                    s[i] = s[i + 1];
                s[y] = c;
            }
        }
    }

    Scrambler _scrambler;

    public Day21()
    {
        _scrambler = new Scrambler(File.ReadAllLines("Inputs/Day21.txt"));
    }

    [Fact]
    public void Part1()
    {
        string answer = _scrambler.Scramble("abcdefgh");
        Assert.Equal("baecdfgh", answer);
    }

    [Fact]
    public void Part2()
    {
        string answer = _scrambler.Unscramble("fbgdceah");
        Assert.Equal("cegdahbf", answer);
    }
}
