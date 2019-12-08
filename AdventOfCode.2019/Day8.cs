using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace AdventOfCode._2019
{
    public class Day8
    {
        const int WIDTH = 25;
        const int HEIGHT = 6;
        const int AREA = WIDTH * HEIGHT;

        char[,,] _layers;

        public Day8()
        {
            string input = File.ReadAllText("Inputs/Day8.txt");
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
            int minLayer = -1 ;
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

            StringBuilder sb = new StringBuilder(
                Environment.NewLine, 
                AREA + (HEIGHT + 1) * Environment.NewLine.Length
                );

            for (int height = 0; height < HEIGHT; height++)
            {
                for (int width = 0; width < WIDTH; width++)
                    sb.Append(render[height, width]);
                sb.AppendLine();
            }
            string answer = sb.ToString();

            const string expected = @"
0110011110011001001001100
1001010000100101001010010
1000011100100001001010000
1000010000100001001010110
1001010000100101001010010
0110010000011000110001110
";

            Assert.Equal(expected, answer);
        }
    }
}
