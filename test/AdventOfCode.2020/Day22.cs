namespace AdventOfCode._2020;

public class Day22
{
    private class Game
    {
        private Queue<int> _deck0;
        private Queue<int> _deck1;

        public Game(string[] input)
        {
            _deck0 = new(input.Length);
            _deck1 = new(input.Length);
            string line;
            int i = 1;
            while (!string.IsNullOrEmpty(line = input[i++]))
            {
                _deck0.Enqueue(int.Parse(line));
            }
            i++;
            while (i < input.Length)
            {
                _deck1.Enqueue(int.Parse(input[i++]));
            }
        }

        public long Run()
        {
            while (_deck0.Count > 0 && _deck1.Count > 0)
                Step();

            var winner = _deck0.Count == 0 ? _deck1 : _deck0;

            return ScoreDeck(winner);
        }

        public long RunRecursive() =>
            ScoreDeck(RunRecursiveInternal(_deck0, _deck1) == -1 ? _deck0 : _deck1);

        private void Step()
        {
            int a = _deck0.Dequeue();
            int b = _deck1.Dequeue();

            if (a > b)
            {
                _deck0.Enqueue(a);
                _deck0.Enqueue(b);
            }
            else
            {
                _deck1.Enqueue(b);
                _deck1.Enqueue(a);
            }
        }

        private static int RunRecursiveInternal(Queue<int> deck0, Queue<int> deck1)
        {
            HashSet<int> states = new();

            while (true)
            {
                if (StepRecursive(deck0, deck1, states))
                    return -1;

                if (deck0.Count == 0 || deck1.Count == 0)
                    return deck0.Count == 0 ? 1 : -1;
            }
        }

        private static bool StepRecursive(Queue<int> deck0, Queue<int> deck1, HashSet<int> states)
        {
            int id = GetId(deck0, deck1);
            if (states.Contains(id))
                return true;

            states.Add(id);

            int a = deck0.Dequeue();
            int b = deck1.Dequeue();

            int winner;
            if (deck0.Count >= a && deck1.Count >= b)
                winner = RunRecursiveInternal(new Queue<int>(deck0.Take(a)), new Queue<int>(deck1.Take(b)));
            else
                winner = a > b ? -1 : 1;

            if (winner == -1)
            {
                deck0.Enqueue(a);
                deck0.Enqueue(b);
            }
            else
            {
                deck1.Enqueue(b);
                deck1.Enqueue(a);
            }

            return false;
        }

        private static int GetId(Queue<int> deck0, Queue<int> deck1)
        {
            HashCode hashCode = new HashCode();
            hashCode.Add("1:");
            foreach (int i in deck0)
                hashCode.Add(i);
            hashCode.Add("2:");
            foreach (int i in deck1)
                hashCode.Add(i);
            return hashCode.ToHashCode();
        }

        private static long ScoreDeck(Queue<int> deck)
        {
            int i = deck.Count;
            long total = 0;
            foreach (int card in deck)
            {
                total += card * i--;
            }
            return total;
        }
    }

    Game _game;

    public Day22()
    {
        _game = new Game(File.ReadAllLines("Inputs/Day22.txt"));
    }

    [Fact]
    public void Part1()
    {
        long answer = _game.Run();
        Assert.Equal(33559, answer);
    }

    [Fact]
    public void Part2()
    {
        long answer = _game.RunRecursive();
        Assert.Equal(32789, answer);
    }

}
