using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode._2018;

public class Day10
{
    private class Point
    {
        private IntVec2 _p;
        private IntVec2 _v;

        public IntVec2 P => _p;

        public Point(IntVec2 p, IntVec2 v)
        {
            _p = p;
            _v = v;
        }

        public static Point Parse(string str)
        {
            GroupCollection groups = s_Regex.Match(str).Groups;
            return new Point(
                new IntVec2(groups["px"].Value, groups["py"].Value),
                new IntVec2(groups["vx"].Value, groups["vy"].Value));
        }

        public void Tick()
        {
            _p += _v;
        }

        public void BackTick()
        {
            _p -= _v;
        }

        private static Regex s_Regex = new Regex(
            @"^position=<\s?(?'px'-?[0-9]+),\s+(?'py'-?[0-9]+)> velocity=<\s?(?'vx'-?[0-9]+),\s+(?'vy'-?[0-9]+)>$",
            RegexOptions.Compiled);
    }

    private readonly Point[] _points;

    public Day10()
    {
        _points = File.ReadAllLines("Inputs/Day10.txt").Select(Point.Parse).ToArray();
    }

    [Fact]
    public void Part1()
    {
        RunToMinArea();

        HashSet<IntVec2> points = new HashSet<IntVec2>(_points.Select(p => p.P));
        (IntVec2 min, IntVec2 max) = points.MinMax();

        StringBuilder sb = new();
        for (int j = min.Y; j <= max.Y; j++)
        {
            sb.Append(Environment.NewLine);
            for (int i = min.X; i <= max.X; i++)
            {
                if (points.Contains(new IntVec2(i, j)))
                    sb.Append('█');
                else
                    sb.Append(' ');
            }
        }

        string answer = sb.ToString();

        const string actual = @"
█████   █    █  ██████  ██████   ████   ██████  ██████  █    █
█    █  █    █  █            █  █    █  █            █  █    █
█    █  █    █  █            █  █       █            █   █  █ 
█    █  █    █  █           █   █       █           █    █  █ 
█████   ██████  █████      █    █       █████      █      ██  
█       █    █  █         █     █       █         █       ██  
█       █    █  █        █      █       █        █       █  █ 
█       █    █  █       █       █       █       █        █  █ 
█       █    █  █       █       █    █  █       █       █    █
█       █    █  █       ██████   ████   ██████  ██████  █    █";
        Assert.Equal(actual, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = RunToMinArea();
        Assert.Equal(10634, answer);
    }

    private int RunToMinArea()
    {
        int count = 0;
        long minArea = long.MaxValue;
        while (true)
        {
            foreach (Point p in _points)
                p.Tick();

            (IntVec2 min, IntVec2 max) = _points.Select(p => p.P).MinMax();

            long area = (long)(max.X - min.X) * (long)(max.Y - min.Y);
            if (area > minArea)
                break;

            minArea = area;
            count++;
        }

        // undo final tick that goes past min
        foreach (Point p in _points)
            p.BackTick();

        return count;
    }
}
