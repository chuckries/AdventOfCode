using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day23
    {
        private class Machine
        {
            private interface IArg
            {
                bool CanWrite { get; }
                int Read();
                void Write(int value);
            }

            private class ConstantArg : IArg
            {
                private readonly int _value;

                public bool CanWrite => false;

                public ConstantArg(int value)
                {
                    _value = value;
                }

                public int Read() => _value;

                public void Write(int _) => throw new InvalidOperationException();
            }

            private class RegArg : IArg
            {
                private readonly int _regIndex;
                private readonly Machine _machine;

                public bool CanWrite => true;

                public RegArg(int regIndex, Machine machine)
                {
                    _regIndex = regIndex;
                    _machine = machine;
                }

                public int Read() => _machine._regs[_regIndex];

                public void Write(int value) => _machine._regs[_regIndex] = value;
            }

            private interface IInstruction
            {
                void Run();
                IInstruction Toggle();
            }

            private class CpyInstruction : IInstruction
            {
                private readonly IArg _x;
                private readonly IArg _y;
                private readonly Machine _machine;

                public CpyInstruction(IArg x, IArg y, Machine machine)
                {
                    _x = x;
                    _y = y;
                    _machine = machine;
                }

                public void Run()
                {
                    if (_y.CanWrite)
                        _y.Write(_x.Read());
                }

                public IInstruction Toggle()
                {
                    return new JnzInstruction(_x, _y, _machine);
                }
            }

            private class IncInstruction : IInstruction
            {
                private readonly IArg _x;

                public IncInstruction(IArg x)
                {
                    _x = x;
                }

                public void Run()
                {
                    if (_x.CanWrite)
                        _x.Write(_x.Read() + 1);
                }

                public IInstruction Toggle()
                {
                    return new DecInstruction(_x);
                }
            }

            private class DecInstruction : IInstruction
            {
                private readonly IArg _x;

                public DecInstruction(IArg x)
                {
                    _x = x;
                }

                public void Run()
                {
                    if (_x.CanWrite)
                        _x.Write(_x.Read() - 1);
                }

                public IInstruction Toggle()
                {
                    return new IncInstruction(_x);
                }
            }

            private class JnzInstruction : IInstruction
            {
                private readonly IArg _x;
                private readonly IArg _y;
                private readonly Machine _machine;

                public JnzInstruction(IArg x, IArg y, Machine machine)
                {
                    _x = x;
                    _y = y;
                    _machine = machine;
                }

                public void Run()
                {
                    if (_x.Read() != 0)
                        _machine._pc += _y.Read() - 1;

                }

                public IInstruction Toggle()
                {
                    return new CpyInstruction(_x, _y, _machine);
                }
            }

            private class TglInstruction : IInstruction
            {
                private readonly IArg _x;
                private readonly Machine _machine;

                public TglInstruction(IArg x, Machine machine)
                {
                    _x = x;
                    _machine = machine;
                }

                public void Run()
                {
                    int index = _x.Read() + _machine._pc;
                    if (index >= 0 && index < _machine._instructions.Length)
                        _machine._instructions[index] = _machine._instructions[index].Toggle();
                }

                public IInstruction Toggle()
                {
                    return new IncInstruction(_x);
                }
            }

            private int _pc;
            private int[] _regs;
            private IInstruction[] _instructions;

            public int A
            {
                get => _regs[0];
                set => _regs[0] = value;
            }

            public Machine(string[] input)
            {
                _pc = 0;
                _regs = new int[4];
                _instructions = new IInstruction[input.Length];

                for (int i = 0; i < input.Length; i++)
                {
                    string[] t = input[i].Split(' ');

                    _instructions[i] = t[0] switch
                    {
                        "cpy" => new CpyInstruction(ParseArg(t[1]), ParseArg(t[2]), this),
                        "inc" => new IncInstruction(ParseArg(t[1])),
                        "dec" => new DecInstruction(ParseArg(t[1])),
                        "jnz" => new JnzInstruction(ParseArg(t[1]), ParseArg(t[2]), this),
                        "tgl" => new TglInstruction(ParseArg(t[1]), this),
                        _ => throw new InvalidOperationException()
                    };
                }
            }

            public void Run()
            {
                while (_pc < _instructions.Length)
                {
                    _instructions[_pc].Run();
                    _pc++;
                }
            }

            private IArg ParseArg(string s)
            {
                if (int.TryParse(s, out int constant))
                    return new ConstantArg(constant);
                else
                    return new RegArg(RegIndex(s[0]), this);
            }

            private static int RegIndex(char c) => c - 'a';
        }

        Machine _machine;

        public Day23()
        {
            _machine = new Machine(File.ReadAllLines("Inputs/Day23.txt"));
        }

        [Fact]
        public void Part1()
        {
            _machine.A = 7;
            _machine.Run();
            int answer = _machine.A;
            Assert.Equal(10365, answer);
        }

        [Fact(Skip = "Slow...")]
        public void Part2()
        {
            _machine.A = 12;
            _machine.Run();
            int answer = _machine.A;
            Assert.Equal(479006925, answer);
        }

        [Fact]
        public void Example()
        {
            Machine machine = new Machine(new[]
            {
                "cpy 2 a",
                "tgl a",
                "tgl a",
                "tgl a",
                "cpy 1 a",
                "dec a",
                "dec a"
            });

            machine.Run();
            int answer = machine.A;
            Assert.Equal(3, answer);
        }
    }
}
