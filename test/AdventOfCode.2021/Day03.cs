namespace AdventOfCode._2021
{
    public class Day03
    {
        string[] _binaryNums;

        public Day03()
        {
            _binaryNums = File.ReadAllLines("Inputs/Day03.txt");
        }

        [Fact]
        public void Part1()
        {
            int length = _binaryNums[0].Length;

            int gamma = 0;
            int bit = 0;
            while (bit < length)
            {
                int count = _binaryNums.Count(num => num[bit] == '1');
                if (count > _binaryNums.Length / 2)
                    gamma |= 1 << length - bit - 1;
                bit++;
            }

            int epsilon = ~gamma & ((1 << length) - 1);
            int answer = gamma * epsilon;

            Assert.Equal(1307354, answer);
        }

        [Fact]
        public void Part2()
        {
            string[] nums = _binaryNums;
            int bit = 0;

            while (nums.Length != 1)
            {
                nums = OxygenFilter(bit, nums);
                bit++;
            }
            int oxygen = Convert.ToInt32(nums[0], 2);

            nums = _binaryNums;
            bit = 0;
            while (nums.Length != 1)
            {
                nums = CarbonFilter(bit, nums);
                bit++;
            }
            int carbon = Convert.ToInt32(nums[0], 2);

            int answer = oxygen * carbon;

            Assert.Equal(482500, answer);

            string[] OxygenFilter(int bit, string[] starting)
            {
                int count = starting.Count(num => num[bit] == '1');
                char target = count >= starting.Length - count ? '1' : '0';
                return starting.Where(num => num[bit] == target).ToArray();
            }

            string[] CarbonFilter(int bit, string[] starting)
            {
                int count = starting.Count(num => num[bit] == '0');
                char target = count <= starting.Length - count ? '0' : '1';
                return starting.Where(num => num[bit] == target).ToArray();
            }
        }
    }
}
