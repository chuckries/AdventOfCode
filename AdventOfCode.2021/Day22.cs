using NuGet.Frameworks;

namespace AdventOfCode._2021
{
    public class Day22
    {
        private readonly record struct Region(bool Status, IntVec3 Low, IntVec3 High)
        {
            public long Volume =>
                ((long)High.X - Low.X + 1) * ((long)High.Y - Low.Y + 1) * ((long)High.Z - Low.Z + 1);
        }

        private Region[] _input;

        public Day22()
        {
            _input = File.ReadAllLines("Inputs/Day22.txt")
                .Select(l => l.Split(new[] { " ", "=", "..", "," }, StringSplitOptions.None))
                .Select(tok =>
                {
                    return new Region(
                        tok[0] == "on",
                        new IntVec3(tok[2], tok[5], tok[8]),
                        new IntVec3(tok[3], tok[6], tok[9])
                        );
                }).ToArray();
        }

        [Fact]
        public void Part1()
        {
            List<Region> space = _input.Take(1).ToList();

            foreach (var instr in _input.Skip(1))
            {
                if (instr.High.X < -50 || instr.Low.X > 50 ||
                    instr.High.Y < -50 || instr.Low.Y > 50 ||
                    instr.High.Z < -50 || instr.Low.Z > 50)
                {
                    continue;
                }

                space = AddRegionToSpace(space, instr);
            }

            long answer = space.Where(r => r.Status).Aggregate(0L, (tot, r) => tot += r.Volume);
            Assert.Equal(588120, answer);
        }

        [Fact]
        public void Part2()
        {
            List<Region> space = _input.Take(1).ToList();
            foreach (var instr in _input.Skip(1))
            {
                space = AddRegionToSpace(space, instr);
            }

            long answer = space.Where(r => r.Status).Aggregate(0L, (tot, r) => tot += r.Volume);
            Assert.Equal(1134088247046731, answer);
        }

        private List<Region> AddRegionToSpace(List<Region> space, Region incoming)
        {
            List<Region> newSpace = new(space.Count * 2);
            List<Region> toIntersect = new() { incoming };
            foreach (Region existing in space)
            {
                List<Region> nextToIntersect = new(toIntersect.Count);
                foreach(Region intersectWith in toIntersect)
                {
                    (var olds, var news) = IntersectRegions(existing, intersectWith);
                    newSpace.AddRange(olds);
                    nextToIntersect.AddRange(news);
                }
                toIntersect = nextToIntersect;
            }

            newSpace.AddRange(toIntersect);
            return newSpace;
        }

