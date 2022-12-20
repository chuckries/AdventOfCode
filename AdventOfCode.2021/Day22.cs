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
            long answer = _input
                .Where(r => r.Low.X >= -50 && r.Low.Y >= -50 && r.Low.Z >= -50 && r.High.X <= 50 && r.High.Y <= 50 && r.High.Z <= 50)
                .Aggregate(new List<Region>(), (s, r) => AddRegionToSpace(s, r))
                .Where(r => r.Status)
                .Sum(r => r.Volume);

            Assert.Equal(588120, answer);
        }

        [Fact]
        public void Part2()
        {
            long answer = _input
                .Aggregate(new List<Region>(), (s, r) => AddRegionToSpace(s, r))
                .Where(r => r.Status)
                .Sum(r => r.Volume);

            Assert.Equal(1134088247046731, answer);
        }

        private List<Region> AddRegionToSpace(List<Region> space, Region newRegion)
        {
            List<Region> newSpace = new(space.Count * 6 + 1);

            foreach(Region existing in space)
                IntersectRegions(existing, newRegion, newSpace);

            newSpace.Add(newRegion);
            return newSpace;
        }

        private void IntersectRegions(Region existing, Region incoming, List<Region> space)
        {
            if (incoming.Low.X > existing.High.X || incoming.High.X < existing.Low.X ||
                incoming.Low.Y > existing.High.Y || incoming.High.Y < existing.Low.Y ||
                incoming.Low.Z > existing.High.Z || incoming.High.Z < existing.Low.Z)
            {
                space.Add(existing);
                return;
            }

            if (incoming.Low.X <= existing.Low.X && incoming.High.X >= existing.High.X &&
                incoming.Low.Y <= existing.Low.Y && incoming.High.Y >= existing.High.Y &&
                incoming.Low.Z <= existing.Low.Z && incoming.High.Z >= existing.High.Z)
            {
                return;
            }

            // -X
            if (existing.Low.X < incoming.Low.X)
            {
                space.Add(new Region(existing.Status, existing.Low, new(incoming.Low.X - 1, existing.High.Y, existing.High.Z)));
                existing = new Region(existing.Status, new(incoming.Low.X, existing.Low.Y, existing.Low.Z), existing.High);
            }

            // +X
            if (existing.High.X > incoming.High.X)
            {
                space.Add(new Region(existing.Status, new(incoming.High.X + 1, existing.Low.Y, existing.Low.Z), existing.High));
                existing = new Region(existing.Status, existing.Low, new(incoming.High.X, existing.High.Y, existing.High.Z));
            }

            // -Y
            if (existing.Low.Y < incoming.Low.Y)
            {
                space.Add(new Region(existing.Status, existing.Low, new(existing.High.X, incoming.Low.Y - 1, existing.High.Z)));
                existing = new Region(existing.Status, new(existing.Low.X, incoming.Low.Y, existing.Low.Z), existing.High);
            }

            // +Y
            if (existing.High.Y > incoming.High.Y)
            {
                space.Add(new Region(existing.Status, new(existing.Low.X, incoming.High.Y + 1, existing.Low.Z), existing.High));
                existing = new Region(existing.Status, existing.Low, new(existing.High.X, incoming.High.Y, existing.High.Z));
            }

            // -Z
            if (existing.Low.Z < incoming.Low.Z)
            {
                space.Add(new Region(existing.Status, existing.Low, new(existing.High.X, existing.High.Y, incoming.Low.Z - 1)));
                existing = new Region(existing.Status, new(existing.Low.X, existing.Low.Y, incoming.Low.Z), existing.High);
            }

            // +Z
            if (existing.High.Z > incoming.High.Z)
            {
                space.Add(new Region(existing.Status, new(existing.Low.X, existing.Low.Y, incoming.High.Z + 1), existing.High));
            }
        }
    }
}
