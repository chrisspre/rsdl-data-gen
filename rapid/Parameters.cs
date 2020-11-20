using System;

namespace rapid
{
    public record Parameters(int seed = 0)
    {
        public Seed InitialSeed()
        {
            return new Seed(seed);
        }
    }
}
