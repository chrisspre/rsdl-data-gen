using System;
using System.Runtime.InteropServices;

namespace rapid
{
    public static class SeedExtensions
    {
        public static int Next(this Seed seed, int min, int max, out Seed next)
        {
            var lng = seed.Next(out next);
            return ((int)(lng % ((ulong)(max - min)))) + min;
        }

        public static long Next(this Seed seed, long min, long max, out Seed next)
        {
            var lng = seed.Next(out next);
            return ((long)(lng % ((ulong)(max - min)))) + min;
        }
    }

    public class Seed
    {
        private readonly Random rng;

        public Seed(int seed)
        {
            rng = new Random(seed);
        }

        public ulong Next(out Seed next)
        {
            var (a, b) = Create(rng);
            next = new Seed(b);
            return a;
        }

        public (ulong, Seed) Next()
        {
            var (a, b) = Create(rng);
            return (a, new Seed(b));
        }

        private static (ulong, int) Create(Random rng)
        {
            Span<byte> bytes = stackalloc byte[12];
            rng.NextBytes(bytes);
            var a = MemoryMarshal.Read<ulong>(bytes);
            var b = MemoryMarshal.Read<int>(bytes.Slice(8));
            return (a, b);
        }
    }

}
