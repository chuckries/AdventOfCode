using System.Diagnostics;

namespace AdventOfCode._2021;

public class Day18
{
    private abstract class Node
    {
        private ParentNode? _parent;

        public ref ParentNode? Parent => ref _parent;

        public int Depth()
        {
            Node? current = this;
            int depth = 0;
            while (current != null)
            {
                depth++;
                current = current.Parent;
            }

            return depth;
        }

        public abstract int Magniutde();

        public abstract Node Copy(ParentNode? parent);
    }

    [DebuggerDisplay("{ToString(),nq}")]
    private class ParentNode : Node
    {
        private Node? _left;
        private Node? _right;

        public ref Node? Left => ref _left;
        public ref Node? Right => ref _right;

        public override int Magniutde()
        {
            return 3 * Left!.Magniutde() + 2 * Right!.Magniutde();
        }

        public override ParentNode Copy(ParentNode? parent)
        {
            ParentNode copy = new ParentNode()
            {
                Parent = parent,
            };

            copy._left = _left!.Copy(copy);
            copy._right = _right!.Copy(copy);

            return copy;
        }

        public override string ToString()
        {
            return $"[{_left?.ToString()},{_right?.ToString()}]";
        }
    }

    private class ValueNode : Node
    {
        public int Value { get; set; }

        public override int Magniutde()
        {
            return Value;
        }

        public override Node Copy(ParentNode? parent)
        {
            return new ValueNode
            {
                Parent = parent,
                Value = Value
            };
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DebuggerDisplay("{_root.ToString(),nq}")]
    private class Number
    {
        private ParentNode _root;

        private Number(ParentNode root)
        {
            _root = root;
        }

        public static Number Parse(string input)
        {
            Stack<ParentNode> stack = new();
            Node? root = null;
            ref Node? current = ref root;
            foreach (char c in input)
            {
                if (c is '[')
                {
                    ParentNode parent = new ParentNode
                    {
                        Parent = stack.Count > 0 ? stack.Peek() : null
                    };
                    current = parent;
                    stack.Push(parent);
                    current = ref stack.Peek().Left;
                }
                else if (c is ']')
                {
                    root = stack.Pop();
                }
                else if (c is ',')
                {
                    current = ref stack.Peek().Right;
                }
                else
                {
                    int val = c - '0';
                    current = new ValueNode
                    {
                        Value = val,
                        Parent = stack.Peek()
                    };
                }
            }

            return new Number((ParentNode)root!);
        }

        public int Magnitude()
        {
            return _root.Magniutde();
        }

        public Number Add(Number other)
        {
            ParentNode newRoot = new ParentNode();

            newRoot.Left = _root.Copy(newRoot);
            newRoot.Right = other._root.Copy(newRoot);
            return new Number(newRoot);
        }

        public void Reduce()
        {
            while (true)
            {
                if (TryExplode())
                    continue;

                if (TrySplit())
                    continue;

                break;
            }
        }

        bool TryExplode()
        {
            Stack<ParentNode> stack = new();
            PushLeft(_root);

            while (stack.Count > 0)
            {
                if (stack.Peek().Depth() >= 5)
                {
                    Explode(stack.Peek());
                    return true;
                }

                PushLeft(stack.Pop().Right!);
            }

            return false;

            void PushLeft(Node n)
            {
                while (n is not ValueNode)
                {
                    ParentNode pn = (ParentNode)n!;
                    stack.Push(pn);
                    n = pn.Left!;
                }
            }
        }

        bool TrySplit()
        {
            Stack<ParentNode> stack = new();
            PushLeft(_root);

            while (stack.Count > 0)
            {
                ValueNode? valueNode;
                valueNode = stack.Peek().Left as ValueNode;
                if (valueNode is not null && valueNode.Value >= 10)
                {
                    Split(valueNode);
                    return true;
                }

                valueNode = stack.Peek().Right as ValueNode;
                if (valueNode is not null && valueNode.Value >= 10)
                {
                    Split(valueNode);
                    return true;
                }

                PushLeft(stack.Pop().Right!);
            }

            return false;

            void PushLeft(Node n)
            {
                while (n is not ValueNode)
                {
                    ParentNode pn = (ParentNode)n!;
                    stack.Push(pn);
                    n = pn.Left!;
                }
            }
        }

        private void Explode(ParentNode node)
        {
            ValueNode x = (ValueNode)node.Left!;
            ValueNode y = (ValueNode)node.Right!;

            ValueNode? left = GetLeftOf(x);
            ValueNode? right = GetRightOf(y);

            if (left is not null) left.Value += x.Value;
            if (right is not null) right.Value += y.Value;

            ValueNode newNode = new ValueNode { Value = 0, Parent = node.Parent };
            if (node.Parent is null)
                throw new InvalidOperationException();
            else if (node == node.Parent.Left)
                node.Parent.Left = newNode;
            else
                node.Parent.Right = newNode;
        }

        private void Split(ValueNode node)
        {
            (int quotient, int remainder) = DivRem(node.Value, 2);
            int x = quotient;
            int y = quotient + remainder;

            ParentNode newNode = new ParentNode { Parent = node.Parent };
            newNode.Left = new ValueNode { Value = x, Parent = newNode };
            newNode.Right = new ValueNode { Value = y, Parent = newNode };

            if (node.Parent is null)
                _root = newNode;
            else if (node == node.Parent.Left)
                node.Parent.Left = newNode;
            else
                node.Parent.Right = newNode;
        }

        private ValueNode? GetLeftOf(ValueNode node)
        {
            ParentNode? current = node.Parent;
            if (current is null)
                return null;

            if (node != current.Right)
            {
                while (current is not null && current != current.Parent?.Right)
                    current = current.Parent;

                current = current?.Parent;
            }

            if (current is null)
                return null;

            if (current.Left is ValueNode valueNode)
                return valueNode;

            current = (ParentNode?)current.Left;
            while (current is not null)
            {
                if (current.Right is ValueNode rightValue)
                    return rightValue;

                current = (ParentNode?)current.Right;
            }

            return null;
        }

        private ValueNode? GetRightOf(ValueNode node)
        {
            ParentNode? current = node.Parent;
            if (current is null)
                return null;

            if (node != current.Left)
            {
                while (current is not null && current != current.Parent?.Left)
                    current = current.Parent;

                current = current?.Parent;
            }

            if (current is null)
                return null;

            if (current.Right is ValueNode valueNode)
                return valueNode;

            current = (ParentNode?)current.Right;
            while (current is not null)
            {
                if (current.Left is ValueNode leftValue)
                    return leftValue;

                current = (ParentNode?)current.Left;
            }

            return null;
        }
    }

    private readonly Number[] _numbers;

    public Day18()
    {
        _numbers = File.ReadAllLines("Inputs/Day18.txt").Select(Number.Parse).ToArray();
    }

    [Fact]
    public void Part1()
    {
        Number current = _numbers[0];
        for (int i = 1; i < _numbers.Length; i++)
        {
            current = current.Add(_numbers[i]);
            current.Reduce();
        }

        int answer = current.Magnitude();
        Assert.Equal(3359, answer);
    }

    [Fact]
    public void Part2()
    {
        int max = int.MinValue;
        for (int i = 0; i < _numbers.Length; i++)
            for (int j = 0; j < _numbers.Length; j++)
                if (i != j)
                {
                    Number sum = _numbers[i].Add(_numbers[j]);
                    sum.Reduce();
                    int cand = sum.Magnitude();
                    if (cand > max)
                        max = cand;
                }

        Assert.Equal(4616, max);
    }
}
