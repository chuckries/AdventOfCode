using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace AdventOfCode._2016
{
    public class Day08
    {
        const int Width = 50;
        const int Height = 6;
        bool[,] _display = new bool[Width, Height];
        bool[] _tmpRow = new bool[Width];
        bool[] _tmpCol = new bool[Height];

        private string[] _input;

        public Day08()
        {
            _input = File.ReadAllLines("Inputs/Day08.txt");
            foreach (string s in _input)
            {
                string[] tokens = s.Split(' ');
                if (tokens[0] == "rect")
                {
                    int index = tokens[1].IndexOf('x');
                    int x = int.Parse(tokens[1].AsSpan(0, index));
                    int y = int.Parse(tokens[1].AsSpan(index + 1));
                    Rect(x, y);
                }
                else if (tokens[0] == "rotate")
                {
                    int count = int.Parse(tokens[^1]);
                    int index = tokens[2].IndexOf('=');
                    int id = int.Parse(tokens[2].AsSpan(index + 1));
                    if (tokens[1] == "row")
                        RotateRow(id, count);
                    else if (tokens[1] == "column")
                        RotateColumn(id, count);
                    else throw new InvalidOperationException();
                }
                else throw new InvalidOperationException();
            }
        }

        [Fact]
        public void Part1()
        {
            int answer = 0;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (_display[i, j])
                        answer++;

            Assert.Equal(128, answer);
        }

        [Fact]
        public void Part2()
        {
            StringBuilder sb = new StringBuilder(Height * Width * 2);
            sb.AppendLine();

            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    sb.Append(_display[i, j] ? '█' : ' ');
                }
                sb.AppendLine();
            }

            // EOARGPHYAO
            string expected = @"
████  ██   ██  ███   ██  ███  █  █ █   █ ██   ██  
█    █  █ █  █ █  █ █  █ █  █ █  █ █   ██  █ █  █ 
███  █  █ █  █ █  █ █    █  █ ████  █ █ █  █ █  █ 
█    █  █ ████ ███  █ ██ ███  █  █   █  ████ █  █ 
█    █  █ █  █ █ █  █  █ █    █  █   █  █  █ █  █ 
████  ██  █  █ █  █  ███ █    █  █   █  █  █  ██  
";

            Assert.Equal(expected, sb.ToString());
        }

        private void Rect(int x, int y)
        {
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    _display[i, j] = true;
        }

        private void RotateColumn(int col, int count)
        {
            for (int j = 0; j < Height; j++)
                _tmpCol[j] = _display[col, j];

            for (int j = 0; j < Height; j++)
                _display[col, (j + count) % Height] = _tmpCol[j];
        }

        private void RotateRow(int row, int count)
        {
            for (int i = 0; i < Width; i++)
                _tmpRow[i] = _display[i, row];

            for (int i = 0; i < Width; i++)
                _display[(i + count) % Width, row] = _tmpRow[i];
        }
    }
}
