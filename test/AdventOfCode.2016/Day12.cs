namespace AdventOfCode._2016;

public class Day12
{
    private class Machine
    {
        private Action[] _code;
        private int[] _regs = new int[4];
        private int _pc;

        public int A => _regs[0];
        public int C
        {
            set => _regs[2] = value;
        }

        public Machine(string[] code)
        {
            _code = code.Select<string, Action>(s =>
            {
                string[] t = s.Split(' ');
                bool parsedX = int.TryParse(t[1], out int x);
                int y = 0;
                bool parsedY = t.Length > 2 ? int.TryParse(t[2], out y) : false;
                char regX = t[1][0];
                char regY = t.Length > 2 ? t[2][0] : 'z';

                return t[0] switch
                {
                    "cpy" when parsedX => () => CpyConst(x, RegIndex(regY)),
                    "cpy" when !parsedX => () => CpyReg(RegIndex(regX), RegIndex(regY)),
                    "inc" => () => Inc(RegIndex(regX)),
                    "dec" => () => Dec(RegIndex(regX)),
                    "jnz" when parsedX => () => JnzConst(x, y),
                    "jnz" when !parsedX => () => JnzReg(RegIndex(regX), y),
                    _ => throw new InvalidOperationException()
                };
            }).ToArray();
        }

        public void Run()
        {
            while (_pc >= 0 && _pc < _code.Length)
            {
                _code[_pc]();
                _pc++;
            }
        }

        private void CpyConst(int x, int y) => _regs[y] = x;
        private void CpyReg(int x, int y) => _regs[y] = _regs[x];
        private void Inc(int x) => _regs[x]++;
        private void Dec(int x) => _regs[x]--;
        private void JnzConst(int x, int y) => _pc += (x != 0) ? (y - 1) : 0;
        private void JnzReg(int x, int y) => JnzConst(_regs[x], y);

        private static int RegIndex(char reg) => reg - 'a';
    }

    Machine _vm;

    public Day12()
    {
        _vm = new Machine(File.ReadAllLines("Inputs/Day12.txt"));
    }

    [Fact]
    public void Part1()
    {
        _vm.Run();
        Assert.Equal(317993, _vm.A);
    }

    [Fact]
    public void Part2()
    {
        _vm.C = 1;
        _vm.Run();
        Assert.Equal(9227647, _vm.A);
    }
}
