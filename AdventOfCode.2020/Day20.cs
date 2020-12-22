using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdventOfCode.Common;

using Xunit;

namespace AdventOfCode._2020
{
    public class Day20
    {
        // cheats, these are known
        private const int TileSize = 10;
        private const int MapSize = 12;

        [Flags]
        private enum Orientation : int
        {
            // lazy, this will probably bite me
            One     = 0b00000001,
            Two     = 0b00000010,
            Three   = 0b00000100,
            Four    = 0b00001000,
            Five    = 0b00010000,
            Six     = 0b00100000,
            Seven   = 0b01000000,
            Eight   = 0b10000000
        }

        private class Tile
        {
            public readonly int Width;
            public readonly int Height;
            public readonly int Id;
            public readonly int Index;

            private readonly bool[,] _tile;

            public Tile(int width, int height, int id, int index, bool[,] tile)
            {
                Width = width;
                Height = height;
                Id = id;
                Index = index;
                _tile = tile;
            }

            public OrientedTile Orient(Orientation orientation) =>
                new OrientedTile(this, orientation);

            public bool Get(int x, int y, Orientation orientation) => orientation switch
            {
                Orientation.One     => _tile[            x,              y],
                Orientation.Two     => _tile[Width - 1 - x,              y],
                Orientation.Three   => _tile[            x, Height - 1 - y],
                Orientation.Four    => _tile[Width - 1 - x, Height - 1 - y],
                Orientation.Five    => _tile[            y,              x],
                Orientation.Six     => _tile[Width - 1 - y,              x],
                Orientation.Seven   => _tile[            y, Height - 1 - x],
                Orientation.Eight   => _tile[Width - 1 - y, Height - 1 - x],
                _ => throw new InvalidOperationException()
            };
        }

        private readonly struct OrientedTile : IEquatable<OrientedTile>
        {
            public readonly int Width;
            public readonly int Height;
            public readonly Tile Tile;
            public readonly Orientation Orientation;

            public OrientedTile(Tile tile, Orientation orientation)
            {
                Tile = tile;
                Orientation = orientation;

                Width = orientation switch
                {
                    Orientation.One     => tile.Width,
                    Orientation.Two     => tile.Width,
                    Orientation.Three   => tile.Width,
                    Orientation.Four    => tile.Width,
                    Orientation.Five    => tile.Height,
                    Orientation.Six     => tile.Height,
                    Orientation.Seven   => tile.Height,
                    Orientation.Eight   => tile.Height,
                    _ => throw new InvalidOperationException()
                };

                Height = orientation switch
                {
                    Orientation.One     => tile.Height,
                    Orientation.Two     => tile.Height,
                    Orientation.Three   => tile.Height,
                    Orientation.Four    => tile.Height,
                    Orientation.Five    => tile.Width,
                    Orientation.Six     => tile.Width,
                    Orientation.Seven   => tile.Width,
                    Orientation.Eight   => tile.Width,
                    _ => throw new InvalidOperationException()
                };
            }

            public bool Get(int x, int y) => Tile.Get(x, y, Orientation);

            public bool Top(int i) => Get(i, 0);

            public bool Bottom(int i) => Get(i, Height - 1);

            public bool Left(int i) => Get(0, i);

            public bool Right(int i) => Get(Width - 1, i);

            public bool Equals(OrientedTile other)
            {
                return Tile.Equals(other.Tile) && Orientation.Equals(other.Orientation);
            }

            public override bool Equals(object other)
            {
                return other is OrientedTile otherTile && Equals(otherTile);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Tile.GetHashCode(), Orientation);
            }
        }

        private class TileSet
        {
            private List<Tile> _tiles = new(MapSize * MapSize);

            public void AddTile(int id, bool[,] tile)
            {
                _tiles.Add(new Tile(TileSize, TileSize, id, _tiles.Count, tile));
            }

            public long Solve()
            {
                List<OrientedTile> solution = GetSolution();

                int topLeft = 0;
                int topRight = MapSize - 1;
                int bottomLeft = MapSize * (MapSize - 1);
                int bottomRight = MapSize * MapSize - 1;

                long answer = 1;
                answer *= solution[topLeft].Tile.Id;
                answer *= solution[topRight].Tile.Id;
                answer *= solution[bottomLeft].Tile.Id;
                answer *= solution[bottomRight].Tile.Id;

                return answer;
            }

            public bool[,] GetImage()
            {
                List<OrientedTile> solution = GetSolution();

                int imageSize = MapSize * (TileSize - 2);
                bool[,] image = new bool[imageSize, imageSize];

                for (int tileIndex = 0; tileIndex < solution.Count; tileIndex++)
                {
                    OrientedTile tile = solution[tileIndex];
                    IntVec2 mapCoords = (tileIndex % MapSize, tileIndex / MapSize);
                    IntVec2 imageBase = mapCoords * (TileSize - 2);
                    IntVec2 tileBase = (1, 1);

                    for (int i = 0; i < TileSize - 2; i++)
                        for (int j = 0; j < TileSize - 2; j++)
                            image[imageBase.X + i, imageBase.Y + j] = tile.Get(tileBase.X + i, tileBase.Y + j);
                }

                return image;
            }

