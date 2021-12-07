using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode._2021
{
    public class Day04
    {
        const int BoardSize = 5;

        private class Board
        {
            private (int num, bool has)[,] _spaces = new(int, bool)[BoardSize, BoardSize];

            public static Board Parse(TextReader reader)
            {
                Board board = new Board();
                for (int j = 0; j < BoardSize; j++)
                {
                    int i = 0;
                    string line = reader.ReadLine()!;
                    foreach (int num in line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
                    {
                        board._spaces[i, j] = (num, false);
                        i++;
                    }
                }

                return board;
            }

            public void CallNum(int num)
            {
                for (int i = 0; i < BoardSize; i++)
                    for (int j = 0; j < BoardSize; j++)
                        if (_spaces[i, j].num == num)
                            _spaces[i, j].has = true;
            }

            public bool IsWinner()
            {
                bool isWin;
                for (int i = 0; i < BoardSize; i++)
                {
                    isWin = true;
                    for (int j = 0; j < BoardSize && isWin; j++)
                        isWin &= _spaces[i, j].has;
                    if (isWin)
                        return true;
                }

                for (int j = 0; j < BoardSize; j++)
                {
                    isWin = true;
                    for (int i = 0; i < BoardSize && isWin; i++)
                        isWin &= _spaces[i, j].has;
                    if (isWin)
                        return true;
                }

                return false;
            }

            public int SumUnmarked()
            {
                int sum = 0;
                for (int i = 0; i < BoardSize; i++)
                    for (int j = 0; j < BoardSize; j++)
                        if (!_spaces[i, j].has)
                            sum += _spaces[i, j].num;
                return sum;
            }
        }

        int[] _numbers;
        List<Board> _boards;

        public Day04()
        {
            using (StreamReader sr = new StreamReader(new FileStream("Inputs/Day04.txt", FileMode.Open, FileAccess.Read)))
            {
                _numbers = sr.ReadLine()!.Split(',').Select(int.Parse).ToArray();

                _boards = new();
                while (true)
                {
                    if (sr.ReadLine() is null)
                        break;

                    _boards.Add(Board.Parse(sr));
                }
            }
        }

        [Fact]
        public void Part1()
        {
            int answer = 0;
            foreach (int num in _numbers)
            {
                foreach (Board board in _boards)
                    board.CallNum(num);

                Board? winner = _boards.FirstOrDefault(b => b.IsWinner());
                if (winner is not null)
                {
                    answer = winner.SumUnmarked() * num;
                    break;
                }
            }

            Assert.Equal(54275, answer);
        }

        [Fact]
        public void Part2()
        {
            Board? lastWinner = null;
            int winningNumber = 0;
            foreach (int num in _numbers)
            {
                foreach (Board board in _boards)
                {
                    board.CallNum(num);
                    if (board.IsWinner())
                    {
                        lastWinner = board;
                        winningNumber = num;
                    }
                }

                _boards.RemoveAll(b => b.IsWinner());
            }

            int answer = lastWinner!.SumUnmarked() * winningNumber;

            Assert.Equal(13158, answer);
        }
    }
}
