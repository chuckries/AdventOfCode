namespace AdventOfCode._2020;

public class Day20
{
    // cheats, these are known
    private const int TileSize = 10;
    private const int MapSize = 12;

    private enum EdgeType
    {
        Top,
        RTop,
        Bottom,
        RBottom,
        Left,
        RLeft,
        Right,
        RRight
    };

    [Flags]
    private enum Orientation : int
    {
        // lazy, this will probably bite me
        One = 0b00000001,
        Two = 0b00000010,
        Three = 0b00000100,
        Four = 0b00001000,
        Five = 0b00010000,
        Six = 0b00100000,
        Seven = 0b01000000,
        Eight = 0b10000000
    }

    private class Tile
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int Id;
        public readonly int Index;

        public readonly int Top;
        public readonly int RTop;
        public readonly int Bottom;
        public readonly int RBottom;
        public readonly int Left;
        public readonly int RLeft;
        public readonly int Right;
        public readonly int RRight;

        private readonly bool[,] _tile;

        public Tile(int width, int height, int id, int index, bool[,] tile)
        {
            Width = width;
            Height = height;
            Id = id;
            Index = index;
            _tile = tile;

            int top, rtop, bottom, rbottom, left, rleft, right, rright;
            top = rtop = bottom = rbottom = left = rleft = right = rright = 0;

            for (int i = 0; i < Width; i++)
            {
                top |= (_tile[i, 0] ? 1 : 0) << i;
                rtop |= (_tile[Width - 1 - i, 0] ? 1 : 0) << i;
                bottom |= (_tile[i, Height - 1] ? 1 : 0) << i;
                rbottom |= (_tile[Width - 1 - i, Height - 1] ? 1 : 0) << i;
            }

            for (int i = 0; i < Height; i++)
            {
                left |= (_tile[0, i] ? 1 : 0) << i;
                rleft |= (_tile[0, Height - 1 - i] ? 1 : 0) << i;
                right |= (_tile[Width - 1, i] ? 1 : 0) << i;
                rright |= (_tile[Width - 1, Height - 1 - i] ? 1 : 0) << i;
            }

            Top = top;
            RTop = rtop;
            Bottom = bottom;
            RBottom = rbottom;
            Left = left;
            RLeft = rleft;
            Right = right;
            RRight = rright;
        }

        public OrientedTile Orient(Orientation orientation) => new OrientedTile(this, orientation);

