namespace AdventOfCode._2015;

public class Day25
{
    const int Row = 2947;
    const int Col = 3029;

    [Fact]
    public void Part1()
    {
        int index = GetIndex(Col, Row);
        long code = 20151125;
        for (int i = 1; i < index; i++)
        {
            code = (code * 252533) % 33554393;
        }

        Assert.Equal(19980801, code);
    }

    private int GetIndex(int x, int y)
    {
        int diagNumber = x + y - 1;
        int diagTotal = (((diagNumber - 1) * diagNumber) / 2) + x;
        return diagTotal;
    }
}