        private (List<Region> olds, List<Region> news) IntersectRegions(Region existing, Region incoming)
        {
            List<Region> olds = new();
            List<Region> news = new();

            if (incoming.Low.X > existing.High.X || incoming.High.X < existing.Low.X ||
                incoming.Low.Y > existing.High.Y || incoming.High.Y < existing.Low.Y ||
                incoming.Low.Z > existing.High.Z || incoming.High.Z < existing.Low.Z)
            {
                // no intersection
                olds.Add(existing);
                news.Add(incoming);
                return (olds, news);
            }

            if (incoming.Low.X <= existing.Low.X && incoming.High.X >= existing.High.X &&
                incoming.Low.Y <= existing.Low.Y && incoming.High.Y >= existing.High.Y &&
                incoming.Low.Z <= existing.Low.Z && incoming.High.Z >= existing.High.Z)
            {
                //existing is fully consumed by incoming
                news.Add(incoming);
                return (olds, news);
            }

            news.Add(incoming);

            // -X
            if (existing.Low.X < incoming.Low.X)
            {
                olds.Add(new Region(existing.Status, existing.Low, new(incoming.Low.X - 1, existing.High.Y, existing.High.Z)));
                existing = new Region(existing.Status, new(incoming.Low.X, existing.Low.Y, existing.Low.Z), existing.High);
            }
            //else if (incoming.Low.X < existing.Low.X)
            //{
            //    news.Add(new Region(incoming.Status, incoming.Low, new(existing.Low.X - 1, incoming.High.Y, incoming.High.Z)));
            //    incoming = new Region(incoming.Status, new(existing.Low.X, incoming.Low.Y, incoming.Low.Z), incoming.High);
            //}

            // +X
            if (existing.High.X > incoming.High.X)
            {
                olds.Add(new Region(existing.Status, new(incoming.High.X + 1, existing.Low.Y, existing.Low.Z), existing.High));
                existing = new Region(existing.Status, existing.Low, new(incoming.High.X, existing.High.Y, existing.High.Z));
            }
            //else if (incoming.High.X > existing.High.X)
            //{
            //    news.Add(new Region(incoming.Status, new(existing.High.X + 1, incoming.Low.Y, incoming.Low.Z), incoming.High));
            //    incoming = new Region(incoming.Status, incoming.Low, new(existing.High.X, incoming.High.Y, incoming.High.Z));
            //}

            // -Y
            if (existing.Low.Y < incoming.Low.Y)
            {
                olds.Add(new Region(existing.Status, existing.Low, new(existing.High.X, incoming.Low.Y - 1, existing.High.Z)));
                existing = new Region(existing.Status, new(existing.Low.X, incoming.Low.Y, existing.Low.Z), existing.High);
            }
            //else if (incoming.Low.Y < existing.Low.Y)
            //{
            //    news.Add(new Region(incoming.Status, incoming.Low, new(incoming.High.X, existing.Low.Y - 1, incoming.High.Z)));
            //    incoming = new Region(incoming.Status, new(incoming.Low.X, existing.Low.Y, incoming.Low.Z), incoming.High);
            //}

            // +Y
            if (existing.High.Y > incoming.High.Y)
            {
                olds.Add(new Region(existing.Status, new(existing.Low.X, incoming.High.Y + 1, existing.Low.Z), existing.High));
                existing = new Region(existing.Status, existing.Low, new(existing.High.X, incoming.High.Y, existing.High.Z));
            }
            //else if (incoming.High.Y > existing.High.Y)
            //{
            //    news.Add(new Region(incoming.Status, new(incoming.Low.X, existing.High.Y + 1, incoming.Low.Z), incoming.High));
            //    incoming = new Region(incoming.Status, incoming.Low, new(incoming.High.X, existing.High.Y, incoming.High.Z));
            //}

            // -Z
            if (existing.Low.Z < incoming.Low.Z)
            {
                olds.Add(new Region(existing.Status, existing.Low, new(existing.High.X, existing.High.Y, incoming.Low.Z - 1)));
                existing = new Region(existing.Status, new(existing.Low.X, existing.Low.Y, incoming.Low.Z), existing.High);
            }
            //else if (incoming.Low.Z < existing.Low.Z)
            //{
            //    news.Add(new Region(incoming.Status, incoming.Low, new(incoming.High.X, incoming.High.Y, existing.Low.Z - 1)));
            //    incoming = new Region(incoming.Status, new(incoming.Low.X, incoming.Low.Y, existing.Low.Z), incoming.High);
            //}

            // +Z
            if (existing.High.Z > incoming.High.Z)
            {
                olds.Add(new Region(existing.Status, new(existing.Low.X, existing.Low.Y, incoming.High.Z + 1), existing.High));
                existing = new Region(existing.Status, existing.Low, new(existing.High.X, existing.High.Y, incoming.High.Z));
            }
            //else if (incoming.High.Z > existing.High.Z)
            //{
            //    news.Add(new Region(incoming.Status, new(incoming.Low.X, incoming.Low.Y, existing.High.Z + 1), incoming.High));
            //    incoming = new Region(incoming.Status, incoming.Low, new(incoming.High.X, incoming.High.Y, existing.High.Z));
            //}

            //olds.Add(incoming);

            return (olds, news);
        }
    }
}
