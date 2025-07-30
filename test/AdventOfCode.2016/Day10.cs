namespace AdventOfCode._2016;

public class Day10
{
    private class Graph
    {
        private class Bot
        {
            private int _low;
            private int _high;
            private List<Func<int>> _valueFactories = new List<Func<int>>();

            public int GetLow()
            {
                Resolve();
                return _low;
            }

            public int GetHigh()
            {
                Resolve();
                return _high;
            }

            public void AddValueFactory(Func<int> valueFactory)
            {
                _valueFactories.Add(valueFactory);
            }

            private void Resolve()
            {
                if (_low == 0 || _high == 0)
                {
                    int val1 = _valueFactories[0]();
                    int val2 = _valueFactories[1]();

                    if (val1 < val2)
                    {
                        _low = val1;
                        _high = val2;
                    }
                    else
                    {
                        _low = val2;
                        _high = val1;
                    }
                }
            }
        }

        List<Bot> _bots = new List<Bot>();
        List<Func<int>> _outputs = new List<Func<int>>();

        public Graph(string[] input)
        {
            BuildGraph(input);
        }

        public (int low, int high) GetBotValues(int index)
        {
            Bot bot = _bots[index];
            return (bot.GetLow(), bot.GetHigh());
        }

        public int GetOutputValue(int index)
        {
            return _outputs[index]();
        }

        private void BuildGraph(string[] input)
        {
            foreach (string s in input)
            {
                string[] tokens = s.Split(' ');
                if (tokens[0] == "value")
                {
                    int value = int.Parse(tokens[1]);
                    int bot = int.Parse(tokens[5]);
                    GetBot(bot).AddValueFactory(() => value);
                }
                else if (tokens[0] == "bot")
                {
                    Bot bot = GetBot(int.Parse(tokens[1]));
                    int lowIndex = int.Parse(tokens[6]);
                    int highIndex = int.Parse(tokens[11]);

                    if (tokens[5] == "bot")
                    {
                        GetBot(lowIndex).AddValueFactory(bot.GetLow);
                    }
                    else if (tokens[5] == "output")
                    {
                        AddOutput(lowIndex, bot.GetLow);
                    }
                    else throw new InvalidOperationException();

                    if (tokens[10] == "bot")
                    {
                        GetBot(highIndex).AddValueFactory(bot.GetHigh);
                    }
                    else if (tokens[10] == "output")
                    {
                        AddOutput(highIndex, bot.GetHigh);
                    }
                    else throw new InvalidOperationException();
                }
                else throw new InvalidOperationException();
            }
        }

        private Bot GetBot(int index)
        {
            while (_bots.Count <= index)
                _bots.Add(new Bot());
            return _bots[index];
        }

        private void AddOutput(int index, Func<int> valueFactory)
        {
            while (_outputs.Count <= index)
                _outputs.Add(null);
            _outputs[index] = valueFactory;
        }
    }

    Graph _graph;

    public Day10()
    {
        _graph = new Graph(File.ReadAllLines("Inputs/Day10.txt"));
    }

    [Fact]
    public void Part1()
    {
        int i = 0;
        while (true)
        {
            if (_graph.GetBotValues(i) == (17, 61))
                break;
            i++;
        }

        Assert.Equal(161, i);
    }

    [Fact]
    public void Part2()
    {
        int answer =
            _graph.GetOutputValue(0) *
            _graph.GetOutputValue(1) *
            _graph.GetOutputValue(2);

        Assert.Equal(133163, answer);
    }
}
