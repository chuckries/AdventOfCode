using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day7
    {
        private class DependencyGraph
        {
            private abstract class Arg
            {
                public abstract ushort Resolve();
            }

            [DebuggerDisplay("{_value}")]
            private class ConstantArg : Arg
            {
                private readonly ushort _value;

                public ConstantArg(ushort value)
                {
                    _value = value;
                }

                public override ushort Resolve() => _value;
            }

            [DebuggerDisplay("{_name,nq}")]
            private class NodeArg : Arg
            {
                public readonly string _name;
                private readonly DependencyGraph _graph;

                public NodeArg(string name, DependencyGraph graph)
                {
                    _name = name;
                    _graph = graph;
                }

                public override ushort Resolve() => _graph.GetNode(_name).Resolve();
            }

            private abstract class Node
            {
                public readonly string Name;
                private Lazy<ushort> _lazyValue;

                protected Node(string name)
                {
                    Name = name;
                    _lazyValue = new Lazy<ushort>(ResolveCore);
                }

                public ushort Resolve()
                {
                    return _lazyValue.Value;
                }

                protected abstract ushort ResolveCore();
            }

            [DebuggerDisplay("{_value} -> {Name,nq}")]
            private class ConstantNode : Node
            {
                private readonly ushort _value;

                public ConstantNode(string name, ushort value)
                    : base(name)
                {
                    _value = value;
                }

                protected override ushort ResolveCore()
                {
                    return _value;
                }
            }

            [DebuggerDisplay("{_op.Method.Name,nq} {_dependency,nq} -> {Name,nq}")]
            private class UnaryNode : Node
            {
                private readonly Arg _dependency;
                private readonly Func<ushort, ushort> _op;

                public UnaryNode(string name, Arg dependency, Func<ushort, ushort> op)
                    : base(name)
                {
                    _dependency = dependency;
                    _op = op;
                }

                protected override ushort ResolveCore()
                {
                    ushort arg1 = _dependency.Resolve();
                    ushort result = _op(arg1);
                    return result;
                }
            }

            [DebuggerDisplay("{_dependency1,nq} {_op.Method.Name,nq} {_dependency2,nq} -> {Name,nq}")]
            private class BinaryNode : Node
            {
                private readonly Arg _dependency1;
                private readonly Arg _dependency2;
                private readonly Func<ushort, ushort, ushort> _op;

                public BinaryNode(string name, Arg dependency1, Arg dependency2, Func<ushort, ushort, ushort> op)
                    : base(name)
                {
                    _dependency1 = dependency1;
                    _dependency2 = dependency2;
                    _op = op;
                }

                protected override ushort ResolveCore()
                {
                    ushort arg1 = _dependency1.Resolve();
                    ushort arg2 = _dependency2.Resolve();
                    ushort result = _op(arg1, arg2);
                    return result;
                }
            }

            private Dictionary<string, Node> _nodes;

            public DependencyGraph(string[] input)
            {
                _nodes = new Dictionary<string, Node>();
                foreach (string s in input)
                {
                    string[] tokens = s.Split();
                    string nodeName = tokens[^1];
                    Node node = null;

                    if (tokens[0] == "NOT")
                    {
                        node = new UnaryNode(nodeName, new NodeArg(tokens[1], this), Not);
                    }
                    else if (tokens[1] == "->")
                    {
                        if (ushort.TryParse(tokens[0], out ushort constVal))
                        {
                            node = new ConstantNode(nodeName, constVal);
                        }
                        else
                        {
                            node = new UnaryNode(nodeName, new NodeArg(tokens[0], this), Nop);
                        }
                    }
                    else
                    {
                        ushort constVal;
                        Arg arg1;
                        Arg arg2;
                        Func<ushort, ushort, ushort> op;

                        if (ushort.TryParse(tokens[0], out constVal))
                            arg1 = new ConstantArg(constVal);
                        else
                            arg1 = new NodeArg(tokens[0], this);

                        if (ushort.TryParse(tokens[2], out constVal))
                            arg2 = new ConstantArg(constVal);
                        else
                            arg2 = new NodeArg(tokens[2], this);

                        op = tokens[1] switch
                        {
                            "AND" => And,
                            "OR" => Or,
                            "LSHIFT" => LShift,
                            "RSHIFT" => RShift,
                            _ => throw new InvalidOperationException()
                        };

                        node = new BinaryNode(nodeName, arg1, arg2, op);
                    }

                    _nodes.Add(node.Name, node);
                }
            }

            public ushort Resolve(string name)
            {
                return GetNode(name).Resolve();
            }

            private Node GetNode(string name)
            {
                return _nodes[name];
            }

            private static ushort Nop(ushort val) => val;
            private static ushort And(ushort val1, ushort val2) => (ushort)(val1 & val2);
            private static ushort Or(ushort val1, ushort val2) => (ushort)(val1 | val2);
            private static ushort Not(ushort val) => (ushort)~val;
            private static ushort LShift(ushort val, ushort amount) => (ushort)((val << amount) & 0xFFFF);
            private static ushort RShift(ushort val, ushort amount) => (ushort)((val >> amount) & 0xFFFF);
        }

        [Fact]
        public void Part1()
        {
            DependencyGraph graph = new DependencyGraph(File.ReadAllLines("Inputs/Day7.txt").ToArray());
            ushort answer = graph.Resolve("a");

            Assert.Equal(16076, answer);
        }

        [Fact]
        public void Part2()
        {
            DependencyGraph graph = new DependencyGraph(File.ReadAllLines("Inputs/Day7.txt").Select(s =>
            {
                if (s.EndsWith("-> b"))
                    s = "16076 -> b";

                return s;
            }).ToArray());

            ushort answer = graph.Resolve("a");
            Assert.Equal(2797, answer);
        }
    }
}
