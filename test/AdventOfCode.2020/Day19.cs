using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode._2020;

public class Day19
{
    [DebuggerDisplay("{Index}")]
    private class Node
    {
        public readonly int Index;

        private List<List<Node>>? _childGroups;
        char? _terminal;

        public Node(int index)
        {
            Index = index;
        }

        public void SetTerminal(char c)
        {
            _terminal = c;
        }

        [MemberNotNull(nameof(_childGroups))]
        public void SetChildGroups(List<List<Node>> childGroups)
        {
            _childGroups = childGroups;
        }

        public bool TryMatch(string str, int index, out int matchedLength)
        {
            matchedLength = 0;

            if (_terminal.HasValue)
            {
                if (_terminal.Value == str[index])
                {
                    matchedLength = 1;
                    return true;
                }
                return false;
            }

            if (_childGroups is null)
                return false;

            foreach (List<Node> group in _childGroups)
            {
                int currentIndex = index;
                bool fullMatch = true;
                foreach (Node child in group)
                {
                    if (!child.TryMatch(str, currentIndex, out int childLength))
                    {
                        fullMatch = false;
                        break;
                    }
                    currentIndex += childLength;
                }
                if (fullMatch)
                {
                    matchedLength = currentIndex - index;
                    return true;
                }
            }

            return false;
        }
    }

    private List<Node> _nodes = new List<Node>();
    private string[] _received;

    public Day19()
    {
        string[] input = File.ReadAllLines("Inputs/Day19.txt");

        int i = 0;
        string line;
        while (!string.IsNullOrEmpty(line = input[i++]))
        {
            Match match = s_Regex.Match(line);
            int index = int.Parse(match.Groups["index"].Value);
            Node n = GetNode(index);

            if (match.Groups["term"].Success)
            {
                n.SetTerminal(match.Groups["term"].Value[0]);
            }
            else
            {
                List<List<Node>> children = new List<List<Node>>();
                List<Node> current = new List<Node>();
                children.Add(current);

                foreach (Capture c in match.Groups["left"].Captures)
                    current.Add(GetNode(int.Parse(c.Value)));

                if (match.Groups["right"].Success)
                {
                    current = new List<Node>();
                    children.Add(current);

                    foreach (Capture c in match.Groups["right"].Captures)
                        current.Add(GetNode(int.Parse(c.Value)));
                }

                n.SetChildGroups(children);
            }
        }

        _received = input[i..];
    }

    [Fact]
    public void Part1()
    {
        Node zero = GetNode(0);
        int answer = _received.Count(s => zero.TryMatch(s, 0, out int length) && length == s.Length);
        Assert.Equal(269, answer);
    }

    [Fact]
    public void Part2()
    {
        Node n42 = GetNode(42);
        Node n31 = GetNode(31);

        static bool TryMatch(string str, int index, int count42, int count31, Node n42, Node n31)
        {
            if (index == str.Length)
                return count31 > 0 && count42 > count31;

            if (count31 == 0)
                if (n42.TryMatch(str, index, out int matchedLength))
                    if (TryMatch(str, index + matchedLength, count42 + 1, count31, n42, n31))
                        return true;

            if (count42 > 1)
                if (n31.TryMatch(str, index, out int matchedLength))
                    if (TryMatch(str, index + matchedLength, count42, count31 + 1, n42, n31))
                        return true;


            return false;
        }

        int answer = _received.Count(s => TryMatch(s, 0, 0, 0, n42, n31));
        Assert.Equal(403, answer);
    }


    private Node GetNode(int index)
    {
        for (int i = _nodes.Count; i <= index; i++)
            _nodes.Add(new Node(i));

        return _nodes[index];
    }

    private static Regex s_Regex = new Regex(
        @"^(?'index'\d+): ((""(?'term'[ab])"")|(?'left'\d+)( (?'left'\d+))*( \| (?'right'\d+)( (?'right'\d+))*)?)$",
        RegexOptions.Compiled);
}
