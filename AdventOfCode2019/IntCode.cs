using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019
{
    class IntCode
    {
        enum Op : int
        {
            Add = 1,
            Mul = 2,
            In = 3,
            Out = 4,
            JumpTrue = 5,
            JumpFalse = 6,
            LessThan = 7,
            Equals = 8,
            Halt = 99,
        }

        enum Mode : int
        {
            Pos = 0,
            Imm = 1
        }

        public delegate Task<int> ReadInput();
        public delegate void WriteOutput(int value);

        public int PC { get; set; } = 0;

        public bool IsHalt { get; private set; } = false;

        public int this[int index]
        {
            get => _memory[index];
            set => _memory[index] = value;
        }

        public ReadInput Reader { get; set; }
        public WriteOutput Writer { get; set; }

        public IntCode(IEnumerable<int> memory)
            : this(memory, null, null)
        { 
        }

        public IntCode(IEnumerable<int> memory, ReadInput reader, WriteOutput writer)
        {
            _memory = memory.ToArray();
            Reader = reader;
            Writer = writer;
        }

        public async Task Step()
        {
            if (!IsHalt)
            {
                Decode(_memory[PC++], out Op op, out Mode[] modes);

                if (op == Op.Halt)
                {
                    IsHalt = true;
                }
                else if (op == Op.In)
                {
                    _memory[ReadArg(Mode.Imm)] = await Reader();
                }
                else if (op == Op.Out)
                {
                    Writer(ReadArg(modes[0]));
                }
                else
                {
                    int val1 = ReadArg(modes[0]);
                    int val2 = ReadArg(modes[1]);

                    if (op == Op.JumpTrue || op == Op.JumpFalse)
                    {
                        if ((op == Op.JumpTrue && val1 != 0) || (op == Op.JumpFalse && val1 == 0))
                            PC = val2;
                    }
                    else
                    {
                        _memory[ReadArg(Mode.Imm)] = op switch
                        {
                            Op.Add => val1 + val2,
                            Op.Mul => val1 * val2,
                            Op.LessThan => val1 < val2 ? 1 : 0,
                            Op.Equals => val1 == val2 ? 1 : 0,
                            _ => throw new InvalidOperationException()
                        };
                    }
                }
            }
        }

        public async Task Run()
        {
            while (!IsHalt)
            {
                await Step();
            }
        }

        private void Decode(int instr, out Op op, out Mode[] modes)
        {
            op = (Op)(instr % 100);
            instr /= 100;

            modes = new Mode[3];
            modes[0] = (Mode)(instr % 10);
            instr /= 10;
            modes[1] = (Mode)(instr % 10);
            instr /= 10;
            modes[2] = (Mode)(instr % 10);
        }

        private int ReadArg(Mode mode) => mode switch
        {
            Mode.Imm => _memory[PC++],
            Mode.Pos => _memory[_memory[PC++]],
            _ => throw new InvalidOperationException()
        };

        private int[] _memory;
    }
}
