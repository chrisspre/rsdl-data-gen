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
        /// generates an infinite sequence of generated values
        /// </summary>
        /// <param name="gen">generator</param>
        /// <typeparam name="T">generator element type</typeparam>
        /// <returns>an infinite sequence of generates values of type T</returns>
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