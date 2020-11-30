using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace rapid
{
    public delegate bool Gen<T>(Params @params, Seed seed, out (T Value, Seed Next) result);


    public record Params(int seed = 0)
    {
        public Seed InitialSeed()
        {
            return new Seed(seed);
        }
    }

    public static partial class GeneratorExtensions
    {
        /// <summary>
        /// Gen<T> extension to generate a sequence of values
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="size"></param>
        /// <param name="params"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Enumerate<T>(this Params generator, Gen<T> gen)
        {
            var seed = generator.InitialSeed();
            while (true)
            {
                if (gen(generator, seed, out var result))
                {
                    yield return result.Value;
                    seed = result.Next;
                }
            }
        }

        public static Gen<T> Const<T>(this Params generator, T value)
        {
            return (Params generator, Seed seed, out (T, Seed) result) =>
            {
                result = (value, seed);
                return true;
            };
        }

        /// <summary>
        /// foundational Gen<char> combinator for character of the given range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Gen<char> CharRange(this Params generator, string range)
        {
            return (Params generator, Seed seed, out (char, Seed) result) =>
            {
                var lng = seed.Next(out var next);
                result = (range[(int)(lng % (ulong)range.Length)], next);
                return true;
            };
        }

        internal static Gen<char> HexDigit(this Params generator) =>
            generator.CharRange("0123456789ABCDEF");

        public static Gen<int> Range(this Params generator, int min, int max)
        {
            return (Params generator, Seed seed, out (int, Seed) result) =>
            {
                var value = seed.Next(min, max, out var next);
                result = (value, next);
                return true;
            };
        }

        public static Gen<DateTime> Range(this Params generator, DateTime min, DateTime max)
        {
            return (Params generator, Seed seed, out (DateTime, Seed) result) =>
            {
                var value = seed.Next(min.Ticks, max.Ticks, out var next);
                result = (new DateTime(value), next);
                return true;
            };
        }

        public static Gen<string> String(this Params generator, int n, Gen<char> chars)
        {
            return (Params generator, Seed seed, out (string, Seed) result) =>
            {
                var sb = new StringBuilder();
                while (sb.Length < n)
                {
                    if (chars(generator, seed, out var temp))
                    {
                        sb.Append(temp.Value);
                        seed = temp.Next;
                    }
                }
                result = (sb.ToString(), seed);
                return true;
            };
        }

        public static Gen<T> OneOf<T>(this Params generator, params T[] items)
        {
            return generator.Choose((IReadOnlyList<T>)items);
        }

        public static Gen<T> Choose<T>(this Params generator, IReadOnlyList<T> items)
        {
            return (Params generator, Seed seed, out (T, Seed) result) =>
           {
               var lng = seed.Next(out var next);
               var val = items[(int)(lng % (ulong)items.Count)];
               result = (val, next);
               return true;
           };
        }

        public static Gen<T> Choose<K, T>(this Params generator, IDictionary<K, T> items)
        {
            return generator.Choose(items.Values.ToList());
        }

        internal static Gen<string> Word(this Params generator) => generator.Choose(File
            .ReadLines("text.txt")
            .Where(line => !string.IsNullOrEmpty(line.Trim()))
            .SelectMany(line => line.Split(' '))
            .Select(word => word.Trim(',', '.', ' '))
            .ToList()
        );

        internal static Gen<string> Sentence(this Params generator) => generator.Choose(File
            .ReadLines("text.txt")
            .Where(line => !string.IsNullOrEmpty(line.Trim()))
            .ToList()
        );

        public static Gen<IReadOnlyList<T>> List<T>(this Params generator, int min, int max, Gen<T> gen)
        {
            return (Params generator, Seed seed, out (IReadOnlyList<T>, Seed) result) =>
            {
                var accu = new List<T>();
                var len = seed.Next(min, max, out seed);
                while (accu.Count < len)
                {
                    if (gen(generator, seed, out var temp))
                    {
                        accu.Add(temp.Value);
                        seed = temp.Next;
                    }
                }
                result = (accu.AsReadOnly(), seed);
                return true;
            };
        }
    }
}