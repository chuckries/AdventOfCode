using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

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

        public delegate int ReadInput();
        public delegate void WriteOutput(int value);

        public int[] Memory { get; private set; }

        public int PC { get; set; } = 0;

        public bool IsHalt { get; private set; } = false;

        public IntCode(int[] memory, ReadInput reader, WriteOutput writer)
        {
            Memory = memory;
            _reader = reader;
            _writer = writer;
        }

        public void Step()
        {
            if (!IsHalt)
            {
                Decode(Memory[PC++], out Op op, out Mode[] modes);

                if (op == Op.Halt)
                {
                    IsHalt = true;
                }
                else if (op == Op.In)
                {
                    Memory[ReadArg(Mode.Imm)] = _reader();
                }
                else if (op == Op.Out)
                {
                    _writer(ReadArg(modes[0]));
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
                        Memory[ReadArg(Mode.Imm)] = op switch
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

        public void Run()
        {
            while (!IsHalt)
            {
                Step();
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
            Mode.Imm => Memory[PC++],
            Mode.Pos => Memory[Memory[PC++]],
            _ => throw new InvalidOperationException()
        };

        private ReadInput _reader;
        private WriteOutput _writer;
    }
}
