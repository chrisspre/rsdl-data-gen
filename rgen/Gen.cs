using System;

namespace generate
{
    public delegate bool Gen<T>(Seed seed, out (T Value, Seed Next) result);
}