            private List<OrientedTile> GetSolution()
            {
                List<OrientedTile> solution = new(_tiles.Count);
                HashSet<Tile> candidates = new(_tiles);
                if (!SolveInternal(solution, candidates))
                {
                    throw new InvalidOperationException();
                }
                return solution;
            }

            private bool SolveInternal(List<OrientedTile> placed, HashSet<Tile> candidateTiles)
            {
                if (placed.Count == _tiles.Count)
                    return true;

                foreach (var cand in candidateTiles.ToList())
                    foreach (Orientation orientation in AllOrientations())
                    {
                        OrientedTile orientedCandidate = cand.Orient(orientation);
                        if (!TestCandidate(placed, orientedCandidate))
                            continue;

                        placed.Add(orientedCandidate);
                        candidateTiles.Remove(cand);
                        if (SolveInternal(placed, candidateTiles))
                            return true;
                        else
                        {
                            candidateTiles.Add(cand);
                            placed.RemoveAt(placed.Count - 1);

                            if (placed.Count > 0)
                                return false;
                        }
                    }

                return false;
            }

            private bool TestCandidate(List<OrientedTile> placed, OrientedTile candidate)
            {
                (int x, int y) = (placed.Count % MapSize, placed.Count / MapSize);

                if (x > 0)
                {
                    int leftIndex = MapSize * y + (x - 1);
                    for (int i = 0; i < TileSize; i++)
                        if (placed[leftIndex].Right(i) != candidate.Left(i))
                            return false;
                }
                else if (y > 0)
                {
                    int upIndex = MapSize * (y - 1) + x;
                    for (int i = 0; i < TileSize; i++)
                        if (placed[upIndex].Bottom(i) != candidate.Top(i))
                            return false;
                }

                return true;
            }
        }

        private TileSet _tileSet;

        public Day20()
        {
            _tileSet = new TileSet();

            string[] input = File.ReadAllLines("Inputs/Day20.txt");

            int index = 0;
            while (index < input.Length)
            {
                string idLine = input[index++];
                int spaceIndex = idLine.IndexOf(' ');
                int id = int.Parse(idLine.AsSpan(spaceIndex + 1, idLine.Length - spaceIndex - 2));

                // cheat, size is 10
                bool[,] tile = new bool[10, 10];
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        tile[i, j] = input[j + index][i] == '#';
                _tileSet.AddTile(id, tile);

                index += 11;
            }
        }

        [Fact]
        public void Part1()
        {
            long answer = _tileSet.Solve();
            Assert.Equal(104831106565027, answer);
        }

        [Fact]
        public void Part2()
        {
            string[] searchString = new string[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            bool[,] image = _tileSet.GetImage();

            IntVec2 searchSize = (searchString[0].Length, searchString.Length);
            bool[,] searchArray = new bool[searchString[0].Length, searchString.Length];
            for (int j = 0; j < searchSize.Y; j++)
                for (int i = 0; i < searchSize.X; i++)
                    searchArray[i, j] = searchString[j][i] == '#';

            Tile searchTile = new Tile(searchSize.X, searchSize.Y, 0, 0, searchArray);

            HashSet<IntVec2> monsterCoords = new();
            foreach(Orientation orientation in AllOrientations())
            {
                OrientedTile orientedTile = searchTile.Orient(orientation);
                SearchImage(image, orientedTile, monsterCoords);
            }

            const int imageSize = MapSize * (TileSize - 2);
            int answer = 0;
            for (int i = 0; i < imageSize; i++)
                for (int j = 0; j < imageSize; j++)
                    if (image[i, j] && !monsterCoords.Contains((i, j)))
                        answer++;

            Assert.Equal(2093, answer);
        }

        private void SearchImage(bool[,] image, OrientedTile tile, HashSet<IntVec2> monsterCoords)
        {
            const int imageSize = MapSize * (TileSize - 2);

            for (int i = 0; i < imageSize - tile.Width; i++)
                for (int j = 0; j < imageSize - tile.Height; j++)
                {
                    for (int u = 0; u < tile.Width; u++)
                        for (int v = 0; v < tile.Height; v++)
                            if (tile.Get(u, v) && !image[i + u, j + v])
                                goto NoMatch;

                    for (int u = 0; u < tile.Width; u++)
                        for (int v = 0; v < tile.Height; v++)
                            if (tile.Get(u, v))
                                monsterCoords.Add((i + u, j + v));
                    NoMatch:;
                }

        }

        private static IEnumerable<Orientation> AllOrientations()
        {
            yield return Orientation.One;
            yield return Orientation.Two;
            yield return Orientation.Three;
            yield return Orientation.Four;
            yield return Orientation.Five;
            yield return Orientation.Six;
            yield return Orientation.Seven;
            yield return Orientation.Eight;
        }
    }
}
