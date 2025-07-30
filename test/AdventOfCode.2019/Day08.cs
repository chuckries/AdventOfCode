using System.Text;

namespace AdventOfCode._2019;

public class Day08
{
    const int WIDTH = 25;
    const int HEIGHT = 6;
    const int AREA = WIDTH * HEIGHT;

    char[,,] _layers;

    public Day08()
    {
        string input = File.ReadAllText("Inputs/Day08.txt");
        int countLayers = input.Length / AREA;
        _layers = new char[countLayers, HEIGHT, WIDTH];

        int index = 0;
        for (int layer = 0; layer < countLayers; layer++)
            for (int height = 0; height < HEIGHT; height++)
                for (int width = 0; width < WIDTH; width++)
                    _layers[layer, height, width] = input[index++];
    }

    [Fact]
    public void Part1()
    {
        int minLayer = -1;
        int minZeros = int.MaxValue;
        for (int layer = 0; layer < _layers.GetLength(0); layer++)
        {
            int zeros = 0;
            for (int height = 0; height < HEIGHT; height++)
                for (int width = 0; width < WIDTH; width++)
                    if (_layers[layer, height, width] == '0')
                        zeros++;

            if (zeros < minZeros)
            {
                minZeros = zeros;
                minLayer = layer;
            }
        }

        int ones = 0;
        int twos = 0;
        for (int height = 0; height < HEIGHT; height++)
            for (int width = 0; width < WIDTH; width++)
            {
                char c = _layers[minLayer, height, width];
                if (c == '1') ones++;
                else if (c == '2') twos++;
            }

        int answer = ones * twos;
        Assert.Equal(2032, answer);
    }

    [Fact]
    public void Part2()
    {
        char[,] render = new char[HEIGHT, WIDTH];
        for (int height = 0; height < HEIGHT; height++)
            for (int width = 0; width < WIDTH; width++)
                render[height, width] = '2';

        for (int layer = 0; layer < _layers.GetLength(0); layer++)
            for (int height = 0; height < HEIGHT; height++)
                for (int width = 0; width < WIDTH; width++)
                    if (render[height, width] == '2')
                        render[height, width] = _layers[layer, height, width];

        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        for (int height = 0; height < HEIGHT; height++)
        {
            for (int width = 0; width < WIDTH; width++)
                sb.Append(render[height, width] == '1' ? '█' : ' ');
            sb.AppendLine();
        }
        string answer = sb.ToString();

        const string expected = @"
 ██  ████  ██  █  █  ██  
█  █ █    █  █ █  █ █  █ 
█    ███  █    █  █ █    
█    █    █    █  █ █ ██ 
█  █ █    █  █ █  █ █  █ 
 ██  █     ██   ██   ███ 
";

        Assert.Equal(expected, answer);
    }
}
