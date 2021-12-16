using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode._2021
{
    public class Day16
    {
        private record Packet(
            int BitCount,
            int Version,
            int Type,
            int LengthTypeId,
            int Count,
            long Literal);

        private class BitReader
        {
            readonly int[] _packet;
            int _packetIdx;
            int _bitIdx;

            public BitReader(string packet)
            {
                _packet = packet.Select(c => CharToByte(c)).ToArray();
                _packetIdx = 0;
                _bitIdx = 3;
            }

            public bool TryReadBits(int bits, out int result)
            {
                result = 0;

                if (_packetIdx >= _packet.Length)
                    return false;

                if (bits > 32)
                    throw new InvalidOperationException();

                while (bits-- > 0)
                {
                    result = (result << 1) | (_packet[_packetIdx] & (1 << _bitIdx)) >> (_bitIdx);
                    _bitIdx--;
                    if (_bitIdx < 0)
                    {
                        _packetIdx++;
                        _bitIdx = 3;

                        if (_packetIdx >= _packet.Length && bits >= 0)
                            return false;
                    }
                }

                return true;
            }

            public bool TryReadPacket([NotNullWhen(true)] out Packet? packet)
            {
                packet = null;

                if (!TryReadBits(3, out int version))
                    return false;

                if (!TryReadBits(3, out int type))
                    return false;

                int bitCount = 6;
                int lengthTypeId = 0;
                long literal = 0;
                int count = 0;

                if (type == 4)
                {
                    if (!TryReadLiteral(out literal, out int literalLength))
                        return false;
                    bitCount += literalLength;
                }
                else
                {
                    if (!TryReadBits(1, out lengthTypeId))
                        return false;

                    if (lengthTypeId == 0)
                    {
                        if (!TryReadBits(15, out count))
                            return false;
                        bitCount += 16;
                    }
                    else
                    {
                        if (!TryReadBits(11, out count))
                            return false;
                        bitCount += 12;
                    }
                }

                packet = new Packet(bitCount, version, type, lengthTypeId, count, literal);
                return true;
            }

            public bool TryReadLiteral(out long literal, out int bitLength)
            {
                literal = 0;
                bitLength = 0;

                while (true)
                {
                    if (!TryReadBits(5, out int next))
                        return false;

                    bitLength += 5;
                    if (bitLength > 80)
                        throw new InvalidOperationException();

                    literal = (literal << 4) | (long)(next & 0x0F);

                    if ((next & 0x10) == 0)
                        break;
                }

                return true;
            }

            private static int CharToByte(char c) =>
                (byte)(c <= '9' ? c - '0' : c - 'A' + 10);
        }

        BitReader _reader;

        public Day16()
        {
            _reader = new BitReader(File.ReadAllText("Inputs/Day16.txt"));
        }

        [Fact]
        public void Part1()
        {
            int answer = 0;
            while (_reader.TryReadPacket(out Packet? packet))
                answer += packet.Version;

            Assert.Equal(893, answer);
        }

        [Fact]
        public void Part2()
        {
            (long answer, _) = EvalNextPacket();
            Assert.Equal(4358595186090, answer);
        }

        private delegate bool tryEvalNextPacket(out long result);

        private (long result, int bitCount) EvalNextPacket()
        {
            if (!_reader.TryReadPacket(out Packet? packet))
                throw new InvalidOperationException();

            if (packet.Type is 4)
                return (packet.Literal, packet.BitCount);
            else
            {
                int totalBitCount = 0;
                int immediateSubs = 0;
                int max = packet.Count;

                bool CheckBitCount() => totalBitCount >= max;
                bool CheckImmediateSubCount() => immediateSubs >= max;

                Func<bool> check = packet.LengthTypeId is 0 ?
                    CheckBitCount :
                    CheckImmediateSubCount;

                bool TryEvalNext(out long evalResult)
                {
                    evalResult = 0;
                    if (check())
                        return false;

                    (evalResult, int bitCount) = EvalNextPacket();
                    immediateSubs++;
                    totalBitCount += bitCount;
                    return true;
                }

                long result = 0;
                long evalResult;
                long a;
                long b;

                switch (packet.Type)
                {
                    case 0:
                        while (TryEvalNext(out evalResult))
                            result += evalResult;
                        break;

                    case 1:
                        result = 1;
                        while (TryEvalNext(out evalResult))
                            result *= evalResult;
                        break;

                    case 2:
                        TryEvalNext(out result);
                        while (TryEvalNext(out evalResult))
                            if (evalResult < result)
                                result = evalResult;
                        break;

                    case 3:
                        TryEvalNext(out result);
                        while (TryEvalNext(out evalResult))
                            if (evalResult > result)
                                result = evalResult;
                        break;

                    case 5:
                        TryEvalNext(out a);
                        TryEvalNext(out b);
                        result = a > b ? 1 : 0;
                        break;

                    case 6:
                        TryEvalNext(out a);
                        TryEvalNext(out b);
                        result = a < b ? 1 : 0;
                        break;

                    case 7:
                        TryEvalNext(out a);
                        TryEvalNext(out b);
                        result = a == b ? 1 : 0;
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                return (result, totalBitCount + packet.BitCount);
            }
        }
    }
}
