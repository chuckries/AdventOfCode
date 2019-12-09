using System;
using System.Buffers;
using System.Collections.Generic;
using System.Dynamic;
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
            set => WriteMemory(index, value);
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

        public IntCode(IEnumerable<long> program, ReadInput reader, WriteOutput writer)
        {
            _memory = program.ToArray();
            Reader = reader;
            Writer = writer;
        }

        public async Task Step()
        {
            if (!IsHalt)
            {
                (Op op, Mode[] modes) = Decode();

                switch (op)
                {
                    case Op.Halt:   IsHalt = true; break;
                    case Op.In:     WritePC(modes[0], await Reader()); break;
                    case Op.Out:    Writer(ReadPC(modes[0])); break;
                    case Op.Base:   RelativeBase += ReadPC(modes[0]); break;
                    default:
                        long val1 = ReadPC(modes[0]);
                        long val2 = ReadPC(modes[1]);

                        if (op == Op.JumpTrue || op == Op.JumpFalse)
                        {
                            if ((op == Op.JumpTrue && val1 != 0) || (op == Op.JumpFalse && val1 == 0))
                                PC = val2;
                        }
                        else
                        {
                            long val = op switch
                            {
                                Op.Add => val1 + val2,
                                Op.Mul => val1 * val2,
                                Op.LessThan => val1 < val2 ? 1 : 0,
                                Op.Equals => val1 == val2 ? 1 : 0,
                                _ => throw new InvalidOperationException("invalid op code")
                            };

                            WritePC(modes[2], val);
                        }
                        break;
                }
            }
        }

        public async Task Run()
        {
            while (!IsHalt)
                await Step();
        }

        private (Op, Mode[]) Decode()
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

        private long ReadPC() => ReadPC(Mode.Imm);

        private long ReadPC(Mode mode) => ReadMemory(PC++, mode);

        private long ReadMemory(long address, Mode mode) => mode switch
        {
            Mode.Imm => ReadMemory(address),
            _ => ReadMemory(IndirectAddressTarget(address, mode))
        };

        private void WritePC(Mode mode, long value) => WriteMemory(PC++, mode, value);

        private void WriteMemory(long address, Mode mode, long value) =>
            WriteMemory(IndirectAddressTarget(address, mode), value);

        private long IndirectAddressTarget(long address, Mode mode) => mode switch
        {
            Mode.Pos => ReadMemory(address),
            Mode.Relative => ReadMemory(address) + RelativeBase,
            _ => throw new InvalidOperationException("invalid address mode")
        };

        private long ReadMemory(long address)
        {
            EnsureMemory(address);
            return _memory[address];
        }

        private void WriteMemory(long address, long value)
        {
            EnsureMemory(address);
            _memory[address] = value;
        }

        private void EnsureMemory(long address)
        {
            if (address >= _memory.Length)
            {
                long[] newMemory = new long[Math.Max(address + 1, _memory.Length * 2)];
                Array.Copy(_memory, 0, newMemory, 0, _memory.Length);
                _memory = newMemory;
            }
        }

        private long[] _memory;
    }
}
