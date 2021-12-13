using System.Text;

namespace AdventOfCode._2021
{
    public class Day13
    {
        private readonly List<IntVec2> _points;
        private readonly List<IntVec2> _folds;

        public Day13()
        {
            _points = new();
            _folds = new();

            using (StreamReader sr = new StreamReader(new FileStream("Inputs/Day13.txt", FileMode.Open, FileAccess.Read)))
            {
                string line;
                while (string.Empty != (line = sr.ReadLine()!))
                {
                    string[] tok = line.Split(',');
                    _points.Add(new IntVec2(tok[0], tok[1]));
                }

                while (null != (line = sr.ReadLine()!))
                {
                    string[] tok = line.Split()[^1].Split('=');
                    int num = int.Parse(tok[1]);
                    if (tok[0] == "x")
                        _folds.Add(new IntVec2(num, 0));
                    else
                        _folds.Add(new IntVec2(0, num));
                }
            }
        }

        [Fact]
        public void Part1()
        {
            HashSet<IntVec2> src = new(_points);
            HashSet<IntVec2> dst = new(src.Count);
            FoldSet(src, dst, _folds[0]);
            int answer = dst.Count;

            Assert.Equal(759, answer);
        }

        [Fact]
        public void Part2()
        {
            HashSet<IntVec2> current = new(_points);
            HashSet<IntVec2> next = new(current.Count);

            foreach (IntVec2 fold in _folds)
            {
                FoldSet(current, next, fold);

                var tmp = current;
                current = next;
                next = tmp;
                next.Clear();
            }

            (_, IntVec2 max) = current.MinMax();

            StringBuilder sb = new StringBuilder();
            for (int j = 0; j <= max.Y; j++)
            {
                sb.Append(Environment.NewLine);
                for (int i = 0; i <= max.X; i++)
                {
                    if (current.Contains(new IntVec2(i, j)))
                        sb.Append('█');
                    else
                        sb.Append(' ');
                }
            }

            string answer = sb.ToString();

            const string actual = @"
█  █ ████  ██  ███  ████ █  █ ███  ███ 
█  █ █    █  █ █  █    █ █ █  █  █ █  █
████ ███  █    █  █   █  ██   █  █ █  █
█  █ █    █    ███   █   █ █  ███  ███ 
█  █ █    █  █ █ █  █    █ █  █    █ █ 
█  █ ████  ██  █  █ ████ █  █ █    █  █";
            Assert.Equal(actual, answer);
        }

        private void FoldSet(HashSet<IntVec2> src, HashSet<IntVec2> dst, IntVec2 fold)
        {
            if (fold.X == 0)
                foreach (IntVec2 p in src)
                    dst.Add(FoldY(p, fold.Y));
            else
                foreach (IntVec2 p in src)
                    dst.Add(FoldX(p, fold.X));
        }

        private IntVec2 FoldX(IntVec2 p, int fold) =>
            new IntVec2(Fold(p.X, fold), p.Y);

        private IntVec2 FoldY(IntVec2 p, int fold) =>
            new IntVec2(p.X, Fold(p.Y, fold));

        private int Fold(int num, int fold)
        {
            if (num < fold)
                return num;
            else
                return fold - (num - fold);
        }
    }
}
