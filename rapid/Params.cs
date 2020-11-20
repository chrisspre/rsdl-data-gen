using System;

namespace rapid
{
    public record Params(int seed = 0)
    {
        public Seed InitialSeed()
        {
            return new Seed(seed);
        }
    }
}
