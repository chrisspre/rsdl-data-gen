using System;
using System.Linq.Expressions;

namespace rapid
{
  public static partial class Generators
  {
    public static Gen<T> Combine<T, T1>(
      System.Func<T1, T> constr, 
      Gen<T1> gen1
    )
    {
      return (Parameters @params, Seed seed, out (T, Seed) result) => 
      { 
        var res0 = (default(T), Next: seed); 
        gen1(@params, res0.Next, out var res1); 
        result = (constr(res1.Value), res1.Next); 
        return true; 
      }; 
    }

    public static Gen<T> Combine<T, T1, T2>(
      System.Func<T1, T2, T> constr, 
      Gen<T1> gen1,
      Gen<T2> gen2
    )
    {
      return (Parameters @params, Seed seed, out (T, Seed) result) => 
      { 
        var res0 = (default(T), Next: seed); 
        gen1(@params, res0.Next, out var res1); 
        gen2(@params, res1.Next, out var res2); 
        result = (constr(res1.Value, res2.Value), res2.Next); 
        return true; 
      }; 
    }

    public static Gen<T> Combine<T, T1, T2, T3>(
      System.Func<T1, T2, T3, T> constr, 
      Gen<T1> gen1,
      Gen<T2> gen2,
      Gen<T3> gen3
    )
    {
      return (Parameters @params, Seed seed, out (T, Seed) result) => 
      { 
        var res0 = (default(T), Next: seed); 
        gen1(@params, res0.Next, out var res1); 
        gen2(@params, res1.Next, out var res2); 
        gen3(@params, res2.Next, out var res3); 
        result = (constr(res1.Value, res2.Value, res3.Value), res3.Next); 
        return true; 
      }; 
    }

    public static Gen<T> Combine<T, T1, T2, T3, T4>(
      System.Func<T1, T2, T3, T4, T> constr, 
      Gen<T1> gen1,
      Gen<T2> gen2,
      Gen<T3> gen3,
      Gen<T4> gen4
    )
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

    public static Gen<T> Combine<T, T1, T2, T3, T4, T5>(
      System.Func<T1, T2, T3, T4, T5, T> constr, 
      Gen<T1> gen1,
      Gen<T2> gen2,
      Gen<T3> gen3,
      Gen<T4> gen4,
      Gen<T5> gen5
    )
    {
      return (Parameters @params, Seed seed, out (T, Seed) result) => 
      { 
        var res0 = (default(T), Next: seed); 
        gen1(@params, res0.Next, out var res1); 
        gen2(@params, res1.Next, out var res2); 
        gen3(@params, res2.Next, out var res3); 
        gen4(@params, res3.Next, out var res4); 
        gen5(@params, res4.Next, out var res5); 
        result = (constr(res1.Value, res2.Value, res3.Value, res4.Value, res5.Value), res5.Next); 
        return true; 
      }; 
    }

    public static Gen<T> Combine<T, T1, T2, T3, T4, T5, T6>(
      System.Func<T1, T2, T3, T4, T5, T6, T> constr, 
      Gen<T1> gen1,
      Gen<T2> gen2,
      Gen<T3> gen3,
      Gen<T4> gen4,
      Gen<T5> gen5,
      Gen<T6> gen6
    )
    {
      return (Parameters @params, Seed seed, out (T, Seed) result) => 
      { 
        var res0 = (default(T), Next: seed); 
        gen1(@params, res0.Next, out var res1); 
        gen2(@params, res1.Next, out var res2); 
        gen3(@params, res2.Next, out var res3); 
        gen4(@params, res3.Next, out var res4); 
        gen5(@params, res4.Next, out var res5); 
        gen6(@params, res5.Next, out var res6); 
        result = (constr(res1.Value, res2.Value, res3.Value, res4.Value, res5.Value, res6.Value), res6.Next); 
        return true; 
      }; 
    }


    public static Gen<T> Create<T, T1>(Gen<T1> gen1)
    {
        return Combine(GetConstructor<T, T1>(), gen1);
    }

