using System.Collections.Generic;
using demo;

namespace generate
{

    public record GenerationParameters(int seed = 0, int MaxUpperBound = 100)
    {
        public Seed InitialSeed => new Seed(seed);

    }

    public static partial class GeneratorEnvironmentExtensions
    {
        /// <summary>
        /// Gen<T> extension to generate a sequence of values
        /// </summary>
        /// <param name="gen"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Enumerate<T>(this Gen<T> gen, GenerationParameters env)
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