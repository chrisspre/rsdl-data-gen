using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace generate
{

    public static partial class Combinators
    {
        public static Gen<string> Concat(Gen<string> a, Gen<string> b)
        {
            return
                from va in a
                from vb in b
                select va + vb;
        }

        public static Gen<T> Const<T>(T value)
        {
            return (Seed seed, out (T, Seed) result) =>
            {
                result = (value, seed);
                return true;
            };
        }

        /// <summary>
        /// foundational Gen<char> combinator that chooses a character from the given character set
        /// </summary>
        /// <param name="charset">characters to choose from</param>
        /// <returns></returns>
        public static Gen<char> CharRange(string charset)
        {
            return (Seed seed, out (char, Seed) result) =>
            {
                var lng = seed.Next(out var next);
                result = (charset[(int)(lng % (ulong)charset.Length)], next);
                return true;
            };
        }

        internal static Gen<char> HexDigit(bool lower = true) =>
            lower ? CharRange("0123456789abcdef") : CharRange("0123456789ABCDEF");



        /// <summary>
        /// Foundational Gen<int> combinator that chooses a number from the given range of numbers 
        /// (inclusive min, exclusive max)
        /// </summary>
        /// <param name="min">inclusive lower bound of range</param>
        /// <param name="max">exclusive upper bound of range</param>
        public static Gen<int> Range(int min, int max)
        {
            return (Seed seed, out (int, Seed) result) =>
            {
                var value = seed.Next(min, max, out var next);
                result = (value, next);
                return true;
            };
        }

        /// <summary>
        /// Foundational Gen<DateTime> combinator that chooses a date from the given range of dates 
        /// (inclusive min, exclusive max)
        /// </summary>
        /// <param name="min">inclusive lower bound of range</param>
        /// <param name="max">exclusive upper bound of range</param>
        public static Gen<DateTime> Range(DateTime min, DateTime max)
        {
            return (Seed seed, out (DateTime, Seed) result) =>
            {
                var value = seed.Next(min.Ticks, max.Ticks, out var next);
                result = (new DateTime(value), next);
                return true;
            };
        }

        /// <summary>
        /// Gen<string> combinator that produces strings of length n with characters from given character set        
        /// </summary>
        public static Gen<string> String(int n, Gen<char> chars)
        {
            return (Seed seed, out (string, Seed) result) =>
            {
                var sb = new StringBuilder();
                while (sb.Length < n)
                {
                    if (chars(seed, out var temp))
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
            return (Seed seed, out (T, Seed) result) =>
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

        internal static T Cache<T>(this T items, string key, int seconds = 10)
        {
            var item = new CacheItem(key) { Value = new Lazy<T>(() => items) };
            var policy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, seconds) };
            var value = MemoryCache.Default.AddOrGetExisting(item, policy);
            return ((Lazy<T>)item.Value).Value;
        }

        internal static Gen<string> Word => Choose(File
            .ReadLines("text.txt")
            .Where(line => !string.IsNullOrEmpty(line.Trim()))
            .SelectMany(line => line.Split(' '))
            .Select(word => word.Trim(',', '.', ' '))
            .Cache("words")
            .ToList()
        );


        internal static Gen<string> Sentence => Choose(File
            .ReadLines("text.txt")
            .Where(line => !string.IsNullOrEmpty(line.Trim()))
            .Cache("sentences")
            .ToList()
        );

        public static Gen<IReadOnlyList<T>> List<T>(int min, int max, Gen<T> gen)
        {
            return (Seed seed, out (IReadOnlyList<T>, Seed) result) =>
            {
                var accu = new List<T>();
                var len = seed.Next(min, max, out seed);
                while (accu.Count < len)
                {
                    if (gen(seed, out var temp))
                    {
                        accu.Add(temp.Value);
                        seed = temp.Next;
                    }
                }
                result = (accu.AsReadOnly(), seed);
                return true;
            };
        }

        public static Func<IReadOnlyList<T1>, Gen<T>> Linked<T, T1>(System.Func<IReadOnlyList<T1>, Gen<T>> fun)
        {
            return fun;
        }


        public static Func<IReadOnlyList<T1>, IReadOnlyList<T2>, Gen<T>> Linked<T, T1, T2>(System.Func<IReadOnlyList<T1>, IReadOnlyList<T2>, Gen<T>> fun)
        {
            return fun;
        }
    }
}