using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace rapid
{
    public delegate bool Gen<T>(Params parameters, Seed seed, out (T Value, Seed Next) result);

    public static partial class Generators
    {
        /// <summary>
        /// Gen<T> extension to generate a sequence of values
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="size"></param>
        /// <param name="params"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Enumerate<T>(this Gen<T> gen, Params @params)
        {
            var seed = @params.InitialSeed();
            while (true)
            {
                if (gen(@params, seed, out var result))
                {
                    yield return result.Value;
                    seed = result.Next;
                }
            }
        }

        public static Gen<T> Const<T>(T value)
        {
            return (Params @params, Seed seed, out (T, Seed) result) =>
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
        public static Gen<char> CharRange(string range)
        {
            return (Params @params, Seed seed, out (char, Seed) result) =>
            {
                var lng = seed.Next(out var next);
                result = (range[(int)(lng % (ulong)range.Length)], next);
                return true;
            };
        }

        internal static Gen<char> HexDigit =>
            Generators.CharRange("0123456789ABCDEF");

        public static Gen<int> Range(int min, int max)
        {
            return (Params @params, Seed seed, out (int, Seed) result) =>
            {
                var value = seed.Next(min, max, out var next);
                result = (value, next);
                return true;
            };
        }

        public static Gen<DateTime> Range(DateTime min, DateTime max)
        {
            return (Params @params, Seed seed, out (DateTime, Seed) result) =>
            {
                var value = seed.Next(min.Ticks, max.Ticks, out var next);
                result = (new DateTime(value), next);
                return true;
            };
        }

        public static Gen<string> String(int n, Gen<char> chars)
        {
            return (Params @params, Seed seed, out (string, Seed) result) =>
            {
                var sb = new StringBuilder();
                while (sb.Length < n)
                {
                    if (chars(@params, seed, out var temp))
                    {
                        sb.Append(temp.Value);
                        seed = temp.Next;
                    }
                }
                result = (sb.ToString(), seed);
                return true;
            };
        }

        public static Gen<T> OneOf<T>(params T[] items)
        {
            return Choose((IReadOnlyList<T>)items);
        }

        public static Gen<T> Choose<T>(IReadOnlyList<T> items)
        {
            return (Params @params, Seed seed, out (T, Seed) result) =>
           {
               var lng = seed.Next(out var next);
               var val = items[(int)(lng % (ulong)items.Count)];
               result = (val, next);
               return true;
           };
        }

        public static Gen<T> Choose<K, T>(IDictionary<K, T> items)
        {
            return Choose(items.Values.ToList());
        }

        internal static Gen<string> Word => Generators.Choose(File
            .ReadLines("text.txt")
            .Where(line => !string.IsNullOrEmpty(line.Trim()))
            .SelectMany(line => line.Split(' '))
            .Select(word => word.Trim(',', '.', ' '))
            .ToList()
        );

        internal static Gen<string> Sentence => Generators.Choose(File
            .ReadLines("text.txt")
            .Where(line => !string.IsNullOrEmpty(line.Trim()))
            .ToList()
        );

        public static Gen<IReadOnlyList<T>> List<T>(int min, int max, Gen<T> gen)
        {
            return (Params @params, Seed seed, out (IReadOnlyList<T>, Seed) result) =>
            {
                var accu = new List<T>();
                var len = seed.Next(min, max, out seed);
                while (accu.Count < len)
                {
                    if (gen(@params, seed, out var temp))
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