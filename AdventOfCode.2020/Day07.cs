using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xunit;

using AdventOfCode.Common;

namespace AdventOfCode._2020
{
    public class Day07
    {
        private class Edge
        {
            public readonly int Count;
            public readonly Node Node;

            public Edge(int count, Node node)
            {
                Count = count;
                Node = node;
            }
        }

        private class Node
        {
            public readonly string Color;

            public List<Edge> Children { get; } = new();

            public Node(string color)
            {
                Color = color;
            }
        }

        private const string Target = "shiny gold";
        private Dictionary<string, Node> _nodes;

        public Day07()
        {
            string[] input = File.ReadAllLines("Inputs/Day07.txt");
            _nodes = new(input.Length);

            foreach (string line in input)
            {
                Match match = s_Regex.Match(line);
                if (!match.Success)
                    continue;

                string color = match.Groups["left"].Value;
                Node parent = GetNode(color);

                foreach (Capture c in match.Groups["right"].Captures)
                {
                    int index = c.Value.IndexOf(' ');
                    int num = int.Parse(c.Value.AsSpan(0, index));
                    color = c.Value.AsSpan(index + 1).ToString();

                    parent.Children.Add(new Edge(num, GetNode(color)));
                }
            }
        }

        [Fact]
        public void Part1()
        {
            int answser = _nodes.Values.Count(HasPath) - 1;

            Assert.Equal(332, answser);
        }

        [Fact]
        public void Part2()
        {
            int total = 0;
            foreach (Edge e in GetNode(Target).Children)
            {
                total += CountBags(e.Count, e.Node);
            }

            Assert.Equal(10875, total);
        }

        private bool HasPath(Node n)
        {
            if (n.Color == Target)
                return true;

            if (n.Children.Any(e => HasPath(e.Node)))
                return true;

            return false;
        }

        private int CountBags(int multi, Node source)
        {
            int total = multi;
            foreach (Edge e in source.Children)
            {
                total += multi * CountBags(e.Count, e.Node);
            }

            return total;
        }

        private Node GetNode(string color)
        {
            if (!_nodes.TryGetValue(color, out Node n))
            {
                n = new Node(color);
                _nodes.Add(color, n);
            }
            return n;
        }

        Regex s_Regex = new Regex(
            @"^(?'left'\w+ \w+) bags contain ((?'right'\d+ \w+ \w+) bags?(, )?)+\.$",
            RegexOptions.Compiled);
    }
}
