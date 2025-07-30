namespace AdventOfCode._2018;

public class Day11
{
    private class Grid
    {
        public readonly int Size;
        public readonly int Serial;

        private int[,] _array;

        public Grid(int size, int serial)
        {
            Size = size;
            Serial = serial;

            _array = new int[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    _array[i, j] = GetPower(i, j);
        }

        public int GetPowerOfSquare(int i, int j, int size)
        {
            int total = 0;
            i--;
            j--;
            for (int u = i; u < i + size; u++)
                for (int v = j; v < j + size; v++)
                    total += _array[u, v];

            return total;
        }

        public (int i, int j, int power) MaxPowerForSquareOfSize(int squareSize)
        {
            int maxPower = int.MinValue;
            int maxI = -1;
            int maxJ = -1;

            for (int i = 1; i <= Size - squareSize + 1; i++)
            {
                for (int j = 1; j <= Size - squareSize + 1; j++)
                {
                    int squarePower = GetPowerOfSquare(i, j, squareSize);
                    if (squarePower > maxPower)
                    {
                        maxPower = squarePower;
                        maxI = i;
                        maxJ = j;
                    }
                }
            }

            return (maxI, maxJ, maxPower);
        }

        public (int i, int j, int size, int power) MaxPowerForAnySquare()
        {
            int[,,] dynamic = new int[Size, Size, Size]; // i, j, size

            int maxPower = int.MinValue;
            int maxI = -1;
            int maxJ = -1;
            int maxSize = -1;

            // fill in initial values
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                {
                    int power = _array[i, j];
                    if (power > maxPower)
                    {
                        maxPower = power;
                        maxI = i;
                        maxJ = j;
                        maxSize = 0;
                    }
                    dynamic[i, j, 0] = power;
                }

            // fill out rest of table
            for (int size = 1; size < Size; size++)
            {
                for (int i = 0; i < Size - size; i++)
                    for (int j = 0; j < Size - size; j++)
                    {
                        int power = dynamic[i, j, size - 1];
                        for (int u = i; u <= i + size; u++)
                            power += _array[u, j + size];
                        for (int v = j; v < j + size; v++)
                            power += _array[i + size, v];

                        if (power > maxPower)
                        {
                            maxPower = power;
                            maxI = i;
                            maxJ = j;
                            maxSize = size;
                        }
                        dynamic[i, j, size] = power;
                    }
            }

            return (maxI + 1, maxJ + 1, maxSize + 1, maxPower);
        }

        private int GetPower(int i, int j)
        {
            int rackId = i + 1 + 10;
            int power = rackId * (j + 1);
            power += Serial;
            power *= rackId;
            power = (power % 1000) / 100;
            power -= 5;
            return power;
        }
    }

    const int Input = 6548;
    Grid _grid = new Grid(300, Input);

    [Fact]
    public void Part1()
    {
        var answer = _grid.MaxPowerForSquareOfSize(3);
        Assert.Equal((21, 53), (answer.i, answer.j));
    }

    [Fact]
    public void Part2()
    {
        var answer = _grid.MaxPowerForAnySquare();
        Assert.Equal((233, 250, 12), (answer.i, answer.j, answer.size));
    }
}
