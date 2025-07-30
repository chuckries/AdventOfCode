namespace AdventOfCode._2019;

public class Day19
{
    class Bot
    {
        public Bot(long[] program)
        {
            _bot = new IntCode(
                program,
                _input.Dequeue,
                value => _output = value);
        }

        public long Query(int x, int y)
        {
            _input.Enqueue(x);
            _input.Enqueue(y);
            _bot.Reset();
            _bot.Run();
            return _output;
        }

        IntCode _bot;
        Queue<long> _input = new Queue<long>();
        long _output = 0;
    }

    Bot _bot;

    public Day19()
    {
        _bot = new Bot(File.ReadAllText("Inputs/Day19.txt").Split(',').Select(long.Parse).ToArray());
    }

    [Fact]
    public void Part1()
    {
        int total = 0;
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                if (_bot.Query(i, j) == 1)
                    total++;
            }
        }

        Assert.Equal(112, total);
    }

    [Fact]
    public void Part2()
    {
        int xStart;
        int xEnd;
        int y;

        // find an initial range, try y = 40;
        int xCurrent = 0;
        y = 40;

        while (_bot.Query(xCurrent, y) == 0)
            xCurrent++;
        xStart = xCurrent;

        while (_bot.Query(xCurrent, y) != 0)
            xCurrent++;
        xEnd = xCurrent - 1;

        const int BoxSize = 100;

        (int x, int y) answerCoords;
        while (true)
        {
            ExtendRange(ref xStart, ref xEnd, ref y);

            if (xEnd - xStart + 1 >= BoxSize)
            {
                int candiateX = xEnd - BoxSize + 1;
                if (_bot.Query(candiateX, y + BoxSize - 1) != 0)
                {
                    answerCoords = (candiateX, y);
                    break;
                }
            }
        }

        int answer = answerCoords.x * 10_000 + answerCoords.y;
        Assert.Equal(18261982, answer);
    }

    private void ExtendRange(ref int xStart, ref int xEnd, ref int y)
    {
        y++;

        while (_bot.Query(xStart, y) == 0)
        {
            xStart++;
        }

        while (_bot.Query(xEnd + 1, y) != 0)
        {
            xEnd++;
        }
    }
}
