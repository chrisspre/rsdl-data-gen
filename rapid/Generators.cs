using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace rapid
{
    public delegate bool Gen<T>(Parameters parameters, Seed seed, out (T Value, Seed Next) result);

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
        public static IEnumerable<T> Enumerate<T>(this Gen<T> gen, int size, Parameters @params)
        {
            var seed = @params.InitialSeed();
            for (int i = 0; i < size; i++)
            {
                if (gen(@params, seed, out var result))
                {
                    yield return result.Value;
                    seed = result.Next;
                }
            }
        }

        /// <summary>
        /// foundational Gen<char> combinator for character of the given range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Gen<char> CharRange(string range)
        {
            return (Parameters @params, Seed seed, out (char, Seed) result) =>
            {
                var lng = seed.Next(out var next);
                result = (range[(int)(lng % (ulong)range.Length)], next);
                return true;
            };
        }

        internal static Gen<char> HexDigit =>
            Generators.CharRange("0123456789ABCDEF");

        public static Gen<int> Range(Range range)
        {
            return (Parameters @params, Seed seed, out (int, Seed) result) =>
            {
                var value = seed.NextInt(range, out var next);
                result = (value, next);
                return true;
            };
        }

        public static Gen<int> Range(int min, int max)
        {
            return (Parameters @params, Seed seed, out (int, Seed) result) =>
            {
                var value = seed.NextInt(min, max, out var next);
                result = (value, next);
                return true;
            };
        }

        public static Gen<string> String(int n, Gen<char> chars)
        {
            return (Parameters @params, Seed seed, out (string, Seed) result) =>
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
            return OneOf((IReadOnlyList<T>)items);
        }

        public static Gen<T> OneOf<T>(IReadOnlyList<T> items)
        {
            return (Parameters @params, Seed seed, out (T, Seed) result) =>
           {
               var lng = seed.Next(out var next);
               var val = items[(int)(lng % (ulong)items.Count)];
               result = (val, next);
               return true;
           };
        }
    }
}