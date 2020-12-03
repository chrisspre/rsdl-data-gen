using System;

namespace generate
{
    public static class GeneratorExtensions
    {

        public static Gen<TResult> SelectMany<TSource, TCollection, TResult>(this Gen<TSource> source, Func<TSource, Gen<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return (Seed seed, out (TResult, Seed) result) =>
            {
                if (source(seed, out var r1))
                {
                    if (collectionSelector(r1.Value)(r1.Next, out var r2))
                    {
                        result = (resultSelector(r1.Value, r2.Value), r2.Next);
                        return true;
                    }
                }
                result = default;
                return false;
            };
        }
    }
}