using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AdventOfCode._2020;

public class Day16
{
    [DebuggerDisplay("{Low0}-{High0} {Low1}-{High1}")]
    private class Range
    {
        public readonly string Name;
        public readonly IntVec2 Range0;
        public readonly IntVec2 Range1;

        public Range(string line)
        {
            Match match = s_Regex.Match(line);
            Name = match.Groups["name"].Value;
            Range0 = new IntVec2(match.Groups["lo0"].Value, match.Groups["hi0"].Value);
            Range1 = new IntVec2(match.Groups["lo1"].Value, match.Groups["hi1"].Value);
        }

        public bool Test(int n)
        {
            return (n >= Range0.X && n <= Range0.Y) || (n >= Range1.X && n <= Range1.Y);
        }

        private static Regex s_Regex = new Regex(
            @"^(?'name'\w+( \w+)?): (?'lo0'\d+)-(?'hi0'\d+) or (?'lo1'\d+)-(?'hi1'\d+)$",
            RegexOptions.Compiled);
    }

    private Range[] _ranges;
    private int[] _myTicket;
    private int[][] _tickets;

    public Day16()
    {
        string[] lines = File.ReadAllLines("Inputs/Day16.txt");
        int index = 0;

        List<Range> ranges = new(lines.Length);
        string current;
        while (!string.IsNullOrEmpty(current = lines[index]))
        {
            ranges.Add(new Range(current));
            index++;
        }
        _ranges = ranges.ToArray();
        index += 2;

        _myTicket = lines[index].Split(',').Select(int.Parse).ToArray();

        index += 3;
        List<int[]> tickets = new(lines.Length);
        for (; index < lines.Length; index++)
        {
            tickets.Add(lines[index].Split(',').Select(int.Parse).ToArray());
        }
        _tickets = tickets.ToArray();

    }

    [Fact]
    public void Part1()
    {
        int answer = 0;
        foreach (int[] ticket in _tickets)
            foreach (int n in ticket)
                if (!TestNum(n))
                    answer += n;

        Assert.Equal(30869, answer);
    }

    [Fact]
    public void Part2()
    {
        int[][] validTickets = _tickets.Where(TestTicket).ToArray();

        List<(Range range, List<int> candidates)> validColumns = _ranges.Select(r =>
        {
            List<int> matches = new List<int>();
            for (int i = 0; i < _ranges.Length; i++)
            {
                if (TestColumn(validTickets, i, r))
                    matches.Add(i);
            }
            return (r, matches);
        }).ToList();

        Range[] correct = new Range[_ranges.Length];
        while (validColumns.Count > 0)
        {
            var toRemove = validColumns.First(c => c.candidates.Count == 1);
            int index = toRemove.candidates[0];
            correct[index] = toRemove.range;
            validColumns.Remove(toRemove);
            foreach (var remaining in validColumns)
                remaining.candidates.Remove(index);
        }

        long answer = 1;
        for (int i = 0; i < correct.Length; i++)
            if (correct[i].Name.StartsWith("departure"))
                answer *= _myTicket[i];

        Assert.Equal(4381476149273, answer);
    }

    private bool TestColumn(int[][] tickets, int index, Range range) =>
        tickets.All(t => range.Test(t[index]));

    private bool TestTicket(int[] ticket) =>
        ticket.All(n => TestNum(n));

    private bool TestNum(int n) =>
        _ranges.Any(r => r.Test(n));
}
