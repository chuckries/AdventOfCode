using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2020
{
    public class Day25
    {
        private const long DoorPublicKey = 13316116;
        private const long CardPublicKey = 13651422;

        [Fact]
        public void Part1()
        {
            int DoorLoopSize = GetLoopSize(DoorPublicKey);

            long EncryptionKey = 1;
            for (int i = 0; i < DoorLoopSize; i++)
            {
                EncryptionKey = EncryptStep(EncryptionKey, CardPublicKey);
            }

            Assert.Equal(12929, EncryptionKey);
        }

        private int GetLoopSize(long target)
        {
            const long SubjectNumber = 7;

            long val = 1;
            int loopSize = 1;
            while (true)
            {
                if ((val = EncryptStep(val, SubjectNumber)) == target)
                    return loopSize;

                ++loopSize;
            }
        }

        private long EncryptStep(long val, long subjectNumber)
        {
            const long Divider = 20201227;

            val *= subjectNumber;
            val %= Divider;

            return val;
        }
    }
}
