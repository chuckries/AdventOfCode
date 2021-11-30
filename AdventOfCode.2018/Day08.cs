using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2018
{
    public class Day08
    {
        private class Node
        {
            public List<Node> Children { get; } = new List<Node>();
            public List<int> Metadata { get; } = new List<int>();

            public long GetValue()
            {
                if (Children.Count == 0)
                    return Metadata.Sum();

                return Metadata
                    .Where(m => m <= Children.Count)
                    .Select(m => Children[m - 1].GetValue())
                    .Sum();
            }
        }

        Node _root;

        public Day08()
        {
            int[] input = File.ReadAllText("Inputs/Day08.txt").Split().Select(int.Parse).ToArray();
            int index = 0;
            _root = Recurse(ref index, input);

            static Node Recurse(ref int index, int[] tree)
            {
                Node n = new Node();

                int countChildren = tree[index++];
                int countMetadata = tree[index++];

                for (int i = 0; i < countChildren; i++)
                    n.Children.Add(Recurse(ref index, tree));

                for (int i = 0; i < countMetadata; i++)
                    n.Metadata.Add(tree[index++]);

                return n;
            }
        }

        [Fact]
        public void Part1()
        {
            long answer = Recurse(_root);
            Assert.Equal(43825, answer);

            static long Recurse(Node n) =>
                n.Children.Select(Recurse).Sum() + n.Metadata.Sum();
        }

        [Fact]
        public void Part2()
        {
            long answer = _root.GetValue();
            Assert.Equal(19276, answer);
        }
    }
}
