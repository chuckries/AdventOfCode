namespace AdventOfCode._2020;

public class Day08
{
    private class Machine
    {
        private int _pc;
        private (int arg, Action<int> op)[] _instructions;

        public int Accumulator { get; private set; }

        public Machine(string[] code)
        {
            _instructions = code.Select<string, (int, Action<int>)>(s =>
            {
                int index = s.IndexOf(' ');
                int arg = int.Parse(s.AsSpan(index + 1));

                return s.Substring(0, index) switch
                {
                    "acc" => (arg, Acc),
                    "jmp" => (arg, Jmp),
                    "nop" => (arg, Nop),
                    _ => throw new InvalidOperationException()
                };
            }).ToArray();
        }

        public bool TryRun()
        {
            bool[] used = new bool[_instructions.Length];
            while (true)
            {
                if (_pc == _instructions.Length)
                    return true;

                if (used[_pc])
                    return false;

                used[_pc] = true;
                ref var instr = ref _instructions[_pc++];
                instr.op(instr.arg);
            }
        }

        public void RunToEnd()
        {
            for (int i = 0; i < _instructions.Length; i++)
            {
                var orig = _instructions[i];
                if (orig.op == Acc)
                    continue;

                _instructions[i] = (orig.arg, orig.op == Jmp ? Nop : Jmp);

                if (TryRun())
                    return;

                _instructions[i] = orig;
                _pc = 0;
                Accumulator = 0;
            }
        }

        private void Acc(int arg) => Accumulator += arg;
        private void Jmp(int arg) => _pc += arg - 1;
        private void Nop(int _) { }
    }

    Machine _newMachine;

    public Day08()
    {
        _newMachine = new Machine(File.ReadAllLines("Inputs/Day08.txt"));
    }

    [Fact]
    public void Part1()
    {
        _newMachine.TryRun();

        Assert.Equal(1451, _newMachine.Accumulator);
    }

    [Fact]
    public void Part2()
    {
        _newMachine.RunToEnd();

        Assert.Equal(1160, _newMachine.Accumulator);
    }
}
