using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2019
{
    public class Day4
    {
        const int RANGE_START = 158126;
        const int RANGE_END = 624574;

        IEnumerable<int> _range = Enumerable.Range(RANGE_START, RANGE_END - RANGE_START + 1);

        [Fact]
        public void Part1()
        {
            int answer = _range.Select(GetDigits).Where(IsStrictlyIncreasing).Where(HasAnyDouble).Count();
            Assert.Equal(1665, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = _range.Select(GetDigits).Where(IsStrictlyIncreasing).Where(HasAtLeastExactlyOneDouble).Count();
            Assert.Equal(1131, answer);
        }

        private bool HasAnyDouble(int[] digits)
        {
            for (int i = 0; i < digits.Length - 1; i++)
            {
                if (digits[i] == digits[i + 1])
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAtLeastExactlyOneDouble(int[] digits)
        {
            int i = 0;
            while (i < digits.Length)
            {
                int candidate = digits[i++];
                int count = 1;

                while (i < digits.Length && digits[i] == candidate)
                {
                    count++;
                    i++;
                }

                if (count == 2)
                    return true;
            }

            return false;
        }

        private bool IsStrictlyIncreasing(int[] digits)
        {
            for (int i = 0; i < digits.Length - 1; i++)
            {
                if (digits[i + 1] > digits[i])
                {
                    return false;
                }
            }

            return true;
        }

        private int[] GetDigits(int number)
        {
            List<int> digits = new List<int>();

            while (number > 0)
            {
                digits.Add(number % 10);
                number /= 10;
            }

            return digits.ToArray();
        }
    }
}
