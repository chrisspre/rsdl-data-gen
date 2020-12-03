using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using generate;


namespace demo
{

    static class Program
    {
        public static void Main()
        {
            // Demo1();
            Demo2();
        }

        public static void Demo1()
        {
            var env = new GeneratorEnvironment();

            var gen = env.Create<Person, string, string, string, int>(
                env.String(6, env.HexDigit()),
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
                var env = new GeneratorEnvironment();

                var products = env.Combine((i, n, d) => new Product(i, n, d),
                    env.Concat(env.Const("P-"), env.String(4, env.HexDigit())),
                    env.Word(),
                    env.Sentence()
                );

                var users = env.Create<Person, string, string, string, int>(
                    env.Concat(env.Const("U-"), env.String(4, env.HexDigit())),
                    env.OneOf("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted"),
                    env.OneOf("Smith", "Miller", "Meyer", "Tailor", "Fisher", "Potter", "Carter", "Cooper"),
                    env.Range(1960, 2010)
                );

                var items = env.Linked<LineItem, Product>((i) => env.Combine((k, a, p) => new LineItem(k, a, p),
                    env.Concat(env.Const("I-"), env.String(4, env.HexDigit())),
                    env.Range(1, 10),
                    env.Choose(i)
                ));

                var orders = env.Linked<Order, Person, Product>((u, p) => env.Combine((i, p, n, d) => new Order(i, p, n, d),
                    env.Concat(env.Const("O-"), env.String(4, env.HexDigit())),
                    env.Choose(u),
                    env.Range(new DateTime(2015, 1, 1), DateTime.Today),
                    env.List(3, 10, items(p))
                ));

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