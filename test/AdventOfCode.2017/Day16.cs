namespace AdventOfCode._2017;

public class Day16
{
    private class Dance
    {
        Action<char[]>[] _actions;

        public Dance(string instructions)
        {
            string[] parts = instructions.Split(',');
            _actions = new Action<char[]>[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                Action<char[]> action = null;

                string part = parts[i];
                char code = parts[i][0];
                switch (code)
                {
                    case 's':
                        {
                            int n = int.Parse(part[1..]);
                            action = (progs) => Spin(n, progs);
                            break;
                        }
                    case 'x':
                    case 'p':
                        {
                            string[] subs = part[1..].Split('/');

                            if (code == 'x')
                            {
                                int n0 = int.Parse(subs[0]);
                                int n1 = int.Parse(subs[1]);
                                action = (progs) => Exchange(n0, n1, progs);
                            }
                            else if (code == 'p')
                            {
                                char c0 = subs[0][0];
                                char c1 = subs[1][0];
                                action = (progs) => Partner(c0, c1, progs);
                            }
                            else
                                throw new InvalidOperationException();

                            break;
                        }

                    default:
                        throw new InvalidOperationException();
                }

                _actions[i] = action;
            }
        }

        public void Apply(char[] programs)
        {
            foreach (var action in _actions)
                action(programs);
        }

        private void Spin(int n, char[] programs)
        {
            char[] copy = (char[])programs.Clone();

            int start = programs.Length - n;
            int index = 0;
            for (int i = start; i < programs.Length; i++)
            {
                programs[index++] = copy[i];
            }

            for (int i = 0; i < start; i++)
            {
                programs[index++] = copy[i];
            }
        }

        private void Exchange(int pos0, int pos1, char[] programs)
        {
            (programs[pos0], programs[pos1]) = (programs[pos1], programs[pos0]);
        }

        private void Partner(char name0, char name1, char[] programs)
        {
            int i = 0;
            while (true)
            {
                if (programs[i] == name0 || programs[i] == name1)
                {
                    int pos0 = i;
                    i++;
                    while (true)
                        if (programs[i] == name0 || programs[i] == name1)
                        {
                            Exchange(pos0, i, programs);
                            return;
                        }
                        else
                            i++;
                }
                else
                    i++;
            }
        }
    }

    const string Start = "abcdefghijklmnop";
    private Dance _dance;

    public Day16()
    {
        _dance = new Dance(File.ReadAllText("Inputs/Day16.txt"));
    }

    [Fact]
    public void Part1()
    {
        char[] progs = Start.ToCharArray();
        _dance.Apply(progs);

        Assert.Equal("padheomkgjfnblic", new string(progs));
    }

    [Fact]
    public void Part2()
    {
        char[] progs = Start.ToCharArray();
        List<string> iterations = new() { new string(progs) };

        for (int i = 0; i < 1_000_000_000; i++)
        {
            _dance.Apply(progs);
            string s = new string(progs);
            if (iterations.Contains(s))
                break;

            iterations.Add(s);
        }

        string answer = iterations[1_000_000_000 % iterations.Count];

        Assert.Equal("bfcdeakhijmlgopn", answer);
    }
}
