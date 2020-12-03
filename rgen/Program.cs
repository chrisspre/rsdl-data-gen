using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using generate;


namespace demo
{

    using static Combinators;

    static class Program
    {
        public static void Main()
        {
            // Demo1();
            Demo2();
        }

        public static void Demo1()
        {
            var gen = Create<Person, string, string, string, int>(
                String(6, HexDigit()),
                OneOf("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted"),
                OneOf("Smith", "Miller", "Meyer", "Tailor", "Fisher", "Potter", "Carter", "Cooper"),
                Range(1960, 2010)
            );

            var env = new GenerationParameters();
            foreach (var person in gen.Enumerate(env).Take(12).OrderBy(p => p.lastName))
            {
                Console.WriteLine("{0}", person);
            }
        }


        public static void Demo2()
        {
            var env = new GenerationParameters();

            var options = new JsonSerializerOptions { WriteIndented = true };
            var service = new Container(10, 10, 3);

            foreach (var order in service.Orders)
            {
                Console.WriteLine("{0}", JsonSerializer.Serialize(order, options));
            }
        }

        public class Container
        {
            public IReadOnlyList<Product> Products { get; }
            public IReadOnlyList<Person> Users { get; }
            public IReadOnlyList<Order> Orders { get; }


            public Container(int productCount, int userCount, int orderCount)
            {

                var products = Create<Product, string, string, string>(
                    Concat(Const("P-"), String(4, HexDigit())), //  /P-[0-9a-f]{4}/
                    Word,
                    Sentence
                );

                var users = Create<Person, string, string, string, int>(
                    Concat(Const("U-"), String(4, HexDigit())),
                    OneOf("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted"),
                    OneOf("Smith", "Miller", "Meyer", "Tailor", "Fisher", "Potter", "Carter", "Cooper"),
                    Range(1960, 2010)
                );

                var items = Linked<LineItem, Product>((products) =>
                    Combine((k, a, p) => new LineItem(k, a, p),
                        Concat(Const("I-"), String(4, HexDigit())),
                        Range(1, 10),
                        Choose(products)
                    )
                );

                var orders = Linked<Order, Person, Product>((users, products) =>
                    Combine((i, p, n, d) => new Order(i, p, n, d),
                        Concat(Const("O-"), String(4, HexDigit())),
                        Choose(users),
                        Range(new DateTime(2015, 1, 1), DateTime.Today),
                        List(3, 10, items(products))
                    )
                );


                var env = new GenerationParameters();
                Products = products.Enumerate(env).Take(productCount).ToList();
                Users = users.Enumerate(env).Take(userCount).ToList();
                Orders = orders(Users, Products).Enumerate(env).Take(orderCount).ToList();
            }
        }
    }

    public record Person(string id, string firstName, string lastName, int birthYear)
    {
    }

    public record Product(string Id, string Name, string Description)
    {
    }

    public record Order(string id, Person orderedBy, DateTime ordered, IReadOnlyList<LineItem> items)
    {
    }

    public record LineItem(string id, int amount, Product product)
    {
    }
}