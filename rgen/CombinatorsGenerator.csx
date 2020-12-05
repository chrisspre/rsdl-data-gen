#!/usr/bin/env dotnet-script
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

WriteLine($"using System;");
WriteLine($"using System.Collections.Generic;");
WriteLine();
WriteLine($"namespace generate");
WriteLine($"{{");
WriteLine($"  public static partial class Combinators");
WriteLine($"  {{");
for (int n = 1; n <= 6; n++)
{
    GenerateLink(n);
}
// for (int n = 1; n <= 6; n++)
// {
//     GenerateCombine(n);
// }
// for (int n = 1; n <= 6; n++)
// {
//     GenerateCreate(n);
// }
// for (int n = 1; n <= 6; n++)
// {
//     GenerateGetConstructor(n);
// }
Console.WriteLine($"  }}");
Console.WriteLine($"}}");

void GenerateLink(int n)
{
    string List(Func<int, string> fun) =>
        string.Join(", ", from i in Enumerable.Range(1, n) select fun(i));

    Console.WriteLine();
    var func = $"Func<{List(i => $"IReadOnlyList<T{i}>")}, Gen<T>>";
    Console.WriteLine($"    public static {func} Link<T, {List(i => $"T{i}")}>({List(i => $"Gen<T{i}> gen{i}")}, {func} fun)");
    Console.WriteLine($"    {{");
    Console.WriteLine($"        return fun;");
    Console.WriteLine($"    }}");
}

void GenerateCombine(int n)
{
    string List(Func<int, string> fun)
    {
        return string.Join(", ", from i in Enumerable.Range(1, n) select fun(i));
    }

    Console.WriteLine($"    public static Gen<T> Combine<T, {List(i => $"T{i}")}>(");
    Console.WriteLine($"      System.Func<{List(i => $"T{i}")}, T> constr, ");
    for (var i = 1; i <= n; i++)
    {
        Console.WriteLine($"      Gen<T{i}> gen{i}{(i == n ? "" : ",")}");
    }
    Console.WriteLine($"    )");
    Console.WriteLine($"    {{");
    Console.WriteLine($"      return (Seed seed, out (T, Seed) result) => ");
    Console.WriteLine($"      {{ ");
    Console.WriteLine($"        var res0 = (default(T), Next: seed); ");
    for (var i = 1; i <= n; i++)
    {
        Console.WriteLine($"        gen{i}(res{i - 1}.Next, out var res{i}); ");
    }
    Console.WriteLine($"        result = (constr({List(i => $"res{i}.Value")}), res{n}.Next); ");
    Console.WriteLine($"        return true; ");
    Console.WriteLine($"      }}; ");
    Console.WriteLine($"    }}");
    Console.WriteLine($"");
}

void GenerateCreate(int n)
{
    string List(Func<int, string> fun) =>
        string.Join(", ", from i in Enumerable.Range(1, n) select fun(i));

    Console.WriteLine();
    // Console.WriteLine($"      this Generator generator,");
    Console.WriteLine($"    public static Gen<T> Create<T, {List(i => $"T{i}")}>({List(i => $"Gen<T{i}> gen{i}")})");
    Console.WriteLine($"    {{");
    Console.WriteLine($"        return Combine(GetConstructor<T, {List(i => $"T{i}")}>(), {List(i => $"gen{i}")});");
    Console.WriteLine($"    }}");
}

void GenerateGetConstructor(int n)
{
    string List(Func<int, string> fun) =>
            string.Join(", ", from i in Enumerable.Range(1, n) select fun(i));

    Console.WriteLine();
    Console.WriteLine($"    private static Func<{List(i => $"T{i}")}, T> GetConstructor<T, {List(i => $"T{i}")}>()");
    Console.WriteLine($"    {{");
    Console.WriteLine($"        var info = typeof(T).GetConstructor(new[] {{ {List(i => $"typeof(T{i})")} }});");
    Console.WriteLine($"        var @params = new[] {{ {List(i => $"Expression.Parameter(typeof(T{i}))")} }};");
    Console.WriteLine($"        var body = Expression.New(info, @params);");
    Console.WriteLine($"        var lambda = Expression.Lambda<Func<{List(i => $"T{i}")}, T>>(body, @params);");
    Console.WriteLine($"        return lambda.Compile();");
    Console.WriteLine($"    }}");
}
