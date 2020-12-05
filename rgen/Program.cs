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
            Demo1();
            // Demo2();
        }

        public static void Demo1()
        {
            var gen =
                from i in String(6, HexDigit())
                from f in Choose("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted")
                from l in Choose("Smith", "Miller", "Meyer", "Tailor", "Fisher", "Potter", "Carter", "Cooper")
                from y in Range(1960, 2010)
                select new Person(i, f, l, y);

            var env = new GenerationParameters();
            foreach (var person in gen.Enumerate(env).Take(12).OrderBy(p => p.lastName))
            {
                Console.WriteLine("{0}", person);
            }
        }


        public static void Demo2()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            var env = new GenerationParameters();
            var service = new Container(env, 10, 10, 3);

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


            public Container(GenerationParameters env, int productCount, int userCount, int orderCount)
            {
                // generator for Product
                var products =
                    from key in String(6, HexDigit())
                    from nam in Word
                    from des in Sentence
                    select new Product(key, nam, des);

                // generator for Person
                var users =
                    from id in Concat(Const("U-"), String(4, HexDigit()))
                    from fi in Choose("Abe", "Joe", "Bea", "Ben", "Bob", "Sue", "Sky", "Roy", "Ted")
                    from la in Choose("Smith", "Miller", "Meyer", "Tailor", "Fisher", "Potter", "Carter", "Cooper")
                    from ye in Range(1960, 2010)
                    select new Person(id, fi, la, ye);

                // generator for LineItem dependent on a products generator
                var items = Link(products, (products) =>
                    from k in Concat(Const("I-"), String(4, HexDigit()))
                    from a in Range(1, 10)
                    from p in Choose(products)
                    select new LineItem(k, a, p)
                );

                // generator for Orders 
                var orders = Link(users, products, (users, products) =>
                    from k in Concat(Const("O-"), String(4, HexDigit()))
                    from p in Choose(users)
                    from n in Range(new DateTime(2015, 1, 1), DateTime.Today)
                    from i in items(products).List(3, 7)
                    select new Order(k, p, n, i)
                );

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