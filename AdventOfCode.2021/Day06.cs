namespace AdventOfCode._2021
{
    public class Day06
    {
        int[] _input;

        public Day06()
        {
            _input = File.ReadAllText("Inputs/Day06.txt").Split(',').Select(int.Parse).ToArray();
        }

        [Fact]
        public void Part1()
        {
            ulong answer = Run(80);

            Assert.Equal(386755ul, answer);
        }

        [Fact]
        public void Part2()
        {
            ulong answer = Run(256);

            Assert.Equal(1732731810807ul, answer);
        }

        private ulong Run(int iterations)
        {
            ulong[] fish = new ulong[9];
            ulong[] next = (ulong[])fish.Clone();

            foreach (int start in _input)
                fish[start]++;

            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < 8; j++)
                    next[j] = fish[j + 1];

                next[6] += fish[0];
                next[8] = fish[0];

                var tmp = fish;
                fish = next;
                next = tmp;
            }

            ulong total = 0;
            for (int i = 0; i < fish.Length; i++)
                total += fish[i];

            return total;
        }
    }
}
