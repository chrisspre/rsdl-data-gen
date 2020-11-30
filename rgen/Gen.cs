using System.Collections.Generic;

namespace generate
{
    public delegate bool Gen<T>(Seed seed, out (T Value, Seed Next) result);

    public record Environment(int seed = 0, int MaxUpperBound = 100)
    {
        public Seed InitialSeed => new Seed(seed);
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
        public static IEnumerable<T> Enumerate<T>(this Environment env, Gen<T> gen)
        {
            var seed = env.InitialSeed;
            while (true)
            {
                if (gen(seed, out var result))
                {
                    yield return result.Value;
                    seed = result.Next;
                }
            }
        }
    }
}