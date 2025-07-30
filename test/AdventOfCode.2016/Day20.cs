using System.Text.RegularExpressions;

namespace AdventOfCode._2016;

public class Day20
{
    LinkedList<(uint begin, uint end, bool allow)> _ranges;

    public Day20()
    {
        _ranges = new LinkedList<(uint begin, uint end, bool allow)>();
        _ranges.AddFirst((0, uint.MaxValue, true));

        foreach (var range in Parse())
        {
            var beginRange = _ranges.First;
            while (true)
            {
                if (range.begin >= beginRange.Value.begin && range.begin <= beginRange.Value.end)
                    break;
                beginRange = beginRange.Next;
            }

            var endRange = beginRange;
            while (true)
            {
                if (range.end >= endRange.Value.begin && range.end <= endRange.Value.end)
                    break;
                endRange = endRange.Next;
            }

            if (beginRange != endRange)
            {
                var current = beginRange.Next;

                while (true)
                {
                    var tmp = current;
                    current = tmp.Next;
                    _ranges.Remove(tmp);
                    if (tmp == endRange)
                        break;
                }
            }

            var insertAfterNode = beginRange;
            if (beginRange.Value.begin != range.begin && beginRange.Value.allow)
            {
                insertAfterNode = _ranges.AddAfter(beginRange, (beginRange.Value.begin, range.begin - 1, true));
            }

            uint begin = beginRange.Value.allow ? range.begin : beginRange.Value.begin;
            uint end = endRange.Value.allow ? range.end : endRange.Value.end;

            insertAfterNode = _ranges.AddAfter(insertAfterNode, (begin, end, false));

            if (endRange.Value.end != range.end && endRange.Value.allow)
            {
                _ranges.AddAfter(insertAfterNode, (range.end + 1, endRange.Value.end, endRange.Value.allow));
            }

            _ranges.Remove(beginRange);
        }
    }

    [Fact]
    public void Part1()
    {
        uint answer = _ranges.First(r => r.allow).begin;
        Assert.Equal(22887907U, answer);
    }

    [Fact]
    public void Part2()
    {
        int answer = _ranges.Where(r => r.allow).Select(r => (int)(r.end - r.begin + 1)).Sum();
        Assert.Equal(109, answer);
    }

    private IEnumerable<(uint begin, uint end)> Parse()
    {
        foreach (string s in File.ReadAllLines("Inputs/Day20.txt"))
        {
            Match match = s_Regex.Match(s);
            yield return (uint.Parse(match.Groups["begin"].Value), uint.Parse(match.Groups["end"].Value));
        }
    }

    Regex s_Regex = new Regex(
        @"^(?'begin'\d+)-(?'end'\d+)$",
        RegexOptions.Compiled);
}
