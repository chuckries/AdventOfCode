using System.Diagnostics;

namespace AdventOfCode._2019;

public class Day06
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
            int total = Children.Count;
            foreach (Node c in Children)
                total += c.TotalChildren();

            return total;
        }
    }

    Node _root;
    Dictionary<string, Node> _nodes = new Dictionary<string, Node>();

    public Day06()
    {
        (string parent, string child)[] inputs = File.ReadAllLines("Inputs/Day06.txt")
            .Select(s => s.Split(')'))
            .Select(a => (a[0], a[1]))
            .ToArray();

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
        int total = 0;
        foreach (Node n in _nodes.Values)
            total += n.TotalChildren();

        Assert.Equal(417916, total);
    }

    [Fact]
    public void Part2()
    {
        Node source = GetNode("YOU").Parent;
        Node target = GetNode("SAN").Parent;

        Dictionary<Node, int> distances = new Dictionary<Node, int>();

        Node current = source;
        int distance = 0;
        while (current != null)
        {
            distances.Add(current, distance++);
            current = current.Parent;
        }

        distance = 0;
        current = target;
        while (current != null)
        {
            if (distances.TryGetValue(current, out int ancestorDistance))
            {
                distance += ancestorDistance;
                break;
            }

            current = current.Parent;
            distance++;
        }

        int answer = distance;
        Assert.Equal(523, answer);
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
