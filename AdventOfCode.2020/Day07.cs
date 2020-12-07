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
        private class Node
        {
            public readonly string Color;

            public List<(int count, Node node)> Children { get; } = new();

            public Node(string color)
            {
                Color = color;
            }
        }

        Dictionary<string, Node> _nodes;

        public Day07()
        {
            string[] input = File.ReadAllLines("Inputs/Day07.txt");
            _nodes = new(input.Length);

            foreach (string line in input)
            {
                Match match = s_Regex.Match(line);
                if (!match.Success)
                    continue;

                string left = match.Groups["left"].Value;
                Node parent = GetNode(left);

                foreach (Capture c in match.Groups["right"].Captures)
                {
                    int index = c.Value.IndexOf(' ');
                    int num = int.Parse(c.Value.AsSpan(0, index));
                    string color = c.Value.AsSpan(index + 1).ToString();

                    parent.Children.Add((num, GetNode(color)));
                }
            }
        }

        [Fact]
        public void Part1()
        {
            HashSet<string> colors = new();
            foreach (Node n in _nodes.Values)
            {
                if (n.Color == "shiny gold")
                    continue;

                if (HasPath(n))
                    colors.Add(n.Color);
            }

            Assert.Equal(332, colors.Count);
        }

        [Fact]
        public void Part2()
        {
            int total = 0;
            foreach ((int count, Node child) in GetNode("shiny gold").Children)
            {
                total += CountBags(count, child);
            }

            Assert.Equal(10875, total);
        }

        private bool HasPath(Node source)
        {
            Queue<Node> queue = new();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                if (current.Color == "shiny gold")
                    return true;

                foreach ((int _, Node adj) in current.Children)
                {
                    queue.Enqueue(adj);
                }
            }

            return false;
        }

        private int CountBags(int multi, Node source)
        {
            int total = multi;
            foreach ((int count, Node child) in source.Children)
            {
                total += multi * CountBags(count, child);
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
