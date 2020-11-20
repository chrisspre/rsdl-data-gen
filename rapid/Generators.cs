using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace rapid
{
    public delegate bool Gen<T>(Parameters parameters, Seed seed, out (T Value, Seed Next) result);

    public static class Generators
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

        public static Gen<T> Combine<T, A, B>(System.Func<A, B, T> constr,
            Gen<A> gen1,
            Gen<B> gen2)
        {
            return (Parameters @params, Seed seed, out (T, Seed) result) =>
            {
                // TODO: check for failure
                var res0 = (default(T), Next: seed);
                gen1(@params, res0.Next, out var res1);
                gen2(@params, res1.Next, out var res2);
                result = (constr(res1.Value, res2.Value), res2.Next);
                return true;
            };
        }

        public static Gen<T> Combine<T, A, B, C>(System.Func<A, B, C, T> constr,
            Gen<A> gen1,
            Gen<B> gen2,
            Gen<C> gen3)
        {
            return (Parameters @params, Seed seed, out (T, Seed) result) =>
            {
                // TODO: check for failure
                var res0 = (default(T), Next: seed);
                gen1(@params, res0.Next, out var res1);
                gen2(@params, res1.Next, out var res2);
                gen3(@params, res2.Next, out var res3);
                result = (constr(res1.Value, res2.Value, res3.Value), res3.Next);
                return true;
            };
        }

        public static Gen<T> Combine<T, A, B, C, D>(System.Func<A, B, C, D, T> constr,
                   Gen<A> gen1,
                   Gen<B> gen2,
                   Gen<C> gen3,
                   Gen<D> gen4)
        {
            return (Parameters @params, Seed seed, out (T, Seed) result) =>
            {
                var res0 = (default(T), Next: seed);
                gen1(@params, res0.Next, out var res1);
                gen2(@params, res1.Next, out var res2);
                gen3(@params, res2.Next, out var res3);
                gen4(@params, res3.Next, out var res4);
                result = (constr(res1.Value, res2.Value, res3.Value, res4.Value), res4.Next);
                return true;
            };
        }


        internal static Gen<T> Create<T, T1, T2, T3, T4>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Gen<T4> gen4)
        {
            var info = typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
            Func<T1, T2, T3, T4, T> fun = (a, b, c, d) => (T)info.Invoke(new object[] { a, b, c, d });
            return Combine(fun, gen1, gen2, gen3, gen4);
        }
    }
}