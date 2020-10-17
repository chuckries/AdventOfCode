using System;
using System.Buffers;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    public abstract class IntCodeBase
    {
        protected enum Op : long
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

        protected enum Mode : long
        {
            Pos = 0,
            Imm = 1,
            Relative = 2
        }

        public delegate void OutputWriter(long value);

        public long PC { get; set; }

        public long RelativeBase { get; set; }

        public bool IsHalt { get; private set; }

        public long this[int index]
        {
            get => ReadMemory(index);
            set => WriteMemory(index, value);
        }

        public OutputWriter Writer { get; set; }

        protected IntCodeBase(IEnumerable<long> program)
            : this(program, null)
        {
        }

        protected IntCodeBase(IEnumerable<long> program, OutputWriter writer)
        {
            _program = program.ToArray();
            Writer = writer;

            Reset();
        }

        public void Reset()
        {
            if (_memory == null || _memory.Length < _program.Length)
            {
                _memory = (long[])_program.Clone();
            }
            else
            {
                Array.Clear(_memory, _program.Length, _memory.Length - _program.Length);
                Array.Copy(_program, 0, _memory, 0, _program.Length);
            }

            PC = 0;
            RelativeBase = 0;
            IsHalt = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StepCore(Op op, Mode[] modes)
        {
            switch (op)
            {
                case Op.Halt: IsHalt = true; break;
                case Op.Out: Writer(ReadPC(modes[0])); break;
                case Op.Base: RelativeBase += ReadPC(modes[0]); break;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Decode(out Op op, Mode[] modes)
        {
            long instr = ReadPC();

            op = (Op)(instr % 100);
            instr /= 100;

            modes[0] = (Mode)(instr % 10);
            instr /= 10;
            modes[1] = (Mode)(instr % 10);
            instr /= 10;
            modes[2] = (Mode)(instr % 10);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long ReadPC() => ReadPC(Mode.Imm);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long ReadPC(Mode mode) => ReadMemory(PC++, mode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long ReadMemory(long address, Mode mode) => mode switch
        {
            Mode.Imm => ReadMemory(address),
            _ => ReadMemory(IndirectAddressTarget(address, mode))
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WritePC(Mode mode, long value) => WriteMemory(PC++, mode, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteMemory(long address, Mode mode, long value) =>
            WriteMemory(IndirectAddressTarget(address, mode), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long IndirectAddressTarget(long address, Mode mode) => mode switch
        {
            Mode.Pos => ReadMemory(address),
            Mode.Relative => ReadMemory(address) + RelativeBase,
            _ => throw new InvalidOperationException("invalid address mode")
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long ReadMemory(long address)
        {
            EnsureMemory(address);
            return _memory[address];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteMemory(long address, long value)
        {
            EnsureMemory(address);
            _memory[address] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureMemory(long address)
        {
            if (address >= _memory.Length)
            {
                long[] newMemory = new long[Math.Max(address + 1, _memory.Length * 2)];
                Array.Copy(_memory, 0, newMemory, 0, _memory.Length);
                _memory = newMemory;
            }
        }

        private long[] _program;
        private long[] _memory;
    }

    public class IntCodeAsync : IntCodeBase
    {
        public delegate Task<long> InputReaderAsync(CancellationToken cancellationToken);
        public InputReaderAsync Reader { get; set; }

        public IntCodeAsync(IEnumerable<long> program)
            : base(program)
        { 
        }

        public IntCodeAsync(IEnumerable<long> program, InputReaderAsync reader, OutputWriter writer)
            : base(program, writer)
        {
            Reader = reader;
        }

        public async Task RunAsync()
        {
            await RunAsync(CancellationToken.None);
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            Op op;
            Mode[] modes = new Mode[3];
            while (!IsHalt)
            {
                Decode(out op, modes);

                switch (op)
                {
                    case Op.In: WritePC(modes[0], await Reader(cancellationToken)); break;
                    default: StepCore(op, modes); break;
                }
            }
        }
    }

    public class IntCode : IntCodeBase
    {
        public delegate long InputReader();
        public InputReader Reader { get; set; }

        public IntCode(IEnumerable<long> program)
            : base(program)
        {
        }

        public IntCode(IEnumerable<long> program, InputReader reader, OutputWriter writer)
            : base(program, writer)
        {
            Reader = reader;
        }

        public void Run()
        {
            while (!IsHalt)
            {
                Step();
            }
        }

        public void Step()
        {
            Op op;
            Mode[] modes = new Mode[3];
            if (!IsHalt)
            {
                Decode(out op, modes);

                switch (op)
                {
                    case Op.In: WritePC(modes[0], Reader()); break;
                    default: StepCore(op, modes); break;
                }
            }
        }
    }

}
