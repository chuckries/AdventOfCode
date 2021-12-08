namespace AdventOfCode._2021
{
    public class Day01
    {
        int[] _input;

        public Day01()
        {
            _input = File.ReadAllLines("Inputs/Day01.txt").Select(int.Parse).ToArray();
        }


        [Fact]
        public void Part1()
        {
            int answer = Counter(1);
            Assert.Equal(1342, answer);
        }

        [Fact]
        public void Part2()
        {
            int answer = Counter(3);
            Assert.Equal(1378, answer);
        }

        private int Counter(int count) => _input.Skip(count).Zip(_input).Count(pair => pair.First > pair.Second);

    }
}
