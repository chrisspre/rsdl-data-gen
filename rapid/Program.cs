using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace rapid
{
    class Program
    {
        static void Main(string[] args)
        {
            Demo2();
        }

        static void Demo1()
        {
            var generator = new Params();
            // var gen = generator.CharRange("abcdef");
            // var gen = generator.String(6, generator.HexDigit);
            // var gen = generator.OneOf("Bob", "Sue", "Ted");

            var gen = generator.Create<Person, string, string, string, int>(
                generator.String(4, generator.HexDigit()),
                generator.OneOf("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted"),
                generator.OneOf("Smith", "Miller", "Meyer", "Tailor", "Fisher"),
                generator.Range(1960, 2010)
            );

            foreach (var person in generator.Enumerate(gen).Take(10).OrderBy(p => p.lastName))
            {
                Console.WriteLine("{0}", person);
            }
        }



        static void Demo2()
        {
            var generator = new Params();

            var genP = generator.Combine((i, n, d) => new Product(i, n, d),
                generator.String(4, generator.HexDigit()),
                generator.Word(),
                generator.Sentence()
            );
            var products = generator.Enumerate(genP).Take(10).ToDictionary(p => p.Id);
            // foreach (var product in products.Values)
            // {
            //     Console.WriteLine("{0}", product);
            // }

            var genI = generator.Combine((i, n, d) => new LineItem(i, n, d),
                generator.String(4, generator.HexDigit()),
                generator.Choose(products),
                generator.Range(1, 10)
            );

            var gen = generator.Combine((i, n, d) => new Order(i, n, d),
                generator.String(4, generator.HexDigit()),
                generator.Range(new DateTime(2015, 1, 1), DateTime.Today),
                generator.List(3, 10, genI)
            );

            var orders = generator.Enumerate(gen).Take(10);

            var options = new JsonSerializerOptions { WriteIndented = true };
            foreach (var order in orders)
            {
                Console.WriteLine("{0}", JsonSerializer.Serialize(order, options));
            }
        }
    }




    public record Person(string id, string firstName, string lastName, int year)
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