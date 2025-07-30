using System.Text.RegularExpressions;

namespace AdventOfCode._2015;

public class Day14
{
    private class Reindeer
    {
        private enum Status
        {
            Running,
            Resting
        }

        public readonly string Name;
        public readonly int Velocity;
        public readonly int RunPeriod;
        public readonly int RestPeriod;

        private int _location;
        private Status _currentStatus;
        private int _currentCount;

        public int Location => _location;

        public Reindeer(string input)
        {
            Match match = s_Regex.Match(input);

            Name = match.Groups["name"].Value;
            Velocity = int.Parse(match.Groups["vel"].Value);
            RunPeriod = int.Parse(match.Groups["time"].Value);
            RestPeriod = int.Parse(match.Groups["rest"].Value);

            Reset();
        }

        public void Tick()
        {
            if (_currentStatus == Status.Running)
            {
                _location += Velocity;
                _currentCount++;
                if (_currentCount == RunPeriod)
                {
                    _currentCount = 0;
                    _currentStatus = Status.Resting;
                }
            }
            else if (_currentStatus == Status.Resting)
            {
                _currentCount++;
                if (_currentCount == RestPeriod)
                {
                    _currentCount = 0;
                    _currentStatus = Status.Running;
                }
            }
            else
                throw new InvalidOperationException();
        }

        public void Reset()
        {
            _location = 0;
            _currentStatus = Status.Running;
            _currentCount = 0;
        }

        private static Regex s_Regex = new Regex(
            @"^(?'name'[a-zA-Z]+) can fly (?'vel'\d+) km/s for (?'time'\d+) seconds, but then must rest for (?'rest'\d+) seconds\.$",
            RegexOptions.Compiled);
    }

    [Fact]
    public void Part1()
    {
        int answer = Parse()
            .Select(r =>
            {
                for (int i = 0; i < 2503; i++)
                    r.Tick();

                return r.Location;
            })
            .Max();

        Assert.Equal(2660, answer);
    }

    [Fact]
    public void Part2()
    {
        Reindeer[] reindeer = Parse().ToArray();
        int[] scores = new int[reindeer.Length];

        for (int i = 0; i < 2503; i++)
        {
            int max = int.MinValue;
            List<int> maxIndices = new List<int>();

            for (int j = 0; j < reindeer.Length; j++)
            {
                Reindeer r = reindeer[j];
                r.Tick();
                if (r.Location > max)
                {
                    maxIndices.Clear();
                    maxIndices.Add(j);
                    max = r.Location;
                }
                else if (r.Location == max)
                    maxIndices.Add(j);
            }

            for (int j = 0; j < maxIndices.Count; j++)
                scores[maxIndices[j]]++;
        }

        int answer = scores.Max();
        Assert.Equal(1256, answer);
    }

    private IEnumerable<Reindeer> Parse()
    {
        return File.ReadAllLines("Inputs/Day14.txt").Select(s => new Reindeer(s));
    }
}