    public static Gen<T> Create<T, T1, T2>(Gen<T1> gen1, Gen<T2> gen2)
    {
        return Combine(GetConstructor<T, T1, T2>(), gen1, gen2);
    }

    public static Gen<T> Create<T, T1, T2, T3>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3)
    {
        return Combine(GetConstructor<T, T1, T2, T3>(), gen1, gen2, gen3);
    }

    public static Gen<T> Create<T, T1, T2, T3, T4>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Gen<T4> gen4)
    {
        return Combine(GetConstructor<T, T1, T2, T3, T4>(), gen1, gen2, gen3, gen4);
    }

    public static Gen<T> Create<T, T1, T2, T3, T4, T5>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Gen<T4> gen4, Gen<T5> gen5)
    {
        return Combine(GetConstructor<T, T1, T2, T3, T4, T5>(), gen1, gen2, gen3, gen4, gen5);
    }

    public static Gen<T> Create<T, T1, T2, T3, T4, T5, T6>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3, Gen<T4> gen4, Gen<T5> gen5, Gen<T6> gen6)
    {
        return Combine(GetConstructor<T, T1, T2, T3, T4, T5, T6>(), gen1, gen2, gen3, gen4, gen5, gen6);
    }

    private static Func<T1, T> GetConstructor<T, T1>()
    {
        var info = typeof(T).GetConstructor(new[] { typeof(T1) });
        var @params = new[] { Expression.Parameter(typeof(T1)) };
        var body = Expression.New(info, @params);
        var lambda = Expression.Lambda<Func<T1, T>>(body, @params);
        return lambda.Compile();
    }

    private static Func<T1, T2, T> GetConstructor<T, T1, T2>()
    {
        var info = typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2) });
        var @params = new[] { Expression.Parameter(typeof(T1)), Expression.Parameter(typeof(T2)) };
        var body = Expression.New(info, @params);
        var lambda = Expression.Lambda<Func<T1, T2, T>>(body, @params);
        return lambda.Compile();
    }

    private static Func<T1, T2, T3, T> GetConstructor<T, T1, T2, T3>()
    {
        var info = typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3) });
        var @params = new[] { Expression.Parameter(typeof(T1)), Expression.Parameter(typeof(T2)), Expression.Parameter(typeof(T3)) };
        var body = Expression.New(info, @params);
        var lambda = Expression.Lambda<Func<T1, T2, T3, T>>(body, @params);
        return lambda.Compile();
    }

    private static Func<T1, T2, T3, T4, T> GetConstructor<T, T1, T2, T3, T4>()
    {
        var info = typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        var @params = new[] { Expression.Parameter(typeof(T1)), Expression.Parameter(typeof(T2)), Expression.Parameter(typeof(T3)), Expression.Parameter(typeof(T4)) };
        var body = Expression.New(info, @params);
        var lambda = Expression.Lambda<Func<T1, T2, T3, T4, T>>(body, @params);
        return lambda.Compile();
    }

    private static Func<T1, T2, T3, T4, T5, T> GetConstructor<T, T1, T2, T3, T4, T5>()
    {
        var info = typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
        var @params = new[] { Expression.Parameter(typeof(T1)), Expression.Parameter(typeof(T2)), Expression.Parameter(typeof(T3)), Expression.Parameter(typeof(T4)), Expression.Parameter(typeof(T5)) };
        var body = Expression.New(info, @params);
        var lambda = Expression.Lambda<Func<T1, T2, T3, T4, T5, T>>(body, @params);
        return lambda.Compile();
    }

    private static Func<T1, T2, T3, T4, T5, T6, T> GetConstructor<T, T1, T2, T3, T4, T5, T6>()
    {
        var info = typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
        var @params = new[] { Expression.Parameter(typeof(T1)), Expression.Parameter(typeof(T2)), Expression.Parameter(typeof(T3)), Expression.Parameter(typeof(T4)), Expression.Parameter(typeof(T5)), Expression.Parameter(typeof(T6)) };
        var body = Expression.New(info, @params);
        var lambda = Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T>>(body, @params);
        return lambda.Compile();
    }
  }
}
