using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    class IntCode
    {
        enum Op : long
        {
            Add = 1,
            Mul = 2,
            In = 3,
            Out = 4,
            JumpTrue = 5,
            JumpFalse = 6,
            LessThan = 7,
            Equals = 8,
            Base = 9,
            Halt = 99,
        }

        enum Mode : long
        {
            Pos = 0,
            Imm = 1,
            Relative = 2
        }

        public delegate Task<long> ReadInput();
        public delegate void WriteOutput(long value);

        public long PC { get; set; } = 0;

        public long RelativeBase { get; set; } = 0;

        public bool IsHalt { get; private set; } = false;

        public long this[int index]
        {
            get => ReadMemory(index);
            set => ReadMemory(index) = value;
        }

        public ReadInput Reader { get; set; }
        public WriteOutput Writer { get; set; }

        public IntCode(IEnumerable<int> memory)
            : this(memory, null, null)
        { 
        }

        public IntCode(IEnumerable<int> program, ReadInput reader, WriteOutput writer)
            : this(program.Select(i => (long)i), reader, writer)
        {
        }

        public IntCode(IEnumerable<long> memory, ReadInput reader, WriteOutput writer)
        {
            _memory = memory.ToArray();
            Reader = reader;
            Writer = writer;
        }

        public async Task Step()
        {
            if (!IsHalt)
            {
                (Op op, Mode[] modes) = Decode();

                if (op == Op.Halt)
                    IsHalt = true;
                else if (op == Op.In)
                {
                    long val = await Reader();
                    ReadMemory(ReadPC()) = val;
                }
                else if (op == Op.Out)
                    Writer(ReadArg(modes[0]));
                else if (op == Op.Base)
                    RelativeBase += ReadArg(modes[0]);
                else
                {
                    long val1 = ReadArg(modes[0]);
                    long val2 = ReadArg(modes[1]);

                    if (op == Op.JumpTrue || op == Op.JumpFalse)
                    {
                        if ((op == Op.JumpTrue && val1 != 0) || (op == Op.JumpFalse && val1 == 0))
                            PC = val2;
                    }
                    else
                    {
                        ReadMemory(ReadPC()) = op switch
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

        (Op, Mode[]) Decode()
        {
            long instr = ReadPC();
            
            Op op = (Op)(instr % 100);
            instr /= 100;

            Mode[] modes = new Mode[3];
            modes[0] = (Mode)(instr % 10);
            instr /= 10;
            modes[1] = (Mode)(instr % 10);
            instr /= 10;
            modes[2] = (Mode)(instr % 10);

            return (op, modes);
        }

        private long ReadArg(Mode mode) => mode switch
        {
            Mode.Imm => ReadPC(),
            Mode.Pos => ReadMemory(ReadPC()),
            Mode.Relative => ReadMemory(ReadPC() + RelativeBase),
            _ => throw new InvalidOperationException()
        };

        private long ReadPC()
        {
            return ReadMemory(PC++);
        }

        private ref long ReadMemory(long address)
        {
            if (address >= _memory.Length)
            {
                long[] newMemory = new long[_memory.Length * 2];
                for (int i = 0; i < _memory.Length; i++)
                    newMemory[i] = _memory[i];

                _memory = newMemory;
            }
            return ref _memory[address];
        }

        private long[] _memory;
    }
}
