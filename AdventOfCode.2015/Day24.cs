using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Xunit;

namespace AdventOfCode._2015
{
    public class Day24
    {
        // This problem ended up making me do a lot of random research on partitioning and generating sub sets
        // For the input given, proving the partition of the remaining set doesn't actually matter, but I've included it for completeness
        // For Part1, once I find a given first partition, I check that the remaining set partitions in 2 by using a dynamic algorithm obtained from Wikipedia
        // For Part2, I use a variation of my own algorithm to generate subsets and recursively check that partitioning is possible
        // For the given inputs, this results in the fastest test execution.
        // I believe this is because the given inputs almost always partition, so greedily checking for partition of small sets (more partitions) finds
        // possible solutions quickly, faster than the dynamic approach.

        private int[] _weights;

        public Day24()
        {
            _weights = File.ReadAllLines("Inputs/Day24.txt").Select(int.Parse).ToArray();
        }

        [Fact]
        public void Part1()
        {
            int partialSum = _weights.Sum() / 3;

            long minQ = MinQForPartition(
                _weights,
                partialSum,
                CanPartition2);

            Assert.Equal(11846773891, minQ);
        }

        [Fact]
        public void Part2()
        {
            int partialSum = _weights.Sum() / 4;

            long minQ = MinQForPartition(
                _weights,
                partialSum,
                (weights, mask, count) => CanPartition(weights, mask, count, 3));

            Assert.Equal(80393059, minQ);
        }

        private static long MinQForPartition(int[] weights, int partialSum, Func<int[], int, int, bool> canPartitionRemainder)
        {
            long minQ = long.MaxValue;
            int minCount = int.MaxValue;
            int i = 2;
            while (i <= minCount)
            {
                foreach (int mask in MaskCombinations(weights.Length, i))
                {
                    if (Sum(weights, mask) == partialSum && canPartitionRemainder(weights, mask, i))
                    {
                        minCount = i;
                        long q = Product(weights, mask);
                        if (q < minQ)
                            minQ = q;
                    }
                }
                i++;
            }

            return minQ;
        }

        private static int Sum(int[] weights, int mask)
        {
            int total = 0;
            int bit = 1;
            for (int i = 0; i < weights.Length; i++)
            {
                if ((mask & bit) != 0)
                    total += weights[i];
                bit <<= 1;
            }

            return total;
        }

        private static long Product(int[] weights, int mask)
        {
            long total = 1;
            int bit = 1;
            for (int i = 0; i < weights.Length; i++)
            {
                if ((mask & bit) != 0)
                    total *= weights[i];
                bit <<= 1;
            }

            return total;
        }

        private static bool CanPartition2(int[] weights, int mask, int count)
        {
            int[] remaining = GetRemaining(weights, mask, count);
            int sum = remaining.Sum() / 2;

            bool[,] table = new bool[sum + 1, remaining.Length + 1];
            for (int i = 0; i <= remaining.Length; i++)
                table[0, i] = true;
            for (int i = 1; i <= sum; i++)
                table[i, 0] = false;

            for (int i = 1; i <= sum; i++)
                for (int j = 1; j <= remaining.Length; j++)
                {
                    if (i - remaining[j - 1] >= 0)
                        table[i, j] = table[i, j - 1] || table[i - remaining[j - 1], j - 1];
                    else
                        table[i, j] = table[i, j - 1];
                }

            return table[sum, remaining.Length];
        }

        // dynamic approach to 3 partition problem
        // slower than the recursive approach for the given input.
        private static bool CanPartition3(int[] weights, int mask, int count)
        {
            int[] remaining = GetRemaining(weights, mask, count);
            int sum = remaining.Sum();

            bool[,] table = new bool[sum + 1, sum + 1];
            table[0, 0] = true;

            for (int i = 0; i < remaining.Length; i++)
                for (int j = sum; j >= 0; j--)
                    for (int k = sum; k >= 0; k--)
                        if (table[j, k])
                        {
                            table[j + remaining[i], k] = true;
                            table[j, k + remaining[i]] = true;
                        }

            return table[sum / 3, sum / 3];
        }

        private static bool CanPartition(int[] weights, int mask, int count, int numPartitions)
        {
            int[] remaining = GetRemaining(weights, mask, count);
            int sum = remaining.Sum() / numPartitions;

            int i = 2;
            while (i < remaining.Length)
            {
                foreach (int subMask in MaskCombinations(remaining.Length, i))
                {
                    if (Sum(remaining.ToArray(), subMask) == sum)
                    {
                        if (numPartitions == 2 || CanPartition(remaining, subMask, i, numPartitions--))
                            return true;
                    }
                }
                i++;
            }

            return false;
        }

        private static int[] GetRemaining(int[] weights, int mask, int count)
        {
            int[] remaining = new int[weights.Length - count];
            int j = 0;
            for (int i = 0; i < weights.Length; i++)
                if ((mask & (1 << i)) == 0)
                    remaining[j++] = weights[i];

            return remaining;
        }

        /// <summary>
        /// Generates bitmasks with a specific number of bits sets within an overall width of bits. 
        /// Based on Python's itertools pseudo implementation for Combinations
        /// https://docs.python.org/3/library/itertools.html#itertools.combinations
        /// </summary>
        /// <param name="width">The maximum bit width to use</param>
        /// <param name="count">The number of bits that should be set</param>
        /// <returns>The width choose count possible bitmasks</returns>
        private static IEnumerable<int> MaskCombinations(int width, int count)
        {
            if (width <= 0 || width > 32)
                throw new ArgumentException(nameof(width));
            if (count <= 0 || count > width)
                throw new ArgumentException(nameof(count));

            int[] indices = new int[count];
            for (int i = 0; i < count; i++)
                indices[i] = i;

            yield return MakeMask(indices);

            while (true)
            {
                int i;
                for (i = count - 1; i >= 0; i--)
                {
                    if (indices[i] != i + width - count)
                        break;
                }
                if (i < 0)
                    yield break;

                indices[i]++;
                for (int j = i + 1; j < count; j++)
                {
                    indices[j] = indices[j - 1] + 1;
                }
                yield return MakeMask(indices);
            }


            static int MakeMask(int[] indices)
            {
                int mask = 0;
                for (int i = 0; i < indices.Length; i++)
                    mask |= (1 << indices[i]);
                return mask;
            }
        }
    }
}
