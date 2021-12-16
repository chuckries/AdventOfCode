using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AdventOfCode._2021
{
    public class Day16
    {
        private record Packet(
            int BitCount,
            int Version,
            int Type,
            int LengthTypeId,
            ulong Data);

        private class BitReader
        {
            //readonly string _packet;
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

                if (bits > 31)
                    throw new InvalidOperationException();

                while (bits-- > 0)
                {
                    result = (result << 1) | (_packet[_packetIdx] & (1 << _bitIdx)) >> (_bitIdx);
                    _bitIdx--;
                    if (_bitIdx < 0)
                    {
                        _packetIdx++;
                        _bitIdx = 3;

                        if (_packetIdx >= _packet.Length && bits != 0)
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
                ulong data = 0;

                if (type == 4)
                {
                    if (!TryReadLiteral(out data, out int literalLength))
                        return false;
                    bitCount += literalLength;
                }
                else
                {
                    if (!TryReadBits(1, out lengthTypeId))
                        return false;

                    int intData = 0;
                    if (lengthTypeId == 0)
                    {
                        if (!TryReadBits(15, out intData))
                            return false;
                        bitCount += 16;
                    }
                    else
                    {
                        if (!TryReadBits(11, out intData))
                            return false;
                        bitCount += 12;
                    }
                    data = (ulong)(long)intData;
                }

                packet = new Packet(bitCount, version, type, lengthTypeId, data);
                return true;
            }

            public bool TryReadLiteral (out ulong literal, out int bitLength)
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

                    literal = (literal << 4) | (ulong)(long)(next & 0x0F);

                    if ((next & 0x10) == 0)
                        break;
                }

                return true;
            }

            private static int CharToByte(char c) =>
                (byte)(c <= '9' ? c - '0' : c - 'A' + 10);
        }

        readonly Packet[] _packets;

        public Day16()
        {
            BitReader reader = new BitReader(File.ReadAllText("Inputs/Day16.txt"));
            List<Packet> packets = new();
            while (reader.TryReadPacket(out Packet? packet))
                packets.Add(packet);

            _packets = packets.ToArray();
        }

        [Fact]
        public void TestReader()
        {
            BitReader reader = new BitReader("D2FE28");

            Assert.True(reader.TryReadBits(3, out int version));
            Assert.Equal(6, version);

            Assert.True(reader.TryReadBits(3, out int type));
            Assert.Equal(4, type);

            Assert.True(reader.TryReadLiteral(out ulong literal, out int bitLength));
            Assert.Equal(2021ul, literal);
            Assert.Equal(15, bitLength);

            reader = new BitReader("D2FE28");
            Assert.True(reader.TryReadPacket(out Packet? packet));

            Assert.Equal(6, packet!.Version);
            Assert.Equal(4, packet!.Type);
            Assert.Equal(2021ul, packet!.Data);
            Assert.Equal(0, packet!.LengthTypeId);
            Assert.Equal(21, packet!.BitCount);
        }

        [Fact]
        public void Part1()
        {
            int answer = _packets.Sum(p => p.Version);
            Assert.Equal(893, answer);
        }

        [Fact]
        public void Part2()
        {
            (ulong answer, int bitCount, int nextIndex) = EvalPacket(0);
            Assert.Equal(0ul, answer);
        }

        private delegate bool tryEvalNextPacket(out ulong result);

        private (ulong result, int bitCount, int nextIndex) EvalPacket(int index)
        {
            int currentIndex = index;
            Packet p = _packets[currentIndex];

            if (p.Type == 4)
            {
                return (p.Data, p.BitCount, index);
            }
            else
            {
                tryEvalNextPacket tryEvalNext;
                int totalBitCount = 0;

                if (p.LengthTypeId == 0)
                {
                    int maxBitCount = (int)p.Data;
                    tryEvalNext = (out ulong evalResult) => {
                        evalResult = 0;
                        if (totalBitCount >= maxBitCount)
                            return false;

                        (evalResult, int bitCount, index) = EvalPacket(index + 1);
                        totalBitCount += bitCount;
                        return true;
                    };
                }
                else
                {
                    int packet = 0;
                    int packets = (int)p.Data;

                    tryEvalNext = (out ulong evalResult) =>
                    {
                        evalResult = 0;
                        if (packet >= packets)
                            return false;

                        (evalResult, int bitCount, index) = EvalPacket(index + 1);
                        totalBitCount += bitCount;
                        packet++;
                        return true;
                    };
                }

                ulong result = 0;

                if (p.Type == 0)
                {
                    while (tryEvalNext(out ulong evalResult))
                        result += evalResult;
                }
                else if (p.Type == 1)
                {
                    result = 1;
                    while (tryEvalNext(out ulong evalResult))
                        result *= evalResult;
                }
                else if (p.Type == 2)
                {
                    result = ulong.MaxValue;
                    while (tryEvalNext(out ulong evalResult))
                        if (evalResult < result)
                            result = evalResult;
                }
                else if (p.Type == 3)
                {
                    result = ulong.MinValue;
                    while (tryEvalNext(out ulong evalResult))
                        if (evalResult > result)
                            result = evalResult;
                }
                else if (p.Type == 5)
                {
                    tryEvalNext(out ulong a);
                    tryEvalNext(out ulong b);

                    result = (ulong)(a > b ? 1 : 0);
                }
                else if (p.Type == 6)
                {
                    tryEvalNext(out ulong a);
                    tryEvalNext(out ulong b);

                    result = (ulong)(a < b ? 1 : 0);
                }
                else if (p.Type == 7)
                {
                    tryEvalNext(out ulong a);
                    tryEvalNext(out ulong b);

                    result = (ulong)(a == b ? 1 : 0);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                return (result, totalBitCount + p.BitCount, index);
            }

        }
    } 
}
