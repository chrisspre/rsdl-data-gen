using System;
using System.Collections.Generic;

namespace generate
{
  public static partial class Combinators
  {

    public static Func<IReadOnlyList<T1>, Gen<T>> Link<T, T1>(Gen<T1> gen1, Func<IReadOnlyList<T1>, Gen<T>> fun)
    {
        return fun;
    }

    public static Func<IReadOnlyList<T1>, IReadOnlyList<T2>, Gen<T>> Link<T, T1, T2>(Gen<T1> gen1, Gen<T2> gen2, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, Gen<T>> fun)
    {
        return fun;
    }

    public static Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, Gen<T>> Link<T, T1, T2, T3>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, Gen<T>> fun)
    {
        return fun;
    }

    public static Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, IReadOnlyList<T4>, Gen<T>> Link<T, T1, T2, T3, T4>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Gen<T4> gen4, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, IReadOnlyList<T4>, Gen<T>> fun)
    {
        return fun;
    }

    public static Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, IReadOnlyList<T4>, IReadOnlyList<T5>, Gen<T>> Link<T, T1, T2, T3, T4, T5>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Gen<T4> gen4, Gen<T5> gen5, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, IReadOnlyList<T4>, IReadOnlyList<T5>, Gen<T>> fun)
    {
        return fun;
    }

    public static Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, IReadOnlyList<T4>, IReadOnlyList<T5>, IReadOnlyList<T6>, Gen<T>> Link<T, T1, T2, T3, T4, T5, T6>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Gen<T4> gen4, Gen<T5> gen5, Gen<T6> gen6, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IReadOnlyList<T3>, IReadOnlyList<T4>, IReadOnlyList<T5>, IReadOnlyList<T6>, Gen<T>> fun)
    {
        return fun;
    }
  }
}
