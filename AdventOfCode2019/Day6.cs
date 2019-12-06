using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2019
{
    public class Day6
    {
        [DebuggerDisplay("{Name,nq}")]
        class Node
        {
            public readonly string Name;
            public Node Parent { get; set; }
            public List<Node> Children { get; } = new List<Node>();

            public Node(string name)
            {
                Name = name;
            }

            public int TotalChildren()
            {
                return Children.Count + Children.Select(c => c.TotalChildren()).Sum();
            }
        }

        Node _root;
        Dictionary<string, Node> _nodes = new Dictionary<string, Node>();

        public Day6()
        {
            (string parent, string child)[] inputs = File.ReadAllLines("Inputs/Day6.txt").Select(s => s.Split(')')).Select(a => (a[0], a[1])).ToArray();
            foreach (var input in inputs)
            {
                Node parent = GetNode(input.parent);
                if (parent.Name.Equals("COM"))
                    _root = parent;

                Node child = GetNode(input.child);

                child.Parent = parent;
                parent.Children.Add(child);
            }
        }

        [Fact]
        public void Part1()
        {
            int answer = _nodes.Values.Select(n => n.TotalChildren()).Sum();
            Assert.Equal(417916, answer);
        }

        [Fact]
        public void Part2()
        {
            Node source = GetNode("YOU").Parent;
            Node target = GetNode("SAN").Parent;

            Dictionary<Node, int> distances = new Dictionary<Node, int>() { { source, 0 } };
            Queue<Node> toVisit = new Queue<Node>(new[] { source });

            void Add(Node candidate, int distance)
            {
                if (candidate != null && !distances.ContainsKey(candidate))
                {
                    distances.Add(candidate, distance);
                    toVisit.Enqueue(candidate);
                }
            }

            while (!distances.ContainsKey(target))
            {
                Node current = toVisit.Dequeue();
                int distance = distances[current] + 1;
                Add(current.Parent, distance);
                foreach (Node child in current.Children) Add(child, distance);
            }

            Assert.Equal(523, distances[target]);
        }

        private Node GetNode(string name)
        {
            if (!_nodes.TryGetValue(name, out Node node))
            {
                node = new Node(name);
                _nodes.Add(name, node);
            }
            return node;
        }
    }
}
