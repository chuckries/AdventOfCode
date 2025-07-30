using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode._2015;

public class Day23
{
    private class Machine
    {
        private int[] _regs = new int[2];
        private int _pc;
        private Action[] _program;

        public int A
        {
            get => _regs[0];
            set => _regs[0] = value;
        }

        public int B => _regs[1];

        public Machine(string[] input)
        {
            Parse(input);
        }

        public void Run()
        {
            while (_pc >= 0 && _pc < _program.Length)
                _program[_pc]();
        }

        [MemberNotNull(nameof(_program))]
        private void Parse(string[] input)
        {
            _program = input.Select<string, Action>(s =>
            {
                string[] tokens = s.Split(' ');
                string op = tokens[0];
                int regIndex;
                int offset;

                if (op == "hlf")
                {
                    regIndex = ParseArg(tokens[1]);
                    return () => Hlf(regIndex);
                }
                else if (op == "tpl")
                {
                    regIndex = ParseArg(tokens[1]);
                    return () => Tpl(regIndex);
                }
                else if (op == "inc")
                {
                    regIndex = ParseArg(tokens[1]);
                    return () => Inc(regIndex);
                }
                else if (op == "jmp")
                {
                    offset = ParseOffset(tokens[1]);
                    return () => Jmp(offset);
                }
                else if (op == "jie")
                {
                    regIndex = ParseArg(tokens[1]);
                    offset = ParseOffset(tokens[2]);
                    return () => Jie(regIndex, offset);
                }
                else if (op == "jio")
                {
                    regIndex = ParseArg(tokens[1]);
                    offset = ParseOffset(tokens[2]);
                    return () => Jio(regIndex, offset);
                }
                else throw new InvalidOperationException();

            }).ToArray();

            int ParseArg(string arg) => arg.Trim(',') switch
            {
                "a" => 0,
                "b" => 1,
                _ => throw new InvalidOperationException()
            };

            int ParseOffset(string offest) => int.Parse(offest.Trim(',', '+'));
        }

        private void Hlf(int regIndex)
        {
            ref int reg = ref GetReg(regIndex);
            reg /= 2;
            _pc++;
        }

        private void Tpl(int regIndex)
        {
            ref int reg = ref GetReg(regIndex);
            reg *= 3;
            _pc++;
        }

        private void Inc(int regIndex)
        {
            ref int reg = ref GetReg(regIndex);
            reg++;
            _pc++;
        }

        private void Jmp(int offset)
        {
            _pc += offset;
        }

        private void Jie(int regIndex, int offset)
        {
            int reg = GetReg(regIndex);
            if (reg % 2 == 0)
                _pc += offset;
            else
                _pc++;
        }

        private void Jio(int regIndex, int offset)
        {
            int reg = GetReg(regIndex);
            if (reg == 1)
                _pc += offset;
            else
                _pc++;
        }

        private ref int GetReg(int regIndex)
        {
            return ref _regs[regIndex];
        }
    }

    Machine _machine;

    public Day23()
    {
        _machine = new Machine(File.ReadAllLines("Inputs/Day23.txt"));
    }

    [Fact]
    public void Part1()
    {
        _machine.Run();
        int answer = _machine.B;
        Assert.Equal(307, answer);
    }

    [Fact]
    public void Part2()
    {
        _machine.A = 1;
        _machine.Run();
        int answer = _machine.B;
        Assert.Equal(160, answer);
    }
}
