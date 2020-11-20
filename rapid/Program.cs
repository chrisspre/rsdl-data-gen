using System;

namespace rapid
{
    class Program
    {
        static void Main(string[] args)
        {
            // var gen = Generators.CharRange("abcdef");
            // var gen = Generators.String(6, Generators.HexDigit);
            // var gen = Generators.OneOf("Bob", "Sue", "Ted");

            var gen = Generators.Create<Person, string, string, string, int>(
                Generators.String(4, Generators.HexDigit),
                Generators.OneOf("Bob", "Sue", "Ted"),
                Generators.OneOf("Smith", "Miller", "Meyer"),
                Generators.Range(1960, 2010)
            );

            var @params = new Parameters();
            foreach (var item in gen.Enumerate(11, @params))
            {
                Console.WriteLine("{0}", item);
            }
        }
    }






    public record Person(string id, string firstName, string lastName, int year)
    {
    }
}