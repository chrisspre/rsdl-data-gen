using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using demo;
using generate;


static class Program
{
    public static void Main()
    {
        Demo1();
        // Demo2();
    }

    public static void Demo1()
    {
        var env = new GeneratorEnvironment();

        var gen = env.Create<Person, string, string, string, int>(
            env.String(4, env.HexDigit()),
            env.OneOf("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted"),
            env.OneOf("Smith", "Miller", "Meyer", "Tailor", "Fisher", "Potter", "Carter", "Cooper"),
            env.Range(1960, 2010)
        );

        foreach (var person in env.Enumerate(gen).Take(12).OrderBy(p => p.lastName))
        {
            Console.WriteLine("{0}", person);
        }
    }



    public static void Demo2()
    {
        var env = new GeneratorEnvironment();

        var genP = env.Combine((i, n, d) => new Product(i, n, d),
            env.String(4, env.HexDigit()),
            env.Word(),
            env.Sentence()
        );
        var products = env.Enumerate(genP).Take(10).ToDictionary(p => p.Id);
        // foreach (var product in products.Values)
        // {
        //     Console.WriteLine("{0}", product);
        // }

        var genI = env.Combine((i, n, d) => new LineItem(i, n, d),
            env.String(4, env.HexDigit()),
            env.Choose(products),
            env.Range(1, 10)
        );

        var gen = env.Combine((i, n, d) => new Order(i, n, d),
            env.String(4, env.HexDigit()),
            env.Range(new DateTime(2015, 1, 1), DateTime.Today),
            env.List(3, 10, genI)
        );

        var orders = env.Enumerate(gen).Take(10);

        var options = new JsonSerializerOptions { WriteIndented = true };
        foreach (var order in orders)
        {
            Console.WriteLine("{0}", JsonSerializer.Serialize(order, options));
        }
    }
}


namespace demo
{



    public record Person(string id, string firstName, string lastName, int birthYear)
    {
    }

    public record Product(string Id, string Name, string Description)
    {
    }

    public record Order(string id, DateTime ordered, IReadOnlyList<LineItem> items)
    {
    }

    public record LineItem(string id, Product product, int amount)
    {
    }
}