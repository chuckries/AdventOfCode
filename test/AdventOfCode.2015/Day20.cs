namespace AdventOfCode._2015;

public class Day20
{
    const int Input = 36_000_000;

    [Fact]
    public void Part1()
    {
        int target = Input / 10;

        int[] bigArray = new int[target + 1];
        int i = 1;
        for (; i <= target; i++)
        {
            int index = i;
            while (index <= target)
            {
                bigArray[index] += i;
                index += i;
            }

            if (bigArray[i] >= target)
                break;
        }

        Assert.Equal(831_600, i);
    }

    [Fact]
    public void Part2()
    {
        int target = Input / 11;

        int[] bigArray = new int[target + 1];
        int i = 1;
        for (; i <= target; i++)
        {
            int index = i;
            for (int j = 0; j < 50 && index <= target; j++)
            {
                bigArray[index] += i;
                index += i;
            }

            if (bigArray[i] >= target)
                break;
        }

        Assert.Equal(884_520, i);
    }
}