        public bool Get(int x, int y, Orientation orientation) => orientation switch
        {
            Orientation.One => _tile[x, y],
            Orientation.Two => _tile[Width - 1 - x, y],
            Orientation.Three => _tile[x, Height - 1 - y],
            Orientation.Four => _tile[Width - 1 - x, Height - 1 - y],
            Orientation.Five => _tile[y, x],
            Orientation.Six => _tile[Width - 1 - y, x],
            Orientation.Seven => _tile[y, Height - 1 - x],
            Orientation.Eight => _tile[Width - 1 - y, Height - 1 - x],
            _ => throw new InvalidOperationException()
        };
    }

    private class OrientedTile
    {
        public readonly int Width;
        public readonly int Height;
        public readonly Tile Tile;
        public readonly Orientation Orientation;
        public readonly int Top;
        public readonly int Bottom;
        public readonly int Left;
        public readonly int Right;

        public bool this[int x, int y] => Tile.Get(x, y, Orientation);

        public OrientedTile(Tile tile, Orientation orientation)
        {
            Tile = tile;
            Orientation = orientation;

            Width = orientation switch
            {
                Orientation.One => tile.Width,
                Orientation.Two => tile.Width,
                Orientation.Three => tile.Width,
                Orientation.Four => tile.Width,
                Orientation.Five => tile.Height,
                Orientation.Six => tile.Height,
                Orientation.Seven => tile.Height,
                Orientation.Eight => tile.Height,
                _ => throw new InvalidOperationException()
            };

            Height = orientation switch
            {
                Orientation.One => tile.Height,
                Orientation.Two => tile.Height,
                Orientation.Three => tile.Height,
                Orientation.Four => tile.Height,
                Orientation.Five => tile.Width,
                Orientation.Six => tile.Width,
                Orientation.Seven => tile.Width,
                Orientation.Eight => tile.Width,
                _ => throw new InvalidOperationException()
            };

            Top = orientation switch
            {
                Orientation.One => tile.Top,
                Orientation.Two => tile.RTop,
                Orientation.Three => tile.Bottom,
                Orientation.Four => tile.RBottom,
                Orientation.Five => tile.Left,
                Orientation.Six => tile.Right,
                Orientation.Seven => tile.RLeft,
                Orientation.Eight => tile.RRight,
                _ => throw new InvalidOperationException()
            };

            Bottom = orientation switch
            {
                Orientation.One => tile.Bottom,
                Orientation.Two => tile.RBottom,
                Orientation.Three => tile.Top,
                Orientation.Four => tile.RTop,
                Orientation.Five => tile.Right,
                Orientation.Six => tile.Left,
                Orientation.Seven => tile.RRight,
                Orientation.Eight => tile.RLeft,
                _ => throw new InvalidOperationException()
            };

            Left = orientation switch
            {
                Orientation.One => tile.Left,
                Orientation.Two => tile.Right,
                Orientation.Three => tile.RLeft,
                Orientation.Four => tile.RRight,
                Orientation.Five => tile.Top,
                Orientation.Six => tile.RTop,
                Orientation.Seven => tile.Bottom,
                Orientation.Eight => tile.RBottom,
                _ => throw new InvalidOperationException()
            };

            Right = orientation switch
            {
                Orientation.One => tile.Right,
                Orientation.Two => tile.Left,
                Orientation.Three => tile.RRight,
                Orientation.Four => tile.RLeft,
                Orientation.Five => tile.Bottom,
                Orientation.Six => tile.RBottom,
                Orientation.Seven => tile.Top,
                Orientation.Eight => tile.RTop,
                _ => throw new InvalidOperationException()
            };
        }
    }

    private class TileSet
    {
        private readonly struct EdgeId
        {
            public readonly EdgeType Type;
            public readonly Tile Tile;

            public EdgeId(EdgeType type, Tile tile)
            {
                Type = type;
                Tile = tile;
            }
        }

        private List<Tile> _tiles = new(MapSize * MapSize);
        private Dictionary<int, List<EdgeId>> _edgeLookup = new(MapSize * MapSize * 8); // cap too big, oh well

        public void AddTile(int id, bool[,] tileBuffer)
        {
            Tile tile = new Tile(TileSize, TileSize, id, _tiles.Count, tileBuffer);
            _tiles.Add(tile);

            AddEdge(tile.Top, EdgeType.Top, tile);
            AddEdge(tile.RTop, EdgeType.RTop, tile);
            AddEdge(tile.Bottom, EdgeType.Bottom, tile);
            AddEdge(tile.RBottom, EdgeType.RBottom, tile);
            AddEdge(tile.Left, EdgeType.Left, tile);
            AddEdge(tile.RLeft, EdgeType.RLeft, tile);
            AddEdge(tile.Right, EdgeType.Right, tile);
            AddEdge(tile.RRight, EdgeType.RRight, tile);

            void AddEdge(int edge, EdgeType type, Tile tile)
            {
                if (!_edgeLookup.TryGetValue(edge, out List<EdgeId> edges))
                    _edgeLookup.Add(edge, new List<EdgeId> { new EdgeId(type, tile) });
                else
                    edges.Add(new EdgeId(type, tile));
            }
        }

        public long GetMultipliedCornerIds() =>
            GetCorners().Aggregate(1L, (total, corner) => total * corner.Id);

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
                        image[imageBase.X + i, imageBase.Y + j] = tile[tileBase.X + i, tileBase.Y + j];
            }

            return image;
        }

        private IEnumerable<Tile> GetCorners()
        {
            int cornersFound = 0;
            foreach (Tile tile in _tiles)
            {
                if ((_edgeLookup[tile.Top].Count == 1 || _edgeLookup[tile.Bottom].Count == 1) &&
                    (_edgeLookup[tile.Left].Count == 1 || _edgeLookup[tile.Right].Count == 1))
                {
                    yield return tile;
                    cornersFound++;
                }

                if (cornersFound == 4)
                    break;
            }
        }

        private List<OrientedTile> GetSolution()
        {
            List<OrientedTile> solution = new(_tiles.Count);

            OrientedTile previous = GetFirstOrientedCorner();
            solution.Add(previous);

            for (int i = 1; i < _tiles.Count; i++)
            {
                int edgeId;
                EdgeType edgeType;
                if (i % MapSize > 0)
                {
                    edgeId = previous.Right;
                    edgeType = EdgeType.Right;
                }
                else
                {
                    previous = solution[i - MapSize];
                    edgeId = previous.Bottom;
                    edgeType = EdgeType.Bottom;
                }

                previous = GetNextOrientedTile(edgeId, edgeType, previous.Tile);
                solution.Add(previous);
            }

            return solution;
        }

        private OrientedTile GetNextOrientedTile(int edgeId, EdgeType edgeType, Tile previousTile)
        {
            List<EdgeId> matches = _edgeLookup[edgeId];
            EdgeId correct = matches[0].Tile == previousTile ? matches[1] : matches[0];

            Orientation orientation = edgeType switch
            {
                EdgeType.Bottom => correct.Type switch
                {
                    EdgeType.Top => Orientation.One,
                    EdgeType.RTop => Orientation.Two,
                    EdgeType.Bottom => Orientation.Three,
                    EdgeType.RBottom => Orientation.Four,
                    EdgeType.Left => Orientation.Five,
                    EdgeType.RLeft => Orientation.Seven,
                    EdgeType.Right => Orientation.Six,
                    EdgeType.RRight => Orientation.Eight,
                    _ => throw new InvalidOperationException(),
                },
                EdgeType.Right => correct.Type switch
                {
                    EdgeType.Top => Orientation.Five,
                    EdgeType.RTop => Orientation.Six,
                    EdgeType.Bottom => Orientation.Seven,
                    EdgeType.RBottom => Orientation.Eight,
                    EdgeType.Left => Orientation.One,
                    EdgeType.RLeft => Orientation.Three,
                    EdgeType.Right => Orientation.Two,
                    EdgeType.RRight => Orientation.Four,
                    _ => throw new InvalidOperationException(),
                },
                _ => throw new InvalidOperationException()
            };

            return correct.Tile.Orient(orientation);
        }

        private OrientedTile GetFirstOrientedCorner()
        {
            foreach (Tile tile in _tiles)
            {
                if (_edgeLookup[tile.Top].Count == 1)
                {
                    if (_edgeLookup[tile.Left].Count == 1)
                        return tile.Orient(Orientation.One);
                    else if (_edgeLookup[tile.Right].Count == 1)
                        return tile.Orient(Orientation.Two);
                }
                else if (_edgeLookup[tile.Bottom].Count == 1)
                {
                    if (_edgeLookup[tile.Left].Count == 1)
                        return tile.Orient(Orientation.Three);
                    else if (_edgeLookup[tile.Right].Count == 1)
                        return tile.Orient(Orientation.Four);
                }
            }

            throw new InvalidOperationException();
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
            bool[,] tile = new bool[TileSize, TileSize];
            for (int i = 0; i < TileSize; i++)
                for (int j = 0; j < TileSize; j++)
                    tile[i, j] = input[j + index][i] == '#';
            _tileSet.AddTile(id, tile);

            index += TileSize + 1;
        }
    }

    [Fact]
    public void Part1()
    {
        long answer = _tileSet.GetMultipliedCornerIds();
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
        foreach (Orientation orientation in s_Orientations)
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
                        if (tile[u, v] && !image[i + u, j + v])
                            goto NoMatch;

                for (int u = 0; u < tile.Width; u++)
                    for (int v = 0; v < tile.Height; v++)
                        if (tile[u, v])
                            monsterCoords.Add((i + u, j + v));

                        NoMatch:;
            }

    }

    private static Orientation[] s_Orientations = new[]
    {
        Orientation.One,
        Orientation.Two,
        Orientation.Three,
        Orientation.Four,
        Orientation.Five,
        Orientation.Six,
        Orientation.Seven,
        Orientation.Eight,
    };
}
