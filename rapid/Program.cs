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
            var @params = new Params();
            // var gen = Generators.CharRange("abcdef");
            // var gen = Generators.String(6, Generators.HexDigit);
            // var gen = Generators.OneOf("Bob", "Sue", "Ted");

            var gen = Generators.Create<Person, string, string, string, int>(
                Generators.String(4, Generators.HexDigit),
                Generators.OneOf("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted"),
                Generators.OneOf("Smith", "Miller", "Meyer", "Tailor", "Fisher"),
                Generators.Range(1960, 2010)
            );

            foreach (var person in gen.Enumerate(@params).Take(10).OrderBy(p => p.lastName))
            {
                Console.WriteLine("{0}", person);
            }
        }



        static void Demo2()
        {
            var @params = new Params();

            var genP = Generators.Combine((i, n, d) => new Product(i, n, d),
                Generators.String(4, Generators.HexDigit),
                Generators.Word,
                Generators.Sentence
            );
            var products = genP.Enumerate(@params).Take(10).ToDictionary(p => p.Id);
            // foreach (var product in products.Values)
            // {
            //     Console.WriteLine("{0}", product);
            // }

            var genI = Generators.Combine((i, n, d) => new LineItem(i, n, d),
                Generators.String(4, Generators.HexDigit),
                Generators.Choose(products),
                Generators.Range(1, 10)
            );

            var gen = Generators.Combine((i, n, d) => new Order(i, n, d),
                Generators.String(4, Generators.HexDigit),
                Generators.Range(new DateTime(2015, 1, 1), DateTime.Today),
                Generators.List(3, 10, genI)
            );

            var orders = gen.Enumerate(@params).Take(10);

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